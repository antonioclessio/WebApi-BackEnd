using Entities.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Entity.Table
{
    [Table("Feriado")]
    public class Feriado : BaseTable
    {
        [Key]
        public int IdFeriado { get; set; }

        [Required]
        [StringLength(50)]
        [LogAtividade]
        public string Nome { get; set; }

        [Required]
        public DateTime Data { get; set; }

        [Required]
        public byte Tipo { get; set; }

        public int? IdEstado { get; set; }

        public int? IdCidade { get; set; }
    }
}
