using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Entity.Table
{
    [Table("Funcionario_Telefone")]
    public class Funcionario_Telefone : BaseTable
    {
        [Key]
        public int IdFuncionarioTelefone { get; set; }

        [Required]
        public int IdFuncionario { get; set; }

        [Required]
        public byte Tipo { get; set; }

        [Required]
        public long Numero { get; set; }
    }
}
