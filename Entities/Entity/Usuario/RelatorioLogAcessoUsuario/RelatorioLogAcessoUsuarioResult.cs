using System;

namespace Entities.Entity.Usuario
{
    public class RelatorioLogAcessoUsuarioResult
    {
        public DateTime DataHoraAcesso { get; set; }
        public string IPOrigem { get; set; }
        public string Unidade { get; set; }
    }
}
