using GrupoOrto.ERP.Entities.Entity.Table;
using GrupoOrto.ERP.Entities.Enum;
using System.Collections.Generic;
using System.Linq;

namespace GrupoOrto.ERP.Business.Repositories
{
    public class SatisfacaoRepository : BaseRepository
    {
        protected override int IdAplicacao => (int)AplicacaoEnum.Agenda;

        /// <summary>
        /// Retorna uma lista de satisfações de um determinado cliente para um agendamento específico.
        /// </summary>
        /// <param name="idPaciente"></param>
        /// <param name="idAgenda"></param>
        /// <returns></returns>
        public List<Satisfacao> GetSatisfacaoPorAgendamento(int idPaciente, int? idAgenda)
        {
            using (var context = new DatabaseContext())
            {
                var result = (from paciente in context.Paciente
                              join satisfacao in context.Satisfacao on paciente.IdPaciente equals satisfacao.IdPaciente
                              where paciente.IdPaciente == idPaciente
                                 && paciente.IdClinica == IdClinicaLogada
                                 && (idAgenda.HasValue == false || satisfacao.IdAgenda == idAgenda.Value)
                              select satisfacao).ToList();

                return result;
            }
        }

        public void RegistrarSatisfacao(DatabaseContext context, Satisfacao model)
        {
            model.DataHoraCadastro = CurrentDateTime;
            model.DataHoraAlteracao = CurrentDateTime;
            model.IdUsuarioCadastro = IdUsuarioLogado;
            model.IdUsuarioAlteracao = IdUsuarioLogado;
            model.Status = (int)DefaultStatusEnum.Ativo;
            model.Usuario = GetLoggedUser().Nome;

            context.Set<Satisfacao>().Add(model);
        }

        public bool RegistrarSatisfacao(Satisfacao model)
        {
            using (var context = new DatabaseContext())
            {
                RegistrarSatisfacao(context, model);
                return context.SaveChanges() > 0;
            }
        }
    }
}
