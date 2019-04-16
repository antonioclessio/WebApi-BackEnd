using System.Collections.Generic;

namespace Entities.Entity.UsuarioGrupo
{
    public class UsuarioGrupoForm : BaseFormModel
    {
        public int IdUsuarioGrupo { get; set; }
        public string Nome { get; set; }
        public byte Status { get; set; }
        public byte TipoPerfil { get; set; }

        public List<Table.Aplicacao_Permissao> Permissoes { get; set; } = new List<Table.Aplicacao_Permissao>();
    }
}
