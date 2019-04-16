using Entities.Interfaces.V1;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Entity.Table
{
    [Table("Aplicacao")]
    public class Aplicacao : ITable
    {
        [Key]
        public int IdAplicacao { get; set; }

        [Required]
        public DateTime DataHoraCadastro { get; set; }

        [Required]
        public byte Status { get; set; }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; }

        [StringLength(1000)]
        public string Descricao { get; set; }
        
        [StringLength(100)]
        public string Componente { get; set; }

        public int? IdAplicacaoPai { get; set; }

        public byte? Permissao { get; set; }

        [Required]
        public bool Multiplo { get; set; }
    }
}
