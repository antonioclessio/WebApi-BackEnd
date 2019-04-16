using Entities.Interfaces.V1;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Entity.Table
{
    [Table("Usuario_LogAtividade")]
    public class Usuario_LogAtividade : ITable
    {
        [Key]
        public int IdUsuarioLogAtividade { get; set; }

        [Required]
        public DateTime DataHoraCadastro { get; set; }

        [Required]
        public int IdUsuario { get; set; }

        [Required]
        public int IdClinica { get; set; }

        [Required]
        public int IdAplicacao { get; set; }

        [Required]
        public short Acao { get; set; }

        [Required]
        public int IdRegistroAfetado { get; set; }

        [Required]
        public string DescricaoRegistroAfetado { get; set; }

        [Required]
        public string Tabela { get; set; }
    }
}
