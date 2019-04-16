using Entities.Interfaces.V1;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Entity.Table
{
    [Table("AppMainMenu")]
    public class AppMainMenu : ITable
    {
        [Key]
        public int IdMainMenu { get; set; }

        [Required]
        public byte Status { get; set; }

        [Required]
        [StringLength(50)]
        public string Label { get; set; }

        [StringLength(30)]
        public string Icon { get; set; }

        [Required]
        public byte Ordem { get; set; }

        public int? IdParentMenu { get; set; }

        public int? IdAplicacao { get; set; }

        [Required]
        public bool TemDivisor { get; set; }

        [Required]
        public bool Favorito { get; set; }
    }
}
