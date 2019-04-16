using Entities.Entity.Clinica_Usuario;
using System.Collections.Generic;

namespace Entities.Entity.Usuario
{
    public class UsuarioForm : BaseFormModel
    {
        public int IdUsuario { get; set; }
        public byte Status { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }

        public List<ClinicaUsuarioResult> Clinicas { get; set; } = new List<ClinicaUsuarioResult>();
    }
}
