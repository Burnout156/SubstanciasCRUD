using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SubstanciasDatabase;
using SubstanciasDatabase.Criptografia;
using SubstanciasDatabase.Repositories;
using SubstanciasDatabase.Services;
using System.Security.Claims;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// -----------------------------------------------------------------------------
// Banco de Dados (Postgres)
// -----------------------------------------------------------------------------
builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseNpgsql(builder.Configuration.GetConnectionString("Default"));
});

// -----------------------------------------------------------------------------
// Dependências
// -----------------------------------------------------------------------------
var aesKey = builder.Configuration["Crypto:AesKeyBase64"]
    ?? throw new InvalidOperationException("Crypto:AesKeyBase64 não configurado");

builder.Services.AddSingleton(new AesGcmStringProtector(aesKey));
builder.Services.AddScoped<ISubstanciaRepository, SubstanciaRepository>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<ISubstanciaService, SubstanciaService>();

builder.Services.AddControllers()
    .AddJsonOptions(opt => opt.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

// -----------------------------------------------------------------------------
// CORS (Angular em https://localhost:4200)
// -----------------------------------------------------------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy
            .WithOrigins("https://localhost:4200", "http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// -----------------------------------------------------------------------------
// JWT / Keycloak
// -----------------------------------------------------------------------------
JsonWebTokenHandler.DefaultMapInboundClaims = false; // não remapear automaticamente
var authority = "http://localhost:8080/realms/substances"; // Keycloak (dev em HTTP)

// Atenção: em dev, Keycloak está em HTTP, então:
var metadataAddress = $"{authority}/.well-known/openid-configuration";

// Aceitar ambos os audiences (account e frontend) para destravar o 401.
// Quando ajustar o Keycloak para emitir aud=frontend, ative ValidateAudience=true
// e deixe apenas "frontend" aqui.
var acceptedAudiences = new[] { "frontend", "account" };

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MetadataAddress = metadataAddress;
        options.Authority = authority;
        options.RequireHttpsMetadata = false; // Keycloak em http no dev

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = authority,

            // 👇 Aceita os dois 'aud' para acabar com o erro agora
            ValidateAudience = true,
            ValidAudiences = acceptedAudiences,

            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(2),

            // Mapear corretamente os claims
            NameClaimType = "preferred_username",
            RoleClaimType = "roles" // veremos abaixo como popular isso
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = ctx =>
            {
                var auth = ctx.Request.Headers.Authorization.ToString();
                if (!string.IsNullOrWhiteSpace(auth))
                    Console.WriteLine($"🔎 Authorization recebido (len={auth.Length}).");
                else
                    Console.WriteLine("🔎 Authorization ausente.");
                return Task.CompletedTask;
            },
            OnTokenValidated = ctx =>
            {
                var jwt = ctx.Principal;
                var auds = string.Join(",", jwt?.Claims.Where(c => c.Type == "aud").Select(c => c.Value) ?? Array.Empty<string>());
                var azp = jwt?.Claims.FirstOrDefault(c => c.Type == "azp")?.Value;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"✅ Token validado | aud(s)=[{auds}] | azp={azp}");
                Console.ResetColor();
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = ctx =>
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("❌ OnAuthenticationFailed:");
                Console.WriteLine(ctx.Exception.ToString());
                Console.ResetColor();
                return Task.CompletedTask;
            },
            OnChallenge = ctx =>
            {
                Console.WriteLine($"⚠️  Challenge: token ausente/ inválido. Error={ctx.Error}, Desc={ctx.ErrorDescription}");
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    // se usar [Authorize(Roles="...")], vamos mapear roles de realm_access.roles
    // como "roles" em um transformer simples.
});

builder.Services.AddSingleton<IClaimsTransformation, KeycloakRealmRolesTransformer>();

// -----------------------------------------------------------------------------
// Swagger
// -----------------------------------------------------------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// -----------------------------------------------------------------------------
// Migrations automáticas
// -----------------------------------------------------------------------------
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// -----------------------------------------------------------------------------
// Pipeline
// -----------------------------------------------------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Em HTTPS no Kestrel (confiando no certificado dev do .NET)
app.UseCors("AllowAngular");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// -----------------------------------------------------------------------------
// Transformer: extrai realm_access.roles => "roles"
// -----------------------------------------------------------------------------
public class KeycloakRealmRolesTransformer : IClaimsTransformation
{
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var identity = principal.Identity as ClaimsIdentity;
        if (identity == null) return Task.FromResult(principal);

        // realm_access: {"roles": [...]}
        var realmAccessJson = identity.FindFirst("realm_access")?.Value;
        if (!string.IsNullOrEmpty(realmAccessJson))
        {
            try
            {
                using var doc = JsonDocument.Parse(realmAccessJson);
                if (doc.RootElement.TryGetProperty("roles", out var rolesElem) && rolesElem.ValueKind == JsonValueKind.Array)
                {
                    foreach (var r in rolesElem.EnumerateArray())
                    {
                        var role = r.GetString();
                        if (!string.IsNullOrWhiteSpace(role) &&
                            !identity.Claims.Any(c => c.Type == "roles" && c.Value == role))
                        {
                            identity.AddClaim(new Claim("roles", role));
                        }
                    }
                }
            }
            catch { /* ignora parse errors */ }
        }

        return Task.FromResult(principal);
    }
}
