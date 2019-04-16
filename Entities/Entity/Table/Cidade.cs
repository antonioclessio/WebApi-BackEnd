using Entities.Interfaces.V1;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Entity.Table
{
    [Table("Cidade")]
    public class Cidade : ITable
    {
        [Key]
        public int IdCidade { get; set; }

        [Required]
        public DateTime DataHoraCadastro { get; set; }

        [Required]
        public byte Status { get; set; }

        [Required]
        public int IdEstado { get; set; }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; }

        [StringLength(10)]
        public string CodigoIBGE { get; set; }

        [Required]
        public bool Revisao { get; set; }
    }
}
