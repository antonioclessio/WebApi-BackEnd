using GrupoOrto.ERP.Entities.Entity.Paciente;
using GrupoOrto.ERP.Entities.Entity.Table;

namespace GrupoOrto.ERP.Business.Interfaces
{
    public interface IPacienteRepository : IRepository<Paciente, PacienteListResult, PacienteDetailResult, PacienteFilterQuery, PacienteForm>
    {
    }
}
