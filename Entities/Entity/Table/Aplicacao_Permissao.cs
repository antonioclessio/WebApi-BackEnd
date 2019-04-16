using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Entity.Table
{
    [Table("Aplicacao_Permissao")]
    public class Aplicacao_Permissao
    {
        [Key]
        public int IdAplicacaoPermissao { get; set; }

        [Required]
        public int IdAplicacao { get; set; }

        [Required]
        public byte Status { get; set; }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; }

        [Required]
        public byte Indice { get; set; }

        [Required]
        public string Descricao { get; set; }
    }
}
