using System.Collections.Generic;

namespace Entities.Entity.Security
{
    public class UsuarioSessaoModel
    {
        public int IdClinica { get; set; }
        public int IdUsuario { get; set; }
        public int IdUsuarioGrupo { get; set; }
        public string Nome { get; set; }
        public List<PermissaoAplicacaoIdentityModel> Permissoes { get; set; }
    }
}
