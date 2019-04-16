using Entities.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Entity.Table
{
    [Table("UsuarioGrupo")]
    public class UsuarioGrupo : BaseTable
    {
        [Key]
        public int IdUsuarioGrupo { get; set; }

        [Required]
        public byte TipoPerfil { get; set; }

        [Required]
        [StringLength(50)]
        [LogAtividade]
        public string Nome { get; set; }
    }
}
