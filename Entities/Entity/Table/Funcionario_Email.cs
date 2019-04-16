using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Entity.Table
{
    [Table("Funcionario_Email")]
    public class Funcionario_Email : BaseTable
    {
        [Key]
        public int IdFuncionarioEmail { get; set; }

        [Required]
        public int IdFuncionario { get; set; }

        [Required]
        public byte Tipo { get; set; }

        [Required]
        [StringLength(50)]
        public string Email { get; set; }
    }
}
