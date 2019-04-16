using GrupoOrto.ERP.Entities.Entity.Cockpit;
using GrupoOrto.ERP.Entities.Enum;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrupoOrto.ERP.Business.Repositories
{
    public class CockpitRepository : BaseRepository
    {
        protected override int IdAplicacao => (int)AplicacaoEnum.Neutro;

        private readonly int TIMEOUT;

        public CockpitRepository()
        {
            TIMEOUT = 600000;
        }

        #region # Public Methods
        public List<CockpitResultadoDiarioResult> ResultadoDiario(int anoReferencia, int mesReferencia, bool consolidado, int? idUsuario, int? idClinica)
        {
            using (var context = new DatabaseContext())
            {
                if (idUsuario.HasValue == false) idUsuario = IdUsuarioLogado;
                if (idClinica.HasValue == false) idClinica = IdClinicaLogada;

                var sqlQuery = "SP_Cockpit_ResultadoDiario_Mes @IdClinica, @AnoReferencia, @MesReferencia, @Consolidado, @Idusuario";

                return context.Database.SqlQuery<CockpitResultadoDiarioResult>(sqlQuery
                                                                      , new SqlParameter("IdClinica", idClinica)
                                                                      , new SqlParameter("AnoReferencia", anoReferencia)
                                                                      , new SqlParameter("MesReferencia", mesReferencia)
                                                                      , new SqlParameter("Consolidado", consolidado ? 1 : 0)
                                                                      , new SqlParameter("Idusuario", idUsuario)
                                                                      ).ToList();
            }
        }

        public List<CockpitResultadoDiarioSalaResult> ResultadoDiario_Salas(int anoReferencia, int mesReferencia, int idClinica)
        {
            using (var context = new DatabaseContext())
            {
                // Etapa 1: Recuperando todas as salas ativas.
                List<CockpitResultadoDiarioSalaResult> resultado = new List<CockpitResultadoDiarioSalaResult>();
                var salasEntity = context.SalaAtendimento.Where(a => a.Status == (int)DefaultStatusEnum.Ativo && a.IdClinica == idClinica).ToList();

                var reservas = (from grade in context.GradeSalas
                                join mes in context.GradeSalas_Mes on grade.IdGradeSalas equals mes.IdGradeSalas
                                join joinHorario in context.GradeSalas_Mes_Horario on mes.IdGradeSalasMes equals joinHorario.IdGradeSalasMes into listReservas
                                from horarios in listReservas.DefaultIfEmpty()
                                join joindoutor in context.Doutor on horarios.IdDoutor equals joindoutor.IdDoutor into listaDoutores
                                from doutor in listaDoutores.DefaultIfEmpty()
                                where grade.AnoReferencia == anoReferencia && grade.MesReferencia == mesReferencia
                                select new
                                {
                                    IdSalaAtendimento = mes.IdSalaAtendimento,
                                    DiaReferencia = mes.DiaReferencia,
                                    Periodo = horarios != null ? horarios.Periodo : 0,
                                    NomeDoutor = doutor != null ? doutor.Nome : "Vazio",
                                    SiglaDoutor = doutor != null ? doutor.Sigla : null,
                                    TratamentoClinico = false
                                }).ToList();

                // Etapa 2: Para cada sala, começa as verificações das reservas...
                foreach (var sala in salasEntity)
                {
                    List<CockpitResultadoDiarioSalaDiaResult> dias = new List<CockpitResultadoDiarioSalaDiaResult>();

                    DateTime dataInicial = new DateTime(anoReferencia, mesReferencia, 1);
                    DateTime dataFinal = new DateTime(anoReferencia, mesReferencia + 1, 1).AddDays(-1);

                    for (int i = dataInicial.Day; i <= dataFinal.Day; i++)
                    {
                        string descricaoDia = new DateTime(anoReferencia, mesReferencia, i).DayOfWeek.ToString();
                        string diaSemana = traduzirSemana(descricaoDia);
                        string sigla = siglaDiaSemana(descricaoDia);

                        var periodosReservado = reservas.Where(a => a.DiaReferencia == i && a.IdSalaAtendimento == sala.IdSalaAtendimento).ToList();
                        var responsavelManha = string.Join("/", periodosReservado.Where(a => a.Periodo == 1).Select(a => a.SiglaDoutor));
                        var responsavelTarde = string.Join("/", periodosReservado.Where(a => a.Periodo == 2).Select(a => a.SiglaDoutor));
                        var responsavelNoite = string.Join("/", periodosReservado.Where(a => a.Periodo == 3).Select(a => a.SiglaDoutor));
                        var ocupacaoSala = calcularOcupacaoSala(responsavelManha, responsavelTarde, responsavelNoite);
                        dias.Add(new CockpitResultadoDiarioSalaDiaResult
                        {
                            Dia = i,
                            DiaSemana = diaSemana,
                            SiglaDiaSemana = sigla,
                            DoutorManha = responsavelManha == "" ? "Vazio" : responsavelManha,
                            DoutorTarde = responsavelTarde == "" ? "Vazio" : responsavelTarde,
                            DoutorNoite = responsavelNoite == "" ? "Vazio" : responsavelNoite,
                            Ocupacao = ocupacaoSala
                        });
                    }

                    decimal mediaTotalSala = dias.Average(a => a.Ocupacao);

                    resultado.Add(new CockpitResultadoDiarioSalaResult
                    {
                        OcupacaoSala = decimal.Round(mediaTotalSala, 2),
                        Sala = sala.Nome,
                        Dias = dias
                    });
                }

                return resultado;
            }
        }

        public List<CockpitPagina28Result> ResultadoDiario_Pagina28(int? ano, int? mes)
        {
            using (var context = new DatabaseContext())
            {
                context.Database.CommandTimeout = TIMEOUT;

                var sqlQuery = "SP_Cockpit_Pagina28 @Ano, @Mes";

                return context.Database.SqlQuery<CockpitPagina28Result>(sqlQuery, new SqlParameter("Ano", ano.HasValue ? ano.Value : SqlInt32.Null)
                                                                      , new SqlParameter("Mes", mes.HasValue ? mes.Value : SqlInt32.Null)
                                                                      ).ToList();
            }
        }

        public List<CockpitInadimplenciaResult> ResultadoDiario_Inadimplencia(int? ano, int? mes)
        {
            using (var context = new DatabaseContext())
            {
                context.Database.CommandTimeout = TIMEOUT;

                var sqlQuery = "SP_Cockpit_Inadimplencia @Ano, @Mes";

                return context.Database.SqlQuery<CockpitInadimplenciaResult>(sqlQuery, new SqlParameter("Ano", ano.HasValue ? ano.Value : SqlInt32.Null)
                                                                           , new SqlParameter("Mes", mes.HasValue ? mes.Value : SqlInt32.Null)
                                                                           ).ToList();
            }
        }

        public List<CockpitAnaliseResult> ResultadoDiario_Analise(int? ano, int? mes)
        {
            using (var context = new DatabaseContext())
            {
                context.Database.CommandTimeout = TIMEOUT;

                var sqlQuery = "SP_Cockpit_Analise @Ano, @Mes";

                return context.Database.SqlQuery<CockpitAnaliseResult>(sqlQuery, new SqlParameter("Ano", ano.HasValue ? ano.Value : SqlInt32.Null)
                                                                     , new SqlParameter("Mes", mes.HasValue ? mes.Value : SqlInt32.Null)
                                                                     ).ToList();
            }
        }

        public List<CockpitSaldoResult> ResultadoDiario_Saldo(int? ano, int? mes)
        {
            using (var context = new DatabaseContext())
            {
                context.Database.CommandTimeout = TIMEOUT;

                var sqlQuery = "SP_Cockpit_Saldo @Ano, @Mes";

                return context.Database.SqlQuery<CockpitSaldoResult>(sqlQuery, new SqlParameter("Ano", ano.HasValue ? ano.Value : SqlInt32.Null)
                                                                   , new SqlParameter("Mes", mes.HasValue ? mes.Value : SqlInt32.Null)
                                                                   ).ToList();
            }
        }

        public List<CockpitAnaliseImplanteResult> ResultadoDiario_AnaliseImplante(int? ano, int? mes)
        {
            using (var context = new DatabaseContext())
            {
                context.Database.CommandTimeout = TIMEOUT;

                var sqlQuery = "SP_Cockpit_AnaliseImplante @Ano, @Mes";

                return context.Database.SqlQuery<CockpitAnaliseImplanteResult>(sqlQuery, new SqlParameter("Ano", ano.HasValue ? ano.Value : SqlInt32.Null)
                                                                             , new SqlParameter("Mes", mes.HasValue ? mes.Value : SqlInt32.Null)
                                                                             ).ToList();
            }
        }

        public List<CockpitFaturamentoResult> ResultadoDiario_Faturamento(int? ano, int? mes)
        {
            using (var context = new DatabaseContext())
            {
                context.Database.CommandTimeout = TIMEOUT;

                var sqlQuery = "SP_Cockpit_Faturamento @Ano, @Mes";

                return context.Database.SqlQuery<CockpitFaturamentoResult>(sqlQuery, new SqlParameter("Ano", ano.HasValue ? ano.Value : SqlInt32.Null)
                                                                             , new SqlParameter("Mes", mes.HasValue ? mes.Value : SqlInt32.Null)
                                                                             ).ToList();
            }
        }

        public CockpitDadosDentistasResult DadosDentistas(int idDoutor, int ano, int? idClinica)
        {
            using (var context = new DatabaseContext())
            {
                if (idClinica.HasValue == false)
                {
                    idClinica = IdClinicaLogada;
                }

                var dadosDentistas = new CockpitDadosDentistasResult();
                var sqlQueryFaturamento = "SP_Cockpit_DadosDoutor_Faturamento @IdClinica, @IdDoutor, @AnoReferencia";

                dadosDentistas.Faturamento.AddRange(context.Database.SqlQuery<CockpitDadosDentistasFaturamentoResult>(sqlQueryFaturamento, new SqlParameter("IdClinica", idClinica)
                                                                                                                                         , new SqlParameter("IdDoutor", idDoutor)
                                                                                                                                         , new SqlParameter("AnoReferencia", ano)
                                                                                                                                         ).ToList());

                var sqlQueryCarteiraClientes = "SP_Cockpit_DadosDoutor_CarteiraClientes @IdClinica, @IdDoutor, @AnoReferencia";
                dadosDentistas.CarteiraClientes.AddRange(context.Database.SqlQuery<CockpitDadosDentistasCarteiraClientesResult>(sqlQueryCarteiraClientes, new SqlParameter("IdClinica", idClinica)
                                                                                                                                                        , new SqlParameter("IdDoutor", idDoutor)
                                                                                                                                                        , new SqlParameter("AnoReferencia", ano)
                                                                                                                                                        ).ToList());

                return dadosDentistas;
            }
        }
        #endregion

        #region # Private Methods
        private string traduzirSemana(string dia)
        {
            switch (dia)
            {
                case "Monday": return "Segunda";
                case "Tuesday": return "Terça";
                case "Wednesday": return "Quarta";
                case "Thursday": return "Quinta";
                case "Friday": return "Sexta";
                case "Saturday": return "Sábado";
                case "Sunday": return "Domingo";
                default: return null;
            }
        }

        private string siglaDiaSemana(string dia)
        {
            switch (dia)
            {
                case "Monday": return "SEG";
                case "Tuesday": return "TER";
                case "Wednesday": return "QUA";
                case "Thursday": return "QUI";
                case "Friday": return "SEX";
                case "Saturday": return "SAB";
                case "Sunday": return "DOM";
                default: return null;
            }
        }

        private decimal calcularOcupacaoSala(string manha, string tarde, string noite)
        {
            string vazio = "";
            if (manha == vazio && tarde == vazio && noite == vazio) return 0;

            if (manha == vazio && tarde == vazio && noite != vazio) return 33.33M;
            if (manha == vazio && tarde != vazio && noite == vazio) return 33.33M;
            if (manha != vazio && tarde == vazio && noite == vazio) return 33.33M;

            if (manha == vazio && tarde != vazio && noite != vazio) return 66.66M;
            if (manha != vazio && tarde == vazio && noite != vazio) return 66.66M;
            if (manha != vazio && tarde != vazio && noite == vazio) return 66.66M;

            if (manha != vazio && tarde != vazio && noite != vazio) return 100;

            return 0;
        }
        #endregion
    }
}
