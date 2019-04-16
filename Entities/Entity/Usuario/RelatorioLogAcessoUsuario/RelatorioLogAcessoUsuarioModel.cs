using System;

namespace Entities.Entity.Usuario
{
    public class RelatorioLogAcessoUsuarioModel
    {
        public int IdUsuario { get; set; }
        public DateTime DataReferenciaInicio { get; set; }
        public DateTime DataReferenciaFim { get; set; }
    }
}
