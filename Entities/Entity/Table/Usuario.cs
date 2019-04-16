using Entities.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Entity.Table
{
    [Table("Usuario")]
    public class Usuario : BaseTable
    {
        [Key]
        public int IdUsuario { get; set; }

        [Required]
        [StringLength(50)]
        [LogAtividade]
        public string Nome { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Email { get; set; }
        
        [Required]
        [StringLength(512)]
        public string Senha { get; set; }

        [StringLength(512)]
        public string SenhaSalt { get; set; }

        [Required]
        public bool PrimeiroAcesso { get; set; }
    }
}
