using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Entity.Table
{
    [Table("LocalizacaoGeografica")]
    public class LocalizacaoGeografica : BaseTable
    {
        [Key]
        public int IdLocalizacaoGeografica { get; set; }

        [Required]
        [StringLength(8)]
        public string CEP { get; set; }

        [Required]
        public int IdPais { get; set; }

        [Required]
        public int IdRegiao { get; set; }

        [Required]
        public int IdEstado { get; set; }

        [Required]
        public int IdCidade { get; set; }

        public int? IdBairro { get; set; }

        [StringLength(100)]
        public string Logradouro { get; set; }

        [StringLength(15)]
        public string Latitude { get; set; }

        [StringLength(15)]
        public string Longitude { get; set; }

        [StringLength(15)]
        public string Altitude { get; set; }

        public int? DDD { get; set; }

        [Required]
        public bool Revisao { get; set; }
    }
}
