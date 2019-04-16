using Entities.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Entity.Table
{
    [Table("Profissao")]
    public class Profissao : BaseTable
    {
        [Key]
        public int IdProfissao { get; set; }

        [Required]
        [StringLength(40)]
        [LogAtividade]
        public string Nome { get; set; }
    }
}
