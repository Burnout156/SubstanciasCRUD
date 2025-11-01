import { KeycloakService } from 'keycloak-angular';

export function initializeKeycloak(keycloak: KeycloakService) {
  return async () => {
    try {
      // 🔹 Inicializa o Keycloak antes do Angular subir
      await keycloak.init({
        config: {
          url: 'http://localhost:8080',
          realm: 'substances',
          clientId: 'frontend', // mesmo client configurado no Keycloak
        },
        initOptions: {
          onLoad: 'login-required',
          checkLoginIframe: false,
        },
        enableBearerInterceptor: false, // ❌ usamos nosso interceptor manual
      });

      // 🔹 Torna o Keycloak global (para debug no console)
      (window as any).keycloak = keycloak.getKeycloakInstance();

      // 🔹 Obtém e decodifica o token
      const token = keycloak.getKeycloakInstance().token;
      if (token) {
        const payload = JSON.parse(atob(token.split('.')[1]));
        console.log('✅ Keycloak iniciado.');
        console.log('🔑 Token (parcial):', token.substring(0, 40) + '...');
        console.log('--- TOKEN PAYLOAD ---');
        console.table(payload);
        console.log('iss:', payload.iss);
        console.log('aud:', payload.aud);
        console.log('azp:', payload.azp);
      } else {
        console.warn('⚠️ Nenhum token foi retornado pelo Keycloak.');
      }

      // 🔁 Atualiza automaticamente o token antes de expirar
      setInterval(async () => {
        const updated = await keycloak.updateToken(30);
        if (updated) {
          console.log('🔁 Token renovado automaticamente.');
        }
      }, 10000);

    } catch (err) {
      console.error('❌ Erro ao inicializar Keycloak', err);
    }
  };
}
