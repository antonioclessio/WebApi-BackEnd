using Entities.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Entity.Table
{
    [Table("Funcionario")]
    public class Funcionario : BaseTable
    {
        [Key]
        public int IdFuncionario { get; set; }

        public int? IdUsuario { get; set; }

        [Required]
        public int IdClinica { get; set; }

        [Required]
        [StringLength(100)]
        [LogAtividade]
        public string Nome { get; set; }

        public int? IdLocalizacaoGeografica { get; set; }

        [StringLength(10)]
        public string NumeroEndereco { get; set; }

        [StringLength(200)]
        public string Complemento { get; set; }

        //[Required]
        public byte? EstadoCivil { get; set; }

        //[Required]
        [StringLength(11)]
        public string CPF { get; set; }

        //[Required]
        [StringLength(10)]
        public string RG { get; set; }

        //[Required]
        public DateTime? DataNascimento { get; set; }

        //[Required]
        public DateTime? DataAdmissao { get; set; }

        public DateTime? DataDesligamento { get; set; }

        [StringLength(10)]
        public string CarteiraProfissionalNro { get; set; }

        [StringLength(10)]
        public string CarteiraProfissionalSerie { get; set; }

        [StringLength(500)]
        public string Observacao { get; set; }

        public int? IdFuncionarioLegado { get; set; }
    }
}
