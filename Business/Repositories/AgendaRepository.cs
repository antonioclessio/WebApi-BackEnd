using GrupoOrto.ERP.Entities.Entity.Agenda;
using GrupoOrto.ERP.Entities.Entity.Table;
using GrupoOrto.ERP.Entities.Enum;
using GrupoOrto.ERP.Entities.Exceptions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

namespace GrupoOrto.ERP.Business.Repositories
{
    public class AgendaRepository : BaseRepository
    {
        protected override int IdAplicacao { get => (int)AplicacaoEnum.Agenda; }

        private readonly int INTERVALO_AGENDAMENTO;

        readonly short PERMISSAO_REGISTRAR_AGENDAMENTO;
        readonly short PERMISSAO_CANCELAR_HORARIO;
        readonly short PERMISSAO_EXCLUIR_HORARIO;
        readonly short PERMISSAO_REGISTRAR_CONFIRMACAO;
        readonly short PERMISSAO_LIBERAR_HORARIO;
        readonly short PERMISSAO_LEMBRETES;

        readonly string LOG_ATIVIDADE_ALTERACAO_FREQUENCIA;
        readonly string LOG_REGISTRO_LEMBRETE;
        readonly string LOG_ALTERACAO_AGENDAMENTO;
        readonly string LOG_EXCLUSAO_HORARIO;
        readonly string LOG_CANCELAR_HORARIO;
        readonly string LOG_LIBERAR_HORARIO;
        readonly string LOG_REGISTRO_CONFIRMACAO;

        public AgendaRepository()
        {
            INTERVALO_AGENDAMENTO = 15;
            PERMISSAO_REGISTRAR_AGENDAMENTO = 5;
            PERMISSAO_EXCLUIR_HORARIO = 12;
            PERMISSAO_CANCELAR_HORARIO = 15;
            PERMISSAO_REGISTRAR_CONFIRMACAO = 16;
            PERMISSAO_LIBERAR_HORARIO = 18;
            PERMISSAO_LEMBRETES = 14;

            LOG_ATIVIDADE_ALTERACAO_FREQUENCIA = "Alteração de frequência";
            LOG_REGISTRO_LEMBRETE = "Registro de lembrete";
            LOG_ALTERACAO_AGENDAMENTO = "Alteração de agendamento";
            LOG_EXCLUSAO_HORARIO = "Exclusão de horário da agenda do doutor";
            LOG_CANCELAR_HORARIO = "Cancelamento de horário da agenda do doutor";
            LOG_LIBERAR_HORARIO = "Liberação de horário da agenda do doutor";
            LOG_REGISTRO_CONFIRMACAO = "Registro de confirmação no agendamento";
        }

        #region # Agendamentos
        /// <summary>
        /// Retorna a agenda de um determinado doutor em uma determinada data.
        /// </summary>
        /// <param name="idDoutor">Código do doutor</param>
        /// <param name="dataReferencia">Data de Referência</param>
        /// <returns></returns>
        public List<GetAgendaDoutorResult> GetAgendaDoutor(int idDoutor, DateTime dataReferencia)
        {
            using (var context = new DatabaseContext())
            {
                var retorno = context.Database.SqlQuery<GetAgendaDoutorResult>($"SELECT * FROM dbo.FN_AgendaDoutor_Dia({idDoutor}, {dataReferencia.Year}, {dataReferencia.Month}, {dataReferencia.Day}, ${INTERVALO_AGENDAMENTO}, ${IdClinicaLogada})")
                    .OrderBy(a => a.DataHoraAgendamento)
                    .ToList();
                return retorno;
            }

        }

        /// <summary>
        /// Retorna um determinado agendamento
        /// </summary>
        /// <param name="id">Chave do agendamento a ser retornado</param>
        /// <returns></returns>
        public GetAgendamentoPaiResult GetAgendamentoPai(int id)
        {
            using (var context = new DatabaseContext())
            {
                var entity = (from agendamento in context.Agenda
                              join frequencia in context.Frequencia on agendamento.IdFrequencia equals frequencia.IdFrequencia
                              where agendamento.IdAgenda == id
                              select new GetAgendamentoPaiResult
                              {
                                  IdAgenda = agendamento.IdAgenda,
                                  CorFrequencia = frequencia.Cor,
                                  Frequencia = frequencia.Nome,
                                  DataHoraAlteracao = agendamento.DataHoraAlteracao.Value,
                                  DataHoraAgendamento = agendamento.DataHoraAgendamento,
                                  HoraChegada = agendamento.HoraChegada,
                                  HoraAtendimento = agendamento.HoraAtendimento,
                                  DescricaoOdonto = agendamento.DescricaoOdonto,
                                  Descricao = agendamento.Descricao,
                                  TipoTratamento = agendamento.TipoTratamento,
                                  IdPaciente = agendamento.IdPaciente
                              }).FirstOrDefault();

                return entity;
            }
        }

        /// <summary>
        /// Retorna os agendamentos acontecidos em uma determinada data
        /// </summary>
        /// <param name="dataReferencia">Data de referência</param>
        /// <returns></returns>
        public List<GetAgendamentosDiaResult> GetAgendamentosDia(DateTime? dataReferencia)
        {
            if (!dataReferencia.HasValue) { dataReferencia = CurrentDateTime; }
            using (var context = new DatabaseContext())
            {
                var dataHoraInicio = new DateTime(dataReferencia.Value.Year, dataReferencia.Value.Month, dataReferencia.Value.Day, 0, 0, 0);
                var dataHoraFim = new DateTime(dataReferencia.Value.Year, dataReferencia.Value.Month, dataReferencia.Value.Day, 23, 59, 0);

                var listEntity = (from agenda in context.Agenda
                                  join paciente in context.Paciente on agenda.IdPaciente equals paciente.IdPaciente
                                  join pessoa in context.Pessoa on paciente.IdPessoa equals pessoa.IdPessoa
                                  join doutor in context.Doutor on agenda.IdDoutor equals doutor.IdDoutor
                                  join ocorrencia in context.Ocorrencia on agenda.IdOcorrencia equals ocorrencia.IdOcorrencia
                                  join frequencia in context.Frequencia on agenda.IdFrequencia equals frequencia.IdFrequencia
                                  where agenda.DataHoraAgendamento >= dataHoraInicio && agenda.DataHoraAgendamento <= dataHoraFim
                                  select new GetAgendamentosDiaResult
                                  {
                                      DataHoraAgendamento = agenda.DataHoraAgendamento,
                                      NomePaciente = pessoa.Nome,
                                      NomeDoutor = doutor.Nome,
                                      Ocorrencia = ocorrencia,
                                      Frequencia = frequencia
                                  })
                                  .ToList();

                return listEntity;
            }
        }

        /// <summary>
        /// Retorna os agendamentos de um determinado doutor em um período específico
        /// </summary>
        /// <param name="idDoutor"></param>
        /// <param name="dataHoraInicio"></param>
        /// <param name="dataHoraFim"></param>
        /// <returns></returns>
        public List<RetornarPorPeriodoResult> RetornarPorPeriodo(int idDoutor, DateTime dataHoraInicio, DateTime dataHoraFim)
        {
            using (var context = new DatabaseContext())
            {
                var listEntity = (from agenda in context.Agenda
                                  join paciente in context.Paciente on agenda.IdPaciente equals paciente.IdPaciente
                                  join pessoa in context.Pessoa on paciente.IdPessoa equals pessoa.IdPessoa
                                  join doutor in context.Doutor on agenda.IdDoutor equals doutor.IdDoutor
                                  join ocorrencia in context.Ocorrencia on agenda.IdOcorrencia equals ocorrencia.IdOcorrencia
                                  join frequencia in context.Frequencia on agenda.IdFrequencia equals frequencia.IdFrequencia
                                  where agenda.DataHoraAgendamento >= dataHoraInicio && agenda.DataHoraAgendamento <= dataHoraFim
                                     && agenda.IdDoutor == idDoutor
                                  select new RetornarPorPeriodoResult
                                  {
                                      IdAgenda = agenda.IdAgenda,
                                      DataHoraAgendamento = agenda.DataHoraAgendamento,
                                      NomePaciente = pessoa.Nome,
                                      NomeDoutor = doutor.Nome,
                                      Frequencia = frequencia.Nome,
                                      CorFrequencia = frequencia.Cor
                                  }).ToList();

                return listEntity;
            }
        }

        public bool RegistrarChegada(int idAgenda)
        {
            using (var context = new DatabaseContext())
            {
                var entity = context.Agenda.FirstOrDefault(a => a.IdAgenda == idAgenda);
                if (entity == null) throw new BusinessException("Agendamento não encontrado");
                if (entity.DataHoraAgendamento.Date < DateTime.Now.Date) throw new BusinessException("Não é permitido alterar frequência de agendamentos passados");

                context.Entry(entity).State = EntityState.Modified;

                entity.DataHoraAlteracao = CurrentDateTime;
                entity.IdUsuarioAlteracao = IdUsuarioLogado;
                entity.IdFrequencia = (int)FrequenciaEnum.Compareceu;

                RegistrarLogAtividade(context, entity, 6, LOG_ATIVIDADE_ALTERACAO_FREQUENCIA);
                return context.SaveChanges() > 0;
            }
        }

        public bool RegistrarAtendimento(int idAgenda)
        {
            using (var context = new DatabaseContext())
            {
                var entity = context.Agenda.FirstOrDefault(a => a.IdAgenda == idAgenda);
                if (entity == null) throw new BusinessException("Agendamento não encontrado");
                if (entity.DataHoraAgendamento.Date < DateTime.Now.Date) throw new BusinessException("Não é permitido alterar frequência de agendamentos passados");

                context.Entry(entity).State = EntityState.Modified;

                entity.DataHoraAlteracao = CurrentDateTime;
                entity.IdUsuarioAlteracao = IdUsuarioLogado;
                entity.IdFrequencia = (int)FrequenciaEnum.EmAtendimento;

                RegistrarLogAtividade(context, entity, 17, LOG_ATIVIDADE_ALTERACAO_FREQUENCIA);
                return context.SaveChanges() > 0;
            }
        }

        public bool FinalizarAtendimento(int idAgenda)
        {
            using (var context = new DatabaseContext())
            {
                var entity = context.Agenda.FirstOrDefault(a => a.IdAgenda == idAgenda);
                if (entity == null) throw new BusinessException("Agendamento não encontrado");
                if (entity.DataHoraAgendamento.Date < DateTime.Now.Date) throw new BusinessException("Não é permitido alterar frequência de agendamentos passados");

                context.Entry(entity).State = EntityState.Modified;

                entity.DataHoraAlteracao = CurrentDateTime;
                entity.IdUsuarioAlteracao = IdUsuarioLogado;
                entity.IdFrequencia = (int)FrequenciaEnum.JaAtendido;

                RegistrarLogAtividade(context, entity, 8, LOG_ATIVIDADE_ALTERACAO_FREQUENCIA);
                return context.SaveChanges() > 0;
            }
        }

        public bool DesmarcarAtendimento(DesmarcarAgendamentoModel model)
        {
            using (var context = new DatabaseContext())
            {
                var entity = context.Agenda.FirstOrDefault(a => a.IdAgenda == model.IdAgenda);
                permiteAlterarAgendamento(entity);

                if (entity == null) throw new BusinessException("Agendamento não encontrado");
                if (entity.DataHoraAgendamento.Date < DateTime.Now.Date) throw new BusinessException("Não é permitido alterar frequência de agendamentos passados");

                context.Entry(entity).State = EntityState.Modified;

                entity.DataHoraAlteracao = CurrentDateTime;
                entity.IdUsuarioAlteracao = IdUsuarioLogado;
                entity.IdFrequencia = (int)FrequenciaEnum.Desmarcou;

                if (model.Lembrete != null)
                {
                    if (model.Lembrete.DataLembrete < entity.DataHoraAgendamento) throw new BusinessException("Data de lembrete inválida. A data é inferior ao do agendamento.");

                    model.Lembrete.IdAgenda = model.IdAgenda;
                    RegistrarLembrete(context, model.Lembrete);
                }
                else
                {
                    if (model.DataHoraAgendamento < entity.DataHoraAgendamento) throw new BusinessException("A nova data de agendamento é inferir à data do agendamento atual");

                    var satisfacaoRep = new SatisfacaoRepository();
                    satisfacaoRep.RegistrarSatisfacao(context, new Satisfacao
                    {
                        IdAgenda = model.IdAgenda,
                        Texto = model.Satisfacao,
                        IdPaciente = entity.IdPaciente
                    });

                    SalvarAgendamento(context, new AgendaForm
                    {
                        DataHoraAgendamento = model.DataHoraAgendamento,
                        Descricao = entity.Descricao,
                        DescricaoOdonto = entity.DescricaoOdonto,
                        IdAgendaPai = model.IdAgenda,
                        IdPaciente = entity.IdPaciente,
                        IdOcorrencia = entity.IdOcorrencia,
                        IdDoutor = entity.IdDoutor.Value,
                        TipoAgendamento = entity.TipoAgendamento
                    });
                }

                RegistrarLogAtividade(context, entity, 9, LOG_ATIVIDADE_ALTERACAO_FREQUENCIA);
                return context.SaveChanges() > 0;
            }
        }

        public bool RegistrarFalta(int idAgenda)
        {
            using (var context = new DatabaseContext())
            {
                var entity = context.Agenda.FirstOrDefault(a => a.IdAgenda == idAgenda);
                if (entity == null) throw new BusinessException("Agendamento não encontrado");
                if (entity.DataHoraAgendamento.Date < DateTime.Now.Date) throw new BusinessException("Não é permitido alterar frequência de agendamentos passados");

                context.Entry(entity).State = EntityState.Modified;

                entity.DataHoraAlteracao = CurrentDateTime;
                entity.IdUsuarioAlteracao = IdUsuarioLogado;
                entity.IdFrequencia = (int)FrequenciaEnum.Faltou;

                RegistrarLogAtividade(context, entity, 10, LOG_ATIVIDADE_ALTERACAO_FREQUENCIA);
                return context.SaveChanges() > 0;
            }
        }

        public bool ReagendarAtendimento(ReagendarAtendimentoModel model)
        {
            using (var context = new DatabaseContext())
            {
                var entity = context.Agenda.FirstOrDefault(a => a.IdAgenda == model.IdAgenda);
                if (entity == null) throw new BusinessException("Agendamento não encontrado");
                if (entity.DataHoraAgendamento.Date < DateTime.Now.Date) throw new BusinessException("Não é permitido alterar frequência de agendamentos passados");

                context.Entry(entity).State = EntityState.Modified;

                entity.DataHoraAlteracao = CurrentDateTime;
                entity.IdUsuarioAlteracao = IdUsuarioLogado;
                entity.IdFrequencia = (int)FrequenciaEnum.Reagendou;

                var satisfacaoRep = new SatisfacaoRepository();
                satisfacaoRep.RegistrarSatisfacao(context, new Satisfacao
                {
                    IdAgenda = model.IdAgenda,
                    Texto = model.Satisfacao,
                    IdPaciente = entity.IdPaciente
                });

                SalvarAgendamento(context, new AgendaForm
                {
                    DataHoraAgendamento = model.DataHoraAgendamento,
                    Descricao = entity.Descricao,
                    DescricaoOdonto = entity.DescricaoOdonto,
                    IdAgendaPai = model.IdAgenda,
                    IdPaciente = entity.IdPaciente,
                    IdOcorrencia = entity.IdOcorrencia,
                    IdDoutor = entity.IdDoutor.Value,
                    TipoAgendamento = entity.TipoAgendamento
                });

                RegistrarLogAtividade(context, entity, 11, LOG_ATIVIDADE_ALTERACAO_FREQUENCIA);
                return context.SaveChanges() > 0;
            }
        }

        public AtendimentoDoutorStaticResult GetEstatisticasAtendimento(int idDoutor, DateTime dataInicial, DateTime dataFinal)
        {
            using (var context = new DatabaseContext())
            {
                var _dataInicial = new DateTime(dataInicial.Year, dataInicial.Month, dataInicial.Day, 0, 0, 0);
                var _dataFinal = new DateTime(dataFinal.Year, dataFinal.Month, dataFinal.Day, 23, 59, 59);
                var frequenciaRepository = new FrequenciaRepository();
                var listFrequenciaEntity = frequenciaRepository.GetList(null);
                var listEntity = context.Agenda.Where(a => a.IdDoutor == idDoutor && a.DataHoraAgendamento >= _dataInicial && a.DataHoraAgendamento <= _dataFinal).ToList();

                var resultado = new AtendimentoDoutorStaticResult();

                var ocorrenciaAvaliacao = context.Ocorrencia.Where(a => a.TipoOcorrencia == (int)TipoOcorrenciaEnum.Avaliacao).Select(a => a.IdOcorrencia).ToList();

                resultado.TotalAvaliacao = listEntity.Where(a => ocorrenciaAvaliacao.Contains(a.IdOcorrencia.Value)).Count();
                resultado.TotalAindaNaoCompareceu = listEntity.Where(a => a.IdFrequencia == (int)FrequenciaEnum.AindaNaoCompareceu).Count();
                resultado.TotalAntecipouConsulta = listEntity.Where(a => a.IdFrequencia == (int)FrequenciaEnum.AntecipouConsulta).Count();
                resultado.TotalJaAtendido = listEntity.Where(a => a.IdFrequencia == (int)FrequenciaEnum.JaAtendido).Count();
                resultado.TotalDesmarcou = listEntity.Where(a => a.IdFrequencia == (int)FrequenciaEnum.Desmarcou).Count();
                resultado.TotalFaltou = listEntity.Where(a => a.IdFrequencia == (int)FrequenciaEnum.Faltou).Count();
                resultado.TotalReagendou = listEntity.Where(a => a.IdFrequencia == (int)FrequenciaEnum.Reagendou).Count();
                resultado.TotalCompareceu = listEntity.Where(a => a.IdFrequencia == (int)FrequenciaEnum.Compareceu).Count();
                resultado.TotalEmAtendimento = listEntity.Where(a => a.IdFrequencia == (int)FrequenciaEnum.EmAtendimento).Count();
                resultado.TotalFaltaAutomatica = listEntity.Where(a => a.IdFrequencia == (int)FrequenciaEnum.FaltaAutomatica).Count();

                resultado.CorAvaliacao = "";
                resultado.CorAindaNaoCompareceu = listFrequenciaEntity.Find(a => a.IdFrequencia == (int)FrequenciaEnum.AindaNaoCompareceu).Cor;
                resultado.CorAntecipouConsulta = listFrequenciaEntity.Find(a => a.IdFrequencia == (int)FrequenciaEnum.AntecipouConsulta).Cor;
                resultado.CorJaAtendido = listFrequenciaEntity.Find(a => a.IdFrequencia == (int)FrequenciaEnum.JaAtendido).Cor;
                resultado.CorDesmarcou = listFrequenciaEntity.Find(a => a.IdFrequencia == (int)FrequenciaEnum.Desmarcou).Cor;
                resultado.CorFaltou = listFrequenciaEntity.Find(a => a.IdFrequencia == (int)FrequenciaEnum.Faltou).Cor;
                resultado.CorReagendou = listFrequenciaEntity.Find(a => a.IdFrequencia == (int)FrequenciaEnum.Reagendou).Cor;
                resultado.CorCompareceu = listFrequenciaEntity.Find(a => a.IdFrequencia == (int)FrequenciaEnum.Compareceu).Cor;
                resultado.CorEmAtendimento = listFrequenciaEntity.Find(a => a.IdFrequencia == (int)FrequenciaEnum.EmAtendimento).Cor;
                resultado.CorFaltaAutomatica = listFrequenciaEntity.Find(a => a.IdFrequencia == (int)FrequenciaEnum.FaltaAutomatica).Cor;

                return resultado;
            }
        }

        public bool SalvarAgendamento(AgendaForm model)
        {
            if (model.IdAgenda > 0) return AlterarAgendamento(model);

            using (var context = new DatabaseContext())
            {
                SalvarAgendamento(context, model);
                return context.SaveChanges() > 0; ;
            }
        }

        public void SalvarAgendamento(DatabaseContext context, AgendaForm model)
        {
            if (model.IdPaciente == 0)
            {
                var pacienteRepository = new PacienteRepository();
                model.IdPaciente = pacienteRepository.SalvarParaAgendamento(context, model.Paciente);
            }

            var entity = new Agenda();
            entity.Status = (int)DefaultStatusEnum.Ativo;
            entity.DataHoraCadastro = CurrentDateTime;
            entity.DataHoraAlteracao = CurrentDateTime;
            entity.DataHoraAgendamento = model.DataHoraAgendamento.AddHours(FUSO_HORARIO);
            entity.IdUsuarioCadastro = GetLoggedUser().IdUsuario;
            entity.IdUsuarioAlteracao = GetLoggedUser().IdUsuario;
            entity.IdDoutor = model.IdDoutor;
            entity.IdClinica = IdClinicaLogada;
            entity.IdFrequencia = (int)FrequenciaEnum.AindaNaoCompareceu;
            entity.IdOcorrencia = model.IdOcorrencia;
            entity.ExibirAtraso = false;
            entity.TipoAgendamento = model.TipoAgendamento;
            entity.IdPaciente = model.IdPaciente;
            entity.IdAgendaPai = model.IdAgendaPai;
            entity.Descricao = model.Descricao;
            entity.DescricaoOdonto = model.DescricaoOdonto;
            entity.TipoTratamento = model.TipoAgendamento;

            if (model.DataHoraAgendamento < CurrentDateTime) throw new BusinessException("Não é permitido agendamentos retroativos");
            if (doutorAusente(context, entity)) throw new BusinessException("O doutor não estará disponível na data informada.");
            if (existeDuplicidadeHorario(context, entity)) throw new BusinessException("Este horário já contém um agendamento ativo.");
            if (!permiteEncaixe(context, entity)) throw new BusinessException("O limite de encaixes / agendamentos até o próximo atendimento já foi atingido.");
            if (limiteExpedienteAtingido(context, model)) throw new BusinessException("Agendamento fora do horário do expediente da unidade.");

            context.Set<Agenda>().Add(entity);

            RegistrarLogAtividade(context, entity, PERMISSAO_REGISTRAR_AGENDAMENTO);
        }

        private bool AlterarAgendamento(AgendaForm model)
        {
            using (var context = new DatabaseContext())
            {
                var currentEntity = context.Agenda.FirstOrDefault(a => a.IdAgenda == model.IdAgenda);
                if (currentEntity == null) throw new BusinessException("Agendamento não encontrado");

                currentEntity.DataHoraAlteracao = DateTime.Now;
                currentEntity.IdUsuarioAlteracao = GetLoggedUser().IdUsuario;
                currentEntity.Descricao = model.Descricao;
                currentEntity.DescricaoOdonto = model.DescricaoOdonto;

                context.Entry(currentEntity).State = EntityState.Modified;
                RegistrarLogAtividade(context, currentEntity, PERMISSAO_REGISTRAR_AGENDAMENTO, LOG_ALTERACAO_AGENDAMENTO);
                return context.SaveChanges() > 0;
            }
        }

        private bool existeDuplicidadeHorario(DatabaseContext context, Agenda entity)
        {
            /** Status que não permitem duplicidade de horário:
            * 1 - Ainda não compareceu
            * 2 - Antecipou Consulta
            * 7 - Compareceu
            * 8 - Em Atendimento
            * */

            List<int> listaStatusNaoPermitidos = new List<int>();
            listaStatusNaoPermitidos.Add(1);
            listaStatusNaoPermitidos.Add(2);
            listaStatusNaoPermitidos.Add(3);
            listaStatusNaoPermitidos.Add(7);
            listaStatusNaoPermitidos.Add(8);

            int idClinica = GetLoggedUser().IdClinica;

            var agendaEntity = context.Agenda.FirstOrDefault(a => a.DataHoraAgendamento == entity.DataHoraAgendamento
                                                          && listaStatusNaoPermitidos.Contains(a.IdFrequencia.Value)
                                                          && entity.IdDoutor == entity.IdDoutor
                                                          && entity.IdClinica == idClinica);
            return agendaEntity != null;
        }

        private bool permiteEncaixe(DatabaseContext context, Agenda entity)
        {
            /** Status que não permitem duplicidade de horário:
            * 1 - Ainda não compareceu
            * 2 - Antecipou Consulta
            * 7 - Compareceu
            * 8 - Em Atendimento
            * */

            List<int> listaStatusNaoPermitidos = new List<int>();
            listaStatusNaoPermitidos.Add(1);
            listaStatusNaoPermitidos.Add(2);
            listaStatusNaoPermitidos.Add(3);
            listaStatusNaoPermitidos.Add(7);
            listaStatusNaoPermitidos.Add(8);

            var dataInicio = new DateTime(entity.DataHoraAgendamento.Year, entity.DataHoraAgendamento.Month, entity.DataHoraAgendamento.Day, 7, 0, 0);
            var dataFim = new DateTime(entity.DataHoraAgendamento.Year, entity.DataHoraAgendamento.Month, entity.DataHoraAgendamento.Day, 23, 59, 0);

            var listaAgendamentos = context.Agenda.Where(a => a.IdClinica == entity.IdClinica && a.IdDoutor == entity.IdDoutor
                                                           && a.DataHoraAgendamento >= dataInicio && a.DataHoraAgendamento <= dataFim
                                                           && listaStatusNaoPermitidos.Contains(a.IdFrequencia.Value)).ToList();

            int totalAgendamentos = 0;
            if (entity.DataHoraAgendamento.Minute >= 0 && entity.DataHoraAgendamento.Minute < 15)
            {
                totalAgendamentos = listaAgendamentos.Where(a =>
                    a.DataHoraAgendamento.Hour == entity.DataHoraAgendamento.Hour &&
                    a.DataHoraAgendamento.Minute >= 0 &&
                    a.DataHoraAgendamento.Minute < 15
                ).ToList().Count();
            }
            else if (entity.DataHoraAgendamento.Minute >= 15 && entity.DataHoraAgendamento.Minute < 30)
            {
                totalAgendamentos = listaAgendamentos.Where(a =>
                    a.DataHoraAgendamento.Hour == entity.DataHoraAgendamento.Hour &&
                    a.DataHoraAgendamento.Minute >= 15 &&
                    a.DataHoraAgendamento.Minute < 30
                ).ToList().Count();
            }
            else if (entity.DataHoraAgendamento.Minute >= 30 && entity.DataHoraAgendamento.Minute < 45)
            {
                totalAgendamentos = listaAgendamentos.Where(a =>
                    a.DataHoraAgendamento.Hour == entity.DataHoraAgendamento.Hour &&
                    a.DataHoraAgendamento.Minute >= 30 &&
                    a.DataHoraAgendamento.Minute < 45
                ).ToList().Count();
            }
            else if (entity.DataHoraAgendamento.Minute >= 45 && entity.DataHoraAgendamento.Minute <= 59)
            {
                totalAgendamentos = listaAgendamentos.Where(a =>
                    a.DataHoraAgendamento.Hour == entity.DataHoraAgendamento.Hour &&
                    a.DataHoraAgendamento.Minute >= 45 &&
                    a.DataHoraAgendamento.Minute <= 59
                ).ToList().Count();
            }
            // Permitir somente um encaixe.
            // O encaixe extra é no caso de ser uma frequencia de "desmarcou" por exemplo.
            return totalAgendamentos < 2;
        }

        private bool limiteExpedienteAtingido(DatabaseContext context, AgendaForm entity)
        {
            var clinicaRepository = new ClinicaRepository();
            var clinicaEntity = clinicaRepository.GetByKeyFull(IdClinicaLogada);

            TimeSpan? horaInicio = null;
            TimeSpan? horaFim = null;
            TimeSpan? horaAlmocoInicio = null;
            TimeSpan? horaAlmocoFim = null;

            switch (entity.DataHoraAgendamento.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    if (clinicaEntity.ExpedienteDomingoInicio.HasValue) horaInicio = clinicaEntity.ExpedienteDomingoInicio.Value;
                    if (clinicaEntity.ExpedienteDomingoFim.HasValue) horaFim = clinicaEntity.ExpedienteDomingoFim.Value;
                    if (clinicaEntity.AlmocoDomingoInicio.HasValue) horaAlmocoInicio = clinicaEntity.AlmocoDomingoInicio.Value;
                    if (clinicaEntity.AlmocoDomingoFim.HasValue) horaAlmocoFim = clinicaEntity.AlmocoDomingoFim.Value;
                    break;
                case DayOfWeek.Monday:
                    if (clinicaEntity.ExpedienteSegundaInicio.HasValue) horaInicio = clinicaEntity.ExpedienteSegundaInicio.Value;
                    if (clinicaEntity.ExpedienteSegundaFim.HasValue) horaFim = clinicaEntity.ExpedienteSegundaFim.Value;
                    if (clinicaEntity.AlmocoSegundaInicio.HasValue) horaAlmocoInicio = clinicaEntity.AlmocoSegundaInicio.Value;
                    if (clinicaEntity.AlmocoSegundaFim.HasValue) horaAlmocoFim = clinicaEntity.AlmocoSegundaFim.Value;
                    break;
                case DayOfWeek.Tuesday:
                    if (clinicaEntity.ExpedienteTercaInicio.HasValue) horaInicio = clinicaEntity.ExpedienteTercaInicio.Value;
                    if (clinicaEntity.ExpedienteTercaFim.HasValue) horaFim = clinicaEntity.ExpedienteTercaFim.Value;
                    if (clinicaEntity.AlmocoTercaInicio.HasValue) horaAlmocoInicio = clinicaEntity.AlmocoTercaInicio.Value;
                    if (clinicaEntity.AlmocoTercaFim.HasValue) horaAlmocoFim = clinicaEntity.AlmocoTercaFim.Value;
                    break;
                case DayOfWeek.Wednesday:
                    if (clinicaEntity.ExpedienteQuartaInicio.HasValue) horaInicio = clinicaEntity.ExpedienteQuartaInicio.Value;
                    if (clinicaEntity.ExpedienteQuartaFim.HasValue) horaFim = clinicaEntity.ExpedienteQuartaFim.Value;
                    if (clinicaEntity.AlmocoQuartaInicio.HasValue) horaAlmocoInicio = clinicaEntity.AlmocoQuartaInicio.Value;
                    if (clinicaEntity.AlmocoQuartaFim.HasValue) horaAlmocoFim = clinicaEntity.AlmocoQuartaFim.Value;
                    break;
                case DayOfWeek.Thursday:
                    if (clinicaEntity.ExpedienteQuintaInicio.HasValue) horaInicio = clinicaEntity.ExpedienteQuintaInicio.Value;
                    if (clinicaEntity.ExpedienteQuintaFim.HasValue) horaFim = clinicaEntity.ExpedienteQuintaFim.Value;
                    if (clinicaEntity.AlmocoQuintaInicio.HasValue) horaAlmocoInicio = clinicaEntity.AlmocoQuintaInicio.Value;
                    if (clinicaEntity.AlmocoQuintaFim.HasValue) horaAlmocoFim = clinicaEntity.AlmocoQuintaFim.Value;
                    break;
                case DayOfWeek.Friday:
                    if (clinicaEntity.ExpedienteSextaInicio.HasValue) horaInicio = clinicaEntity.ExpedienteSextaInicio.Value;
                    if (clinicaEntity.ExpedienteSextaFim.HasValue) horaFim = clinicaEntity.ExpedienteSextaFim.Value;
                    if (clinicaEntity.AlmocoSextaInicio.HasValue) horaAlmocoInicio = clinicaEntity.AlmocoSextaInicio.Value;
                    if (clinicaEntity.AlmocoSextaFim.HasValue) horaAlmocoFim = clinicaEntity.AlmocoSextaFim.Value;
                    break;
                case DayOfWeek.Saturday:
                    if (clinicaEntity.ExpedienteSabadoInicio.HasValue) horaInicio = clinicaEntity.ExpedienteSabadoInicio.Value;
                    if (clinicaEntity.ExpedienteSabadoFim.HasValue) horaFim = clinicaEntity.ExpedienteSabadoFim.Value;
                    if (clinicaEntity.AlmocoSabadoInicio.HasValue) horaAlmocoInicio = clinicaEntity.AlmocoSabadoInicio.Value;
                    if (clinicaEntity.AlmocoSabadoFim.HasValue) horaAlmocoFim = clinicaEntity.AlmocoSabadoFim.Value;
                    break;
                default:
                    return true;
            }

            TimeSpan intervalo = new TimeSpan(0, INTERVALO_AGENDAMENTO, 0);

            // Se não estiver dentro do intervalo do primeiro período do expediente subtraindo o limite de 15 minutos antes do último agendamento permitido...
            bool periodoManhaValido = (entity.DataHoraAgendamento.AddHours(FUSO_HORARIO).TimeOfDay >= horaInicio && entity.DataHoraAgendamento.AddHours(FUSO_HORARIO).TimeOfDay <= horaAlmocoInicio.Value.Subtract(intervalo));

            // Se não estiver dentro do intervalo do segundo período do expediente subtraindo o limite de 15 minutos antes do último agendamento permitido...
            bool periodoTardeValido = (entity.DataHoraAgendamento.AddHours(FUSO_HORARIO).TimeOfDay >= horaAlmocoFim && entity.DataHoraAgendamento.AddHours(FUSO_HORARIO).TimeOfDay <= horaFim.Value.Subtract(intervalo));

            // Se nenhum dos períodos são válidos, então retorna true, pois o método valida se o horário de agendamento está fora dos limites permitidos.
            return periodoManhaValido == false && periodoTardeValido == false;
        }

        private bool doutorAusente(DatabaseContext context, Agenda entity)
        {
            return context.Ausencia.Where(a => a.IdDoutor == entity.IdDoutor && a.DataHoraInicio.Value == entity.DataHoraAgendamento).ToList().Count > 0;
        }
        #endregion

        #region # Lembretes
        /// <summary>
        /// Retorna os lembretes de agendamentos criados pelo usuário logado em uma determinada data.
        /// </summary>
        /// <param name="dataReferencia">Data de referência</param>
        /// <returns>Lista de lembretes</returns>
        public List<GetLembretesResult> GetLembretes(DateTime? dataReferencia = null)
        {
            using (var context = new DatabaseContext())
            {
                if (dataReferencia.HasValue == false) dataReferencia = CurrentDateTime;

                return (from lembrete in context.Agenda_Lembrete
                        join agenda in context.Agenda on lembrete.IdAgenda equals agenda.IdAgenda
                        join paciente in context.Paciente on agenda.IdPaciente equals paciente.IdPaciente
                        join pessoa in context.Pessoa on paciente.IdPessoa equals pessoa.IdPessoa
                        where lembrete.IdUsuarioCadastro == IdUsuarioLogado
                           && lembrete.DataLembrete == dataReferencia.Value
                           && lembrete.Status == (int)DefaultStatusEnum.Ativo
                        select new GetLembretesResult
                        {
                            Nome = pessoa.Nome,
                            Contatado = false
                        })
                        .ToList();
            }
        }

        /// <summary>
        /// Salva um determinado lembrete
        /// </summary>
        /// <param name="entity">Entidade contendo os dados do lembretes a ser salvo</param>
        /// <returns></returns>
        public void RegistrarLembrete(DatabaseContext context, Agenda_Lembrete entity)
        {
            entity.IdUsuarioCadastro = GetLoggedUser().IdUsuario;
            entity.IdUsuarioAlteracao = GetLoggedUser().IdUsuario;
            entity.DataHoraCadastro = CurrentDateTime;
            entity.DataHoraAlteracao = CurrentDateTime;
            entity.Status = (int)DefaultStatusEnum.Ativo;

            context.Set<Agenda_Lembrete>().Add(entity);
            RegistrarLogAtividade(entity, PERMISSAO_LEMBRETES, LOG_REGISTRO_LEMBRETE);
        }
        #endregion

        #region # Agenda Doutor
        /// <summary>
        /// Utilizado para excluir um horário na agenda do doutor.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Doutor_ExcecaoAgenda RegistrarExcecaoAgendaDoutor(Doutor_ExcecaoAgenda entity)
        {
            using (var context = new DatabaseContext())
            {
                if (context.Doutor_ExcecaoAgenda.FirstOrDefault(a => a.IdDoutor == entity.IdDoutor && a.DataHoraExcecao == entity.DataHoraExcecao) != null)
                    throw new BusinessException("Já existe uma exceção nesta data na agenda deste doutor");

                entity.DataHoraAlteracao = CurrentDateTime;
                entity.DataHoraCadastro = CurrentDateTime;
                entity.IdUsuarioCadastro = IdUsuarioLogado;
                entity.IdUsuarioAlteracao = IdUsuarioLogado;
                entity.Status = (int)DefaultStatusEnum.Ativo;

                context.Set<Doutor_ExcecaoAgenda>().Add(entity);

                RegistrarLogAtividade(context, entity, PERMISSAO_EXCLUIR_HORARIO, LOG_EXCLUSAO_HORARIO);
                context.SaveChanges();

                return entity;
            }
        }

        /// <summary>
        /// Cancela um ou mais horários da agenda de um doutor. O cancelamento registra ausências por intervalo.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool CancelarHorarioAgendaDoutor(CancelarHorarioModel model)
        {
            using (var context = new DatabaseContext())
            {
                if (model.IntervaloInicio != null) model.IntervaloInicio = model.IntervaloInicio.Value.AddHours(FUSO_HORARIO);
                if (model.IntervaloFim != null) model.IntervaloFim = model.IntervaloFim.Value.AddHours(FUSO_HORARIO);

                if (model.DataHoraInicio.HasValue && context.Ausencia.FirstOrDefault(a => a.IdDoutor == model.IdDoutor && a.DataHoraInicio.Value == model.DataHoraInicio.Value) != null)
                    throw new BusinessException("O período da ausência coincide com outra já cadastrada. Verifique.");

                if (model.DataHoraInicio.HasValue && model.DataHoraInicio.Value.Date <= DateTime.Now.Date ||
                    model.IntervaloInicio.HasValue && model.IntervaloInicio.Value.Date <= DateTime.Now.Date)
                    throw new BusinessException("Não é permitido o cadastro de ausências retroativas.");

                if (model.DataHoraFim.HasValue && model.DataHoraFim.Value < model.DataHoraInicio) throw new BusinessException("A data de retorno não pode ser anterior à data de saída.");

                if (model.TipoAusencia != (int)TipoAusenciaEnum.CancelamentoAgenda)
                {
                    var ultimaAusenciaRegistrada = context.Ausencia.Where(a => a.IdDoutor == model.IdDoutor).OrderByDescending(a => a.IdAusencia).Take(1).FirstOrDefault();
                    if (ultimaAusenciaRegistrada != null && !ultimaAusenciaRegistrada.DataHoraFim.HasValue) throw new BusinessException("Existe uma ausência sem retorno registrado. Verifique.");
                }

                do
                {
                    var entity = new Ausencia();
                    entity.DataHoraCadastro = CurrentDateTime;
                    entity.DataHoraAlteracao = CurrentDateTime;
                    entity.IdUsuarioCadastro = IdUsuarioLogado;
                    entity.IdUsuarioAlteracao = IdUsuarioLogado;
                    entity.Status = (int)DefaultStatusEnum.Ativo;
                    entity.IdDoutor = model.IdDoutor;
                    entity.TipoAusencia = model.TipoAusencia;
                    entity.DataHoraInicio = model.IntervaloInicio;

                    model.IntervaloInicio = model.IntervaloInicio.Value.AddMinutes(INTERVALO_AGENDAMENTO);
                    entity.DataHoraFim = model.IntervaloInicio;
                    entity.Observacao = model.Observacao;

                    context.Set<Ausencia>().Add(entity);
                    RegistrarLogAtividade(context, entity, PERMISSAO_CANCELAR_HORARIO, LOG_CANCELAR_HORARIO);
                } while (model.IntervaloInicio <= model.IntervaloFim);

                return context.SaveChanges() > 0;
            }

        }

        public bool LiberarHorarioAgendaDoutor(int key)
        {
            using (var context = new DatabaseContext())
            {
                var entity = context.Ausencia.FirstOrDefault(a => a.IdAusencia == key);
                if (entity == null) throw new BusinessException("Ausência não encontrada");

                context.Entry(entity).State = EntityState.Deleted;
                RegistrarLogAtividade(context, entity, PERMISSAO_LIBERAR_HORARIO, LOG_LIBERAR_HORARIO);

                return context.SaveChanges() > 0;
            }
        }

        /// <summary>
        /// Registra falta automática em todos os agendamentos cujos clientes faltaram e não foi devidamente registrado
        /// </summary>
        /// <param name="intervaloHoras">As faltas que serão localizadas estarão dentro do intervalo configurado em horas</param>
        public void MarcarFaltaAutomatica(int intervaloHoras = 24)
        {
            using (var context = new DatabaseContext())
            {
                var dataHoraInicio = DateTime.Now.AddHours(intervaloHoras * -1);
                var listaAgendamentosAtrasados = context.Agenda.Where(a => a.IdFrequencia == 1 && a.DataHoraAgendamento >= dataHoraInicio && a.DataHoraAgendamento < DateTime.Now).ToList();

                foreach (var agendamento in listaAgendamentosAtrasados)
                {

                }
            }
        }

        /// <summary>
        /// Verifica se é possível realizar a alteração da frequencia do agendamento. A verificação é com base na regra de negócio e não permissão de acesso.
        /// </summary>
        /// <param name="entity"></param>
        private void permiteAlterarAgendamento(Agenda entity)
        {
            if (entity.IdFrequencia == (int)FrequenciaEnum.JaAtendido || entity.IdFrequencia == (int)FrequenciaEnum.Faltou || entity.IdFrequencia == (int)FrequenciaEnum.FaltaAutomatica)
                throw new BusinessException("Este atendimento já foi finalizado");

            if (entity.IdFrequencia == (int)FrequenciaEnum.Desmarcou)
                throw new BusinessException("Este atendimento foi desmarcado");

            if (entity.IdFrequencia == (int)FrequenciaEnum.Reagendou)
                throw new BusinessException("Este atendimento foi reagendado");
        }

        public List<RelatorioTotalAgendamentosDiarioDoutorResult> RelatorioTotalAgendamentosDiarioDoutor(RelatorioTotalAgendamentosDiarioDoutorModel model)
        {
            using (var context = new DatabaseContext())
            {
                context.Database.CommandTimeout = 999999999;
                if (model.IdClinica.HasValue == false) model.IdClinica = IdClinicaLogada;
                if (model.IdDoutor.HasValue == false) model.IdDoutor = 0;

                var sqlQuery = "SP_Relatorio_TotalAgendamentosDiarioDoutor @Ano, @Mes, @IdClinica, @IdDoutor, @IdFrequencia";

                return context.Database.SqlQuery<RelatorioTotalAgendamentosDiarioDoutorResult>(sqlQuery
                                                                                              , new SqlParameter("Ano", model.Ano)
                                                                                              , new SqlParameter("Mes", model.Mes)
                                                                                              , new SqlParameter("IdClinica", model.IdClinica.Value)
                                                                                              , new SqlParameter("IdDoutor", model.IdDoutor)
                                                                                              , new SqlParameter("IdFrequencia", model.IdFrequencia)
                                                                                              ).ToList();
            }
        }
        #endregion

        #region # Confirmação
        public bool RegistrarConfirmacao(Agenda_Confirmacao entity)
        {
            using (var context = new DatabaseContext())
            {
                entity.DataHoraCadastro = DateTime.Now;
                entity.DataHoraAlteracao = DateTime.Now;
                entity.Status = (int)DefaultStatusEnum.Ativo;
                entity.IdUsuarioCadastro = GetLoggedUser().IdUsuario;
                entity.IdUsuarioAlteracao = GetLoggedUser().IdUsuario;

                context.Set<Agenda_Confirmacao>().Add(entity);

                RegistrarLogAtividade(entity, PERMISSAO_REGISTRAR_CONFIRMACAO, LOG_REGISTRO_CONFIRMACAO);
                return context.SaveChanges() > 0; ;
            }
        }

        public List<GetConfirmacaoResult> GetConfirmacoes(int idAgenda)
        {
            using (var context = new DatabaseContext())
            {
                UsuarioRepository usuarioRep = new UsuarioRepository();

                var listEntity = (from agendaConfirmacao in context.Agenda_Confirmacao
                                  join confirmacao in context.Confirmacao on agendaConfirmacao.IdConfirmacao equals confirmacao.IdConfirmacao
                                  join usuario in context.Usuario on agendaConfirmacao.IdUsuarioCadastro equals usuario.IdUsuario
                                  where agendaConfirmacao.Status == (int)DefaultStatusEnum.Ativo && agendaConfirmacao.IdAgenda == idAgenda
                                  select new GetConfirmacaoResult
                                  {
                                      IdAgenda = agendaConfirmacao.IdAgenda,
                                      DataHoraCadastro = agendaConfirmacao.DataHoraCadastro,
                                      IdConfirmacao = agendaConfirmacao.IdConfirmacao,
                                      Recado = agendaConfirmacao.Recado,
                                      Sigla = confirmacao.Sigla,
                                      Confirmacao = confirmacao.Nome,
                                      IdUsuarioCadastro = usuario.IdUsuario,
                                      NomeUsuarioCadastro = usuario.Nome
                                  })
                                  .OrderByDescending(a => a.DataHoraCadastro)
                                  .ToList();

                return listEntity;
            }
        }
        #endregion
    }
}
