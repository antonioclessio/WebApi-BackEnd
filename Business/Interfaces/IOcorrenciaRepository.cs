using GrupoOrto.ERP.Entities.Entity.Ocorrencia;
using GrupoOrto.ERP.Entities.Entity.Table;

namespace GrupoOrto.ERP.Business.Interfaces
{
    public interface IOcorrenciaRepository : IRepository<Ocorrencia, OcorrenciaListResult, OcorrenciaDetailResult, OcorrenciaFilterQuery, OcorrenciaForm>
    {
    }
}
