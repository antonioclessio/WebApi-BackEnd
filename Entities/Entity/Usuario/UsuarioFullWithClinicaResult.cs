using Entities.Entity.Clinica_Usuario;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Entity.Usuario
{
    [NotMapped]
    public class UsuarioFullWithClinicaResult : Table.Usuario
    {
        public List<ClinicaUsuarioResult> Clinicas { get; set; } = new List<ClinicaUsuarioResult>();
    }
}
