using System;

namespace Entities.Entity.LocalizacaoGeografica
{
    public class LocalizacaoGeograficaResult
    {
        public int IdCidade { get; set; }
        public int IdLocalizacaoGeografica { get; set; }
        public string Estado { get; set; }
        public string Cidade { get; set; }
        public string Bairro { get; set; }
        public string LocalizacaoGeografica { get; set; }
        public string NumeroEndereco { get; set; }
        public string Complemento { get; set; }
        public string Logradouro { get; set; }
        public string CEP { get; set; }
        public string CEPFormatado
        {
            get
            {
                if (CEP == null) return null;
                return Convert.ToUInt64(CEP).ToString(@"00000-000");
            }
        }
    }
}
