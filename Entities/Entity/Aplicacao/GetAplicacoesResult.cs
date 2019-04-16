using System.Collections.Generic;

namespace Entities.Entity.Aplicacao
{
    public class GetAplicacoesResult
    {
        public int IdAplicacao { get; set; }
        public string Nome { get; set; }
        public List<Table.Aplicacao_Permissao> Permissoes { get; set; }
    }
}
