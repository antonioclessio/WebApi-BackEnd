using System;

namespace Entities.Entity.Usuario
{
    public class RelatorioLogAtividadeUsuarioResult
    {
        public string Unidade { get; set; }
        public DateTime DataHoraCadastro { get; set; }
        public string Aplicacao { get; set; }
        public string Acao { get; set; }
        public string Observacao { get; set; }
    }
}
