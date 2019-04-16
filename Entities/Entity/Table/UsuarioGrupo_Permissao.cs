using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Entity.Table
{
    [Table("UsuarioGrupo_Permissao")]
    public class UsuarioGrupo_Permissao : BaseTable
    {
        [Key]
        public int IdUsuarioGrupoPermissao { get; set; }

        [Required]
        public int IdUsuarioGrupo { get; set; }

        [Required]
        public int IdAplicacao { get; set; }

        [Required]
        [StringLength(50)]
        public string Permissoes { get; set; }

        #region # Foreign Key
        [ForeignKey("IdUsuarioGrupo")]
        public virtual UsuarioGrupo UsuarioGrupo { get; set; }

        [ForeignKey("IdAplicacao")]
        public virtual Aplicacao Aplicacao { get; set; }
        #endregion
    }
}
