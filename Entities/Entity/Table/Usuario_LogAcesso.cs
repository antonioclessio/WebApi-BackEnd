using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Entity.Table
{
    [Table("Usuario_LogAcesso")]
    public class Usuario_LogAcesso : BaseTable
    {
        [Key]
        public int IdUsuarioLogAcesso { get; set; }

        [Required]
        public int IdUsuario { get; set; }

        [Required]
        public string IPOrigem { get; set; }

        [Required]
        public int IdClinica { get; set; }
    }
}
