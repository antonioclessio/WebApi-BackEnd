using Entities.Enum;
using System;
using System.Collections.Generic;

namespace Entities.Entity.Usuario
{
    public class UsuarioDetailResult : BaseDetailModel
    {
        public int IdUsuario { get; set; }
        public byte Status { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public DateTime DataHoraCadastro { get; set; }
        public DateTime? UltimoAcesso { get; set; }

        public string DescricaoStatus { get { return System.Enum.GetName(typeof(DefaultStatusEnum), (int)Status); } }
    }
}
