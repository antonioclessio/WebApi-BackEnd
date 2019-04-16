using GrupoOrto.ERP.Business.Interfaces;
using GrupoOrto.ERP.Entities.Entity.Clinica;
using GrupoOrto.ERP.Entities.Entity.LocalizacaoGeografica;
using GrupoOrto.ERP.Entities.Entity.Table;
using GrupoOrto.ERP.Entities.Enum;
using GrupoOrto.ERP.Entities.Exceptions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace GrupoOrto.ERP.Business.Repositories
{
    public class ClinicaRepository : BaseRepository, IRepository<Clinica, ClinicaListResult, ClinicaDetailResult, ClinicaFilterQuery, ClinicaForm>
    {
        protected override int IdAplicacao => (int)AplicacaoEnum.Clinica;

        #region # Interface
        public bool Delete(int key)
        {
            using (var context = new DatabaseContext())
            {
                var entity = GetByKeyFull(key);
                context.Entry(entity).State = EntityState.Deleted;

                var result = context.SaveChanges() > 0;
                if (result) RegistrarLogAtividade(entity, PERMISSAO_EXCLUSAO);
                return result;
            }
        }

        public ClinicaDetailResult GetByKey(int key)
        {
            using (var context = new DatabaseContext())
            {
                var entity = context.Clinica.FirstOrDefault(a => a.IdClinica == key);
                ClinicaDetailResult result = null;

                if (entity != null)
                {
                    LocalizacaoGeograficaResult localizacaoGeografica = null;
                    if (entity.IdLocalizacaoGeografica.HasValue)
                    {
                        var localizacaoGeograficaEntity = context.LocalizacaoGeografica.First(a => a.IdLocalizacaoGeografica == entity.IdLocalizacaoGeografica);
                        var bairroEntity = context.Bairro.First(a => a.IdBairro == localizacaoGeograficaEntity.IdBairro);
                        var cidadeEntity = context.Cidade.First(a => a.IdCidade == localizacaoGeograficaEntity.IdCidade);
                        var estadoEntity = context.Estado.First(a => a.IdEstado == cidadeEntity.IdEstado);

                        localizacaoGeografica = new LocalizacaoGeograficaResult
                        {
                            IdLocalizacaoGeografica = entity.IdLocalizacaoGeografica.Value,
                            NumeroEndereco = entity.NumeroEndereco,
                            Complemento = entity.Complemento,
                            Bairro = bairroEntity.Nome,
                            Cidade = cidadeEntity.Nome,
                            Estado = estadoEntity.Sigla,
                            Logradouro = localizacaoGeograficaEntity.Logradouro,
                            CEP = localizacaoGeograficaEntity.CEP
                        };
                    }

                    result = new ClinicaDetailResult
                    {
                        IdClinica = entity.IdClinica,
                        NomeFantasia = entity.NomeFantasia,
                        RazaoSocial = entity.RazaoSocial,
                        Sigla = entity.Sigla,
                        Status = entity.Status,
                        LocalizacaoGeografica = localizacaoGeografica
                    };

                    result.Telefones.AddRange(context.Clinica_Telefone.Where(a => a.IdClinica == entity.IdClinica));
                    result.Emails.AddRange(context.Clinica_Email.Where(a => a.IdClinica == entity.IdClinica));
                }

                return result;
            }
        }

        public ClinicaForm GetForEdit(int key)
        {
            using (var context = new DatabaseContext())
            {
                var entity = context.Clinica.FirstOrDefault(a => a.IdClinica == key);
                ClinicaForm result = null;

                if (entity != null)
                {
                    LocalizacaoGeograficaResult localizacaoGeografica = null;
                    if (entity.IdLocalizacaoGeografica.HasValue)
                    {
                        var geoLocRep = new LocalizacaoGeograficaRepository();
                        localizacaoGeografica = geoLocRep.GetByKey(entity.IdLocalizacaoGeografica.Value);
                        localizacaoGeografica.NumeroEndereco = entity.NumeroEndereco;
                        localizacaoGeografica.Complemento = entity.Complemento;
                    }

                    result = new ClinicaForm
                    {
                        IdClinica = entity.IdClinica,
                        RazaoSocial = entity.RazaoSocial,
                        NomeFantasia = entity.NomeFantasia,
                        Sigla = entity.Sigla,
                        Status = entity.Status,
                        CNPJ = entity.CNPJ,
                        InscricaoEstadual = entity.InscricaoEstadual,
                        HomePage = entity.HomePage,
                        Responsavel = entity.Responsavel,
                        LocalizacaoGeografica = localizacaoGeografica,

                        ExpedienteSegundaInicio = entity.ExpedienteSegundaInicio,
                        ExpedienteSegundaFim = entity.ExpedienteSegundaFim,
                        ExpedienteTercaInicio = entity.ExpedienteTercaInicio,
                        ExpedienteTercaFim = entity.ExpedienteTercaFim,
                        ExpedienteQuartaInicio = entity.ExpedienteQuartaInicio,
                        ExpedienteQuartaFim = entity.ExpedienteQuartaFim,
                        ExpedienteQuintaInicio = entity.ExpedienteQuintaInicio,
                        ExpedienteQuintaFim = entity.ExpedienteQuintaFim,
                        ExpedienteSextaInicio = entity.ExpedienteSextaInicio,
                        ExpedienteSextaFim = entity.ExpedienteSextaFim,
                        ExpedienteSabadoInicio = entity.ExpedienteSabadoInicio,
                        ExpedienteSabadoFim = entity.ExpedienteSabadoFim,
                        ExpedienteDomingoInicio = entity.ExpedienteDomingoInicio,
                        ExpedienteDomingoFim = entity.ExpedienteDomingoFim,

                        AlmocoSegundaInicio = entity.AlmocoSegundaInicio,
                        AlmocoSegundaFim = entity.AlmocoSegundaFim,
                        AlmocoTercaInicio = entity.AlmocoTercaInicio,
                        AlmocoTercaFim = entity.AlmocoTercaFim,
                        AlmocoQuartaInicio = entity.AlmocoQuartaInicio,
                        AlmocoQuartaFim = entity.AlmocoQuartaFim,
                        AlmocoQuintaInicio = entity.AlmocoQuintaInicio,
                        AlmocoQuintaFim = entity.AlmocoQuintaFim,
                        AlmocoSextaInicio = entity.AlmocoSextaInicio,
                        AlmocoSextaFim = entity.AlmocoSextaFim,
                        AlmocoSabadoInicio = entity.AlmocoSabadoInicio,
                        AlmocoSabadoFim = entity.AlmocoSabadoFim,
                        AlmocoDomingoInicio = entity.AlmocoDomingoInicio,
                        AlmocoDomingoFim = entity.AlmocoDomingoFim,
                    };

                    result.Telefones.AddRange(context.Clinica_Telefone.Where(a => a.IdClinica == entity.IdClinica));
                    result.Emails.AddRange(context.Clinica_Email.Where(a => a.IdClinica == entity.IdClinica));
                }

                return result;
            }
        }

        public Clinica GetByKeyFull(int key)
        {
            using (var context = new DatabaseContext())
            {
                return context.Clinica.FirstOrDefault(a => a.IdClinica == key);
            }
        }

        public List<ClinicaListResult> GetList(ClinicaFilterQuery filterView = null)
        {
            using (var context = new DatabaseContext())
            {
                List<ClinicaListResult> listEntity = null;

                if (filterView == null || filterView.IsEmpty)
                {
                    listEntity = (from clinica in context.Clinica
                                  where clinica.Status == (int)DefaultStatusEnum.Ativo
                                  select new ClinicaListResult
                                  {
                                      IdClinica = clinica.IdClinica,
                                      Sigla = clinica.Sigla,
                                      NomeFantasia = clinica.NomeFantasia,
                                      RazaoSocial = clinica.RazaoSocial,
                                      Status = clinica.Status
                                  })
                                  .OrderBy(a => a.Sigla)
                                  .ToList();
                }
                else
                {
                    listEntity = (from clinica in context.Clinica
                                  where clinica.Status == (int)DefaultStatusEnum.Ativo
                                     && (string.IsNullOrEmpty(filterView.Nome) == false || clinica.NomeFantasia.Contains(filterView.Nome))
                                     && (string.IsNullOrEmpty(filterView.Sigla) == false || clinica.Sigla.Equals(filterView.Sigla))
                                     && (filterView.Status.HasValue == false || clinica.Status.Equals(filterView.Status))
                                  select new ClinicaListResult
                                  {
                                      IdClinica = clinica.IdClinica,
                                      Sigla = clinica.Sigla,
                                      NomeFantasia = clinica.NomeFantasia,
                                      RazaoSocial = clinica.RazaoSocial,
                                      Status = clinica.Status
                                  })
                                  .OrderBy(a => a.Sigla)
                                  .ToList();
                }

                return listEntity;
            }
        }

        public bool Save(ClinicaForm entity)
        {
            if (entity.IdClinica == 0)
                return Insert(entity);

            return Update(entity);
        }

        private bool Insert(ClinicaForm model)
        {
            using (var context = new DatabaseContext())
            {
                var entity = new Clinica();
                entity.DataHoraCadastro = CurrentDateTime;
                entity.DataHoraAlteracao = CurrentDateTime;
                entity.IdUsuarioCadastro = IdUsuarioLogado;
                entity.IdUsuarioAlteracao = IdUsuarioLogado;
                entity.Status = (int)DefaultStatusEnum.Ativo;

                entity.RazaoSocial = model.RazaoSocial;
                entity.NomeFantasia = model.NomeFantasia;
                entity.Sigla = model.Sigla;
                entity.CNPJ = model.CNPJ;
                entity.InscricaoEstadual = model.InscricaoEstadual;
                entity.HomePage = model.HomePage;
                entity.Responsavel = model.Responsavel;
                entity.IdLocalizacaoGeografica = model.IdLocalizacaoGeografica;
                entity.NumeroEndereco = model.NumeroEndereco;
                entity.Complemento = model.Complemento;
                entity.ExpedienteSegundaInicio = model.ExpedienteSegundaInicio;
                entity.ExpedienteSegundaFim = model.ExpedienteSegundaFim;
                entity.ExpedienteTercaInicio = model.ExpedienteTercaInicio;
                entity.ExpedienteTercaFim = model.ExpedienteTercaFim;
                entity.ExpedienteQuartaInicio = model.ExpedienteQuartaInicio;
                entity.ExpedienteQuartaFim = model.ExpedienteQuartaFim;
                entity.ExpedienteQuintaInicio = model.ExpedienteQuintaInicio;
                entity.ExpedienteQuintaFim = model.ExpedienteQuintaFim;
                entity.ExpedienteSextaInicio = model.ExpedienteSextaInicio;
                entity.ExpedienteSextaFim = model.ExpedienteSextaFim;
                entity.ExpedienteSabadoInicio = model.ExpedienteSabadoInicio;
                entity.ExpedienteSabadoFim = model.ExpedienteSabadoFim;
                entity.ExpedienteDomingoInicio = model.ExpedienteDomingoInicio;
                entity.ExpedienteDomingoFim = model.ExpedienteDomingoFim;
                entity.AlmocoSegundaInicio = model.AlmocoSegundaInicio;
                entity.AlmocoSegundaFim = model.AlmocoSegundaFim;
                entity.AlmocoTercaInicio = model.AlmocoTercaInicio;
                entity.AlmocoTercaFim = model.AlmocoTercaFim;
                entity.AlmocoQuartaInicio = model.AlmocoQuartaInicio;
                entity.AlmocoQuartaFim = model.AlmocoQuartaFim;
                entity.AlmocoQuintaInicio = model.AlmocoQuintaInicio;
                entity.AlmocoQuintaFim = model.AlmocoQuintaFim;
                entity.AlmocoSextaInicio = model.AlmocoSextaInicio;
                entity.AlmocoSextaFim = model.AlmocoSextaFim;
                entity.AlmocoSabadoInicio = model.AlmocoSabadoInicio;
                entity.AlmocoSabadoFim = model.AlmocoSabadoFim;
                entity.AlmocoDomingoInicio = model.AlmocoDomingoInicio;
                entity.AlmocoDomingoFim = model.AlmocoDomingoFim;
                entity.OrdemExibicao = model.OrdemExibicao;

                model.Telefones.ForEach(item => {
                    item.DataHoraCadastro = CurrentDateTime;
                    item.DataHoraAlteracao = CurrentDateTime;
                    item.IdUsuarioCadastro = IdUsuarioLogado;
                    item.IdUsuarioAlteracao = IdUsuarioLogado;
                    item.Status = (int)DefaultStatusEnum.Ativo;
                    item.IdClinica = entity.IdClinica;

                    context.Clinica_Telefone.Add(item);
                });

                model.Emails.ForEach(item => {
                    item.DataHoraCadastro = CurrentDateTime;
                    item.DataHoraAlteracao = CurrentDateTime;
                    item.IdUsuarioCadastro = IdUsuarioLogado;
                    item.IdUsuarioAlteracao = IdUsuarioLogado;
                    item.Status = (int)DefaultStatusEnum.Ativo;
                    item.IdClinica = entity.IdClinica;

                    context.Clinica_Email.Add(item);
                });

                context.Set<Clinica>().Add(entity);

                var result = context.SaveChanges() > 0;
                if (result) RegistrarLogAtividade(entity, PERMISSAO_CADASTRO);
                return result;
            }
        }

        private bool Update(ClinicaForm model)
        {
            using (var context = new DatabaseContext())
            {
                var entity = GetByKeyFull(model.IdClinica);
                if (entity == null) throw new BusinessException("Clínica não encontrada");

                context.Entry(entity).State = EntityState.Modified;
                
                entity.DataHoraAlteracao = CurrentDateTime;
                entity.IdUsuarioAlteracao = IdUsuarioLogado;
                entity.Status = model.Status;
                entity.RazaoSocial = model.RazaoSocial;
                entity.NomeFantasia = model.NomeFantasia;
                entity.Sigla = model.Sigla;
                entity.CNPJ = model.CNPJ;
                entity.InscricaoEstadual = model.InscricaoEstadual;
                entity.HomePage = model.HomePage;
                entity.Responsavel = model.Responsavel;
                entity.IdLocalizacaoGeografica = model.IdLocalizacaoGeografica;
                entity.NumeroEndereco = model.NumeroEndereco;
                entity.Complemento = model.Complemento;
                entity.ExpedienteSegundaInicio = model.ExpedienteSegundaInicio;
                entity.ExpedienteSegundaFim = model.ExpedienteSegundaFim;
                entity.ExpedienteTercaInicio = model.ExpedienteTercaInicio;
                entity.ExpedienteTercaFim = model.ExpedienteTercaFim;
                entity.ExpedienteQuartaInicio = model.ExpedienteQuartaInicio;
                entity.ExpedienteQuartaFim = model.ExpedienteQuartaFim;
                entity.ExpedienteQuintaInicio = model.ExpedienteQuintaInicio;
                entity.ExpedienteQuintaFim = model.ExpedienteQuintaFim;
                entity.ExpedienteSextaInicio = model.ExpedienteSextaInicio;
                entity.ExpedienteSextaFim = model.ExpedienteSextaFim;
                entity.ExpedienteSabadoInicio = model.ExpedienteSabadoInicio;
                entity.ExpedienteSabadoFim = model.ExpedienteSabadoFim;
                entity.ExpedienteDomingoInicio = model.ExpedienteDomingoInicio;
                entity.ExpedienteDomingoFim = model.ExpedienteDomingoFim;
                entity.AlmocoSegundaInicio = model.AlmocoSegundaInicio;
                entity.AlmocoSegundaFim = model.AlmocoSegundaFim;
                entity.AlmocoTercaInicio = model.AlmocoTercaInicio;
                entity.AlmocoTercaFim = model.AlmocoTercaFim;
                entity.AlmocoQuartaInicio = model.AlmocoQuartaInicio;
                entity.AlmocoQuartaFim = model.AlmocoQuartaFim;
                entity.AlmocoQuintaInicio = model.AlmocoQuintaInicio;
                entity.AlmocoQuintaFim = model.AlmocoQuintaFim;
                entity.AlmocoSextaInicio = model.AlmocoSextaInicio;
                entity.AlmocoSextaFim = model.AlmocoSextaFim;
                entity.AlmocoSabadoInicio = model.AlmocoSabadoInicio;
                entity.AlmocoSabadoFim = model.AlmocoSabadoFim;
                entity.AlmocoDomingoInicio = model.AlmocoDomingoInicio;
                entity.AlmocoDomingoFim = model.AlmocoDomingoFim;
                entity.OrdemExibicao = model.OrdemExibicao;

                var listTelefone = context.Clinica_Telefone.Where(a => a.IdClinica == entity.IdClinica);
                var listEmails = context.Clinica_Email.Where(a => a.IdClinica == entity.IdClinica);

                context.Clinica_Telefone.RemoveRange(listTelefone);
                context.Clinica_Email.RemoveRange(listEmails);

                model.Telefones.ForEach(item => {
                    item.DataHoraCadastro = CurrentDateTime;
                    item.DataHoraAlteracao = CurrentDateTime;
                    item.IdUsuarioCadastro = IdUsuarioLogado;
                    item.IdUsuarioAlteracao = IdUsuarioLogado;
                    item.Status = (int)DefaultStatusEnum.Ativo;
                    item.IdClinica = entity.IdClinica;

                    context.Clinica_Telefone.Add(item);
                });

                model.Emails.ForEach(item => {
                    item.DataHoraCadastro = CurrentDateTime;
                    item.DataHoraAlteracao = CurrentDateTime;
                    item.IdUsuarioCadastro = IdUsuarioLogado;
                    item.IdUsuarioAlteracao = IdUsuarioLogado;
                    item.Status = (int)DefaultStatusEnum.Ativo;
                    item.IdClinica = entity.IdClinica;

                    context.Clinica_Email.Add(item);
                });

                var result = context.SaveChanges() > 0;
                if (result) RegistrarLogAtividade(entity, PERMISSAO_ALTERACAO);
                return result;
            }
        }
        #endregion

        /// <summary>
        /// Retorna a lista de clínicas que podem ser acessadas sem autenticação. Geralmente usado para login.
        /// </summary>
        /// <returns></returns>
        public List<ClinicaPublicResult> GetListPublic()
        {
            using (var context = new DatabaseContext())
            {
                var result = (from clinica in context.Clinica
                              where clinica.Status == (int)DefaultStatusEnum.Ativo
                              select new ClinicaPublicResult
                              {
                                  IdClinica = clinica.IdClinica,
                                  Sigla = clinica.Sigla
                              })
                              .OrderBy(a => a.Sigla)
                              .ToList();

                return result;
            }
        }

        public List<GetListaSiglaResult> GetListaSigla()
        {
            using (var context = new DatabaseContext())
            {
                return (from clinica in context.Clinica
                        where clinica.Status == (int)DefaultStatusEnum.Ativo
                        select new GetListaSiglaResult
                        {
                            IdClinica = clinica.IdClinica,
                            Sigla = clinica.Sigla
                        })
                        .OrderBy(a => a.Sigla)
                        .ToList();
            }
        }
    }
}
