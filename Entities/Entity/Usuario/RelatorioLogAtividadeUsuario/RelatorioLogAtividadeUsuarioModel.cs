using System;

namespace Entities.Entity.Usuario
{
    public class RelatorioLogAtividadeUsuarioModel
    {
        public int IdUsuario { get; set; }
        public DateTime DataReferencia { get; set; }
        public int Aplicacao { get; set; }
        public short Permissao { get; set; }
    }
}
