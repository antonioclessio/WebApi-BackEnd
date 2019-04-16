using GrupoOrto.ERP.Entities.Entity.Profissao;
using GrupoOrto.ERP.Entities.Entity.Table;

namespace GrupoOrto.ERP.Business.Interfaces
{
    public interface IProfissaoRepository : IRepository<Profissao, ProfissaoListResult, ProfissaoDetailResult, ProfissaoFilterQuery, ProfissaoForm>
    {
    }
}
