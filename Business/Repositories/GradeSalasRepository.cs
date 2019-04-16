using GrupoOrto.ERP.Entities.Entity.Table;
using GrupoOrto.ERP.Entities.Enum;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;
using GrupoOrto.ERP.Entities.Exceptions;
using System.Data.SqlClient;
using GrupoOrto.ERP.Entities.Entity.GradeSalas;

namespace GrupoOrto.ERP.Business.Repositories
{
    public class GradeSalasRepository : BaseRepository
    {
        protected override int IdAplicacao => (int)AplicacaoEnum.GradeSalas;

        /// <summary>
        /// Configuração de permissão.
        /// </summary>
        readonly short ASSOCIAR_DOUTOR;
        readonly short GERENCIAR_SALAS_PERMISSAO;
        readonly short GERAR_GRADE;
        readonly short ATUALIZAR_GRADE;
        readonly short REMOVER_ASSOCIACAO_DOUTOR;

        public GradeSalasRepository()
        {
            GERAR_GRADE = 5;
            GERENCIAR_SALAS_PERMISSAO = 6;
            ASSOCIAR_DOUTOR = 7;
            ASSOCIAR_DOUTOR = 8;
            ATUALIZAR_GRADE = 11;
        }

        #region # Interface (Fake)
        /// <summary>
        /// Retorna todas as grades de salas geradas e que estão ativas.
        /// </summary>
        /// <returns></returns>
        public List<GradeSalasListResult> GetAll()
        {
            using (var context = new DatabaseContext())
            {
                return (from grade in context.GradeSalas
                        where grade.Status == (int)DefaultStatusEnum.Ativo
                           && grade.IdClinica == IdClinicaLogada
                        select new GradeSalasListResult
                        {
                            IdGradeSalas = grade.IdGradeSalas,
                            IdClinica = grade.IdClinica,
                            AnoReferencia = grade.AnoReferencia,
                            MesReferencia = grade.MesReferencia
                        })
                        .ToList();
            }
        }

        /// <summary>
        /// Retorna uma grade específica com base na sua chave.
        /// </summary>
        /// <param name="id">Chave da grade a ser pesquisada</param>
        /// <returns>Grade encontrada</returns>
        public GradeSalasListResult GetByKey(int id)
        {
            using (var context = new DatabaseContext())
            {
                return (from grade in context.GradeSalas
                        where grade.Status == (int)DefaultStatusEnum.Ativo
                           && grade.IdClinica == IdClinicaLogada
                           && grade.IdGradeSalas == id
                        select new GradeSalasListResult
                        {
                            IdGradeSalas = grade.IdGradeSalas,
                            IdClinica = grade.IdClinica,
                            AnoReferencia = grade.AnoReferencia,
                            MesReferencia = grade.MesReferencia
                        })
                        .FirstOrDefault();
            }
        }

        /// <summary>
        /// Retorna a entidade espelho da tabela.
        /// </summary>
        /// <param name="key">Chave para encontrar o registro a ser retornado.</param>
        /// <returns></returns>
        public GradeSalas GetByKeyFull(int key)
        {
            using (var context = new DatabaseContext())
            {
                return context.GradeSalas.FirstOrDefault(a => a.IdGradeSalas == key);
            }
        }

        public List<GradeSalasListResult> GetList(GradeSalasFilterQuery filter)
        {
            using (var context = new DatabaseContext())
            {
                return null;
            }
        }

        public GradeSalasForm GetForEdit(int key)
        {
            using (var context = new DatabaseContext())
            {
                return null;
            }
        }

        public bool Delete(int key)
        {
            using (var context = new DatabaseContext())
            {
                return false;
            }
        }

        /// <summary>
        /// Remove uma grade de salas específica.
        /// </summary>
        /// <param name="key">Chave da grade a ser pesquisada.</param>
        /// <returns>Exclusão ocorrida com sucesso ou não</returns>
        public bool DeleteByKey(int key)
        {
            using (var context = new DatabaseContext())
            {
                var entity = context.GradeSalas.FirstOrDefault(a => a.IdGradeSalas == key);
                if (entity == null) throw new BusinessException("Grade de salas não encontrada");

                context.Entry(entity).State = EntityState.Deleted;

                var result = context.SaveChanges() > 0;
                if (result) RegistrarLogAtividade(entity, PERMISSAO_EXCLUSAO);
                return result;
            }
        }

        /// <summary>
        /// Salva uma grade de salas no banco de dados.
        /// </summary>
        /// <param name="entity">Entidade contendo os dados a serem salvos</param>
        /// <returns></returns>
        public bool Save(GradeSalasForm entity)
        {
            using (var context = new DatabaseContext())
            {
                bool isNew = entity.IdGradeSalas == 0;
                if (isNew)
                {
                    entity.IdUsuarioCadastro = GetLoggedUser().IdUsuario;
                    entity.IdUsuarioAlteracao = GetLoggedUser().IdUsuario;
                    entity.IdClinica = GetLoggedUser().IdClinica;

                    context.Set<GradeSalas>().Add(entity);
                }
                else
                {
                    entity.IdUsuarioAlteracao = GetLoggedUser().IdUsuario;
                    context.Entry(entity).State = EntityState.Modified;
                    entity.IdClinica = GetLoggedUser().IdClinica;
                }

                var result = context.SaveChanges() > 0;
                if (result) RegistrarLogAtividade(entity, isNew ? GERAR_GRADE : ATUALIZAR_GRADE);
                return result;
            }
        }
        #endregion

        #region >> Sala Atendimentos
        /// <summary>
        /// Retorna todas as salas de atendimento registradas
        /// </summary>
        /// <returns>Lista de salas</returns>
        public List<SalaAtendimento> GetAllSalas()
        {
            using (var context = new DatabaseContext())
            {
                int idClinica = GetLoggedUser().IdClinica;
                return context.SalaAtendimento.Where(a => a.Status == (int)DefaultStatusEnum.Ativo && a.IdClinica == idClinica).ToList();
            }
        }

        /// <summary>
        /// Retorna uma sala específica
        /// </summary>
        /// <param name="id">Chave da sala</param>
        /// <returns>Sala encontrada</returns>
        public SalaAtendimento GetSalaByKey(int id)
        {
            using (var context = new DatabaseContext())
            {
                int idClinica = GetLoggedUser().IdClinica;
                return context.SalaAtendimento.FirstOrDefault(a => a.IdClinica == idClinica && a.IdSalaAtendimento == id);
            }
        }

        /// <summary>
        /// Exclusão da sala de atendimento
        /// </summary>
        /// <param name="key">Chave da sala a ser excluída</param>
        /// <returns></returns>
        public bool DeleteSalaByKey(int key)
        {
            using (var context = new DatabaseContext())
            {
                var entity = context.SalaAtendimento.FirstOrDefault(a => a.IdSalaAtendimento == key);
                if (entity == null) throw new BusinessException("Sala de atendimento não encontrada");
                if (context.GradeSalas_Mes.Any(a => a.IdSalaAtendimento == entity.IdSalaAtendimento)) throw new BusinessException("A sala está em uso e não pode ser removida.");

                context.Entry(entity).State = EntityState.Deleted;

                var result = context.SaveChanges() > 0;
                if (result) RegistrarLogAtividade(entity, GERENCIAR_SALAS_PERMISSAO, "Exclusão da sala");
                return result;
            }
        }

        public bool SaveSala(SalaAtendimento entity)
        {
            using (var context = new DatabaseContext())
            {
                bool isNew = entity.IdSalaAtendimento == 0;
                if (isNew)
                {
                    entity.IdUsuarioCadastro = GetLoggedUser().IdUsuario;
                    entity.IdUsuarioAlteracao = GetLoggedUser().IdUsuario;
                    entity.IdClinica = GetLoggedUser().IdClinica;
                    entity.Status = (byte)DefaultStatusEnum.Ativo;

                    context.Set<SalaAtendimento>().Add(entity);
                }
                else
                {
                    entity.IdUsuarioAlteracao = GetLoggedUser().IdUsuario;
                    context.Entry(entity).State = EntityState.Modified;
                    entity.IdClinica = GetLoggedUser().IdClinica;
                }

                var result = context.SaveChanges() > 0;
                if (result) RegistrarLogAtividade(entity, GERENCIAR_SALAS_PERMISSAO, isNew ? "Cadastro" : "Alteração");
                return result;
            }
        }
        #endregion

        /// <summary>
        /// Retorna a lista de anos que já existem grades de salas gerados.
        /// </summary>
        /// <returns></returns>
        public List<int> GetAnos()
        {
            using (var context = new DatabaseContext())
            {
                return context.GradeSalas.Select(a => a.AnoReferencia).Distinct().ToList();
            }
        }

        /// <summary>
        /// Gera uma grade de salas para o ano informado como referência.
        /// </summary>
        /// <param name="anoReferencia">Ano referência para gerar a grade.</param>
        /// <returns>Grade gerada com sucesso</returns>
        public bool GerarGrade(int anoReferencia)
        {
            using (var context = new DatabaseContext())
            {
                if (context.SalaAtendimento.Where(a => a.IdClinica == IdClinicaLogada).Count() == 0)
                    throw new BusinessException("Crie as salas de atendimento antes de gerar a grade.");

                context.Database.CommandTimeout = 600000;

                var sqlQuery = "SP_GerarGradeSalas @AnoReferencia, @IdClinica, @IdUsuario";
                context.Database.ExecuteSqlCommand(sqlQuery, new SqlParameter("AnoReferencia", anoReferencia)
                                                           , new SqlParameter("IdClinica", IdClinicaLogada)
                                                           , new SqlParameter("IdUsuario", IdUsuarioLogado));

                return true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="anoReferencia"></param>
        /// <param name="mesReferencia"></param>
        /// <returns></returns>
        public GetGradeResult GetGrade(int anoReferencia, int mesReferencia)
        {
            using (var context = new DatabaseContext())
            {
                //var entity = context.GradeSalas.Include("GradeMes.SalaAtendimento")
                //                               .Include("GradeMes.Horarios.Doutor.Doutor_Especialidade.Especialidade")
                //                               .FirstOrDefault(a => a.AnoReferencia == anoReferencia && a.MesReferencia == mesReferencia && a.IdClinica == idClinica);

                var entity = (from grade in context.GradeSalas
                              join meses in context.GradeSalas_Mes on grade.IdGradeSalas equals meses.IdGradeSalas
                              where grade.AnoReferencia == anoReferencia
                                 && grade.MesReferencia == mesReferencia
                                 && grade.IdClinica == IdClinicaLogada
                              select new GetGradeResult
                              {
                                  IdGradeSalas = grade.IdGradeSalas,
                                  IdClinica = grade.IdClinica,
                                  AnoReferencia = grade.AnoReferencia,
                                  MesReferencia = grade.MesReferencia
                              }).FirstOrDefault();

                if (entity != null)
                {
                    entity.SalaAtendimento.AddRange(context.SalaAtendimento.Where(a => a.Status == (int)DefaultStatusEnum.Ativo).ToList());
                    entity.GradeMes.AddRange((from mes in context.GradeSalas_Mes
                                              where mes.IdGradeSalas == entity.IdGradeSalas
                                              select new GetGrade_MesesResult
                                              {
                                                  IdGradeSalas = mes.IdGradeSalas,
                                                  DataHoraAlteracao = mes.DataHoraAlteracao,
                                                  DataHoraCadastro = mes.DataHoraCadastro,
                                                  IdUsuarioCadastro = mes.IdUsuarioCadastro,
                                                  IdUsuarioAlteracao = mes.IdUsuarioAlteracao,
                                                  DiaReferencia = mes.DiaReferencia,
                                                  Status = mes.Status,
                                                  IdSalaAtendimento = mes.IdSalaAtendimento,
                                                  IdGradeSalasMes = mes.IdGradeSalasMes,
                                                  Horarios = (from horario in context.GradeSalas_Mes_Horario
                                                              join doutor in context.Doutor on horario.IdDoutor equals doutor.IdDoutor
                                                              where horario.IdGradeSalasMes == mes.IdGradeSalasMes
                                                              select new GetGrade_HorariosResult
                                                              {
                                                                  Periodo = horario.Periodo,
                                                                  IdDoutor = horario.IdDoutor,
                                                                  Doutor = doutor,
                                                                  Especialidades = (from especialidade in context.Especialidade
                                                                                    join especialidadeDoutor in context.Doutor_Especialidade on especialidade.IdEspecialidade equals especialidadeDoutor.IdEspecialidade
                                                                                    where especialidadeDoutor.IdDoutor == doutor.IdDoutor
                                                                                    select especialidade).ToList()
                                                              }).ToList()
                                              }).ToList());
                }

                return entity;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idGradeSalasMes"></param>
        /// <param name="idDoutor"></param>
        /// <param name="periodo"></param>
        /// <returns></returns>
        public bool AssociarDoutor(int idGradeSalasMes, int idDoutor, byte periodo)
        {
            using (var context = new DatabaseContext())
            {
                var doutorRep = new DoutorRepository();
                var doutorEntity = doutorRep.GetByKey(idDoutor);

                var entity = new GradeSalas_Mes_Horario();
                entity.DataHoraCadastro = DateTime.Now;
                entity.DataHoraAlteracao = DateTime.Now;
                entity.IdUsuarioCadastro = GetLoggedUser().IdUsuario;
                entity.IdUsuarioAlteracao = GetLoggedUser().IdUsuario;
                entity.Status = (int)DefaultStatusEnum.Ativo;
                entity.IdGradeSalasMes = idGradeSalasMes;
                entity.IdDoutor = idDoutor;
                entity.Periodo = periodo;

                context.Set<GradeSalas_Mes_Horario>().Add(entity);

                var result = context.SaveChanges() > 0;
                if (result) RegistrarLogAtividade(entity, ASSOCIAR_DOUTOR, doutorEntity.Nome);
                return result;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idGradeSalasMes"></param>
        /// <param name="idDoutor"></param>
        /// <param name="periodo"></param>
        /// <returns></returns>
        public bool RemoverAssociacaoDoutor(int idGradeSalasMes, int idDoutor, byte periodo)
        {
            using (var context = new DatabaseContext())
            {
                var entity = context.GradeSalas_Mes_Horario.FirstOrDefault(a => a.IdGradeSalasMes == idGradeSalasMes && a.IdDoutor == idDoutor && a.Periodo == periodo);
                if (entity == null) throw new BusinessException("Associação não encontrada");

                var doutorRep = new DoutorRepository();
                var doutorEntity = doutorRep.GetByKey(idDoutor);

                context.Entry(entity).State = EntityState.Deleted;

                // Salvando o log
                // SalvarLogAtividade<GradeSalas_Mes_Horario>(context, entity, ASSOCIAR_DOUTOR, null);
                var result = context.SaveChanges() > 0;
                if (result) RegistrarLogAtividade(entity, ASSOCIAR_DOUTOR, "Remover: " + doutorEntity.Nome);
                return result;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idGradeSalasMes"></param>
        /// <param name="periodo"></param>
        /// <returns></returns>
        public List<GetAllAssociacoesResult> GetAllAssociacoes(int idGradeSalasMes, byte periodo)
        {
            using (var context = new DatabaseContext())
            {
                //var entities = context.GradeSalas_Mes_Horario.Include(a => a.Doutor).Where(a => a.IdGradeSalasMes == idGradeSalasMes && a.Periodo == periodo).ToList();
                var result = (from grade in context.GradeSalas_Mes_Horario
                              join doutor in context.Doutor on grade.IdDoutor equals doutor.IdDoutor
                              where grade.IdGradeSalasMes == idGradeSalasMes
                                 && grade.Periodo == periodo
                              select new GetAllAssociacoesResult
                              {
                                  IdGradeSalasMesHorario = grade.IdGradeSalasMesHorario,
                                  IdGradeSalasMes = grade.IdGradeSalasMes,
                                  Status = grade.Status,
                                  IdDoutor = grade.IdDoutor,
                                  Doutor = doutor
                              }).ToList();

                return result;
            }
        }
    }
}
