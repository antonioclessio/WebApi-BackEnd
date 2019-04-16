using System.Collections.Generic;

namespace Entities.Entity.Security
{
    public class PermissaoAplicacaoIdentityModel
    {
        public int IdAplicacao { get; set; }
        public List<int> Permissoes { get; } = new List<int>();
    }
}
