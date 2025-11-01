using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubstanciasDatabase.Criptografia
{
    public sealed class EncryptedStringConverter : ValueConverter<string?, string?>
    {
        public EncryptedStringConverter(AesGcmStringProtector protector)
            : base(
                valor => protector.Encrypt(valor),
                valor => protector.Decrypt(valor))
        { }
    }
}
