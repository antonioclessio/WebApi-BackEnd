using System;
using System.Collections.Generic;
using System.Linq;
using GrupoOrto.ERP.Business.Interfaces;
using GrupoOrto.ERP.Common.Security;
using GrupoOrto.ERP.Entities.Entity.Usuario;
using GrupoOrto.ERP.Entities.Entity.Security;
using GrupoOrto.ERP.Entities.Entity.Table;
using GrupoOrto.ERP.Entities.Exceptions;
using System.Data.Entity;
using GrupoOrto.ERP.Entities.Enum;
using System.Data.SqlClient;

namespace GrupoOrto.ERP.Business.Repositories
{
    public class UsuarioRepository : BaseRepository, IRepository<Usuario, UsuarioListResult, UsuarioDetailResult, UsuarioFilterQuery, UsuarioForm>
    {
        protected override int IdAplicacao => (int)AplicacaoEnum.Usuario;

        /// <summary>
        /// Senha padrão para cadastro e resetar senha.
        /// </summary>
        private const string DEFAULT_PASSWORD = "grupoorto";

        /// <summary>
        /// Permissão para resetar a senha. Deve estar coerente com a configurada no banco de dados.
        /// </summary>
        private const int RESETAR_SENHA = 5;

        /// <summary>
        /// Permissão para alterar a senha. Deve estar coerente com a configurada no banco de dados.
        /// </summary>
        private const int ALTERAR_SENHA = 6;

        #region # Interface
        public bool Delete(int key)
        {
            using (var context = new DatabaseContext())
            {
                var entity = context.Usuario.FirstOrDefault(a => a.IdUsuario == key);
                if (entity == null) throw new BusinessException("Usuário não encontrado");
                if (entity.IdUsuario == 1) throw new BusinessException($"O usuário {entity.Nome} não pode ser excluido");

                var entityClinicas = context.Clinica_Usuario.Where(a => a.IdUsuario == key).ToList();
                foreach (var item in entityClinicas) context.Clinica_Usuario.Remove(item);

                var entityLogAcesso = context.Usuario_LogAcesso.Where(a => a.IdUsuario == key).ToList();
                foreach (var item in entityLogAcesso) context.Usuario_LogAcesso.Remove(item);

                context.Entry<Usuario>(entity).State = EntityState.Deleted;

                var result = context.SaveChanges() > 0;
                if (result) RegistrarLogAtividade(entity, PERMISSAO_EXCLUSAO);
                return result;
            }
        }

        public UsuarioDetailResult GetByKey(int key)
        {
            using (var context = new DatabaseContext())
            {
                var entity = context.Usuario.FirstOrDefault(a => a.IdUsuario == key);
                if (entity == null) throw new BusinessException("Usuário não encontrado");

                DateTime? dataUltimAcesso = null;

                // Recuperando o último acesso do usuário
                var usuarioLogAcesso = context.Usuario_LogAcesso.Where(a => a.IdUsuario == key).OrderByDescending(a => a.DataHoraCadastro).FirstOrDefault();
                if (usuarioLogAcesso != null) dataUltimAcesso = usuarioLogAcesso.DataHoraCadastro;

                // Garantindo que informações sensíveis não sejam retornadas ao client.
                entity.Senha = entity.SenhaSalt = null;

                return new UsuarioDetailResult
                {
                    IdUsuario = entity.IdUsuario,
                    Nome = entity.Nome,
                    DataHoraCadastro = entity.DataHoraCadastro,
                    Email = entity.Email,
                    UltimoAcesso = dataUltimAcesso,
                    Status = entity.Status
                };
            }
        }

        public Usuario GetByKeyFull(int key)
        {
            using (var context = new DatabaseContext())
            {
                return context.Usuario.FirstOrDefault(a => a.IdUsuario == key);
            }
        }

        public UsuarioFullWithClinicaResult GetByKeyFullWithClinica(int key)
        {
            using (var context = new DatabaseContext())
            {
                var entity = (from usuario in context.Usuario
                              where usuario.IdUsuario == key
                              select new UsuarioFullWithClinicaResult
                              {
                                  Nome = usuario.Nome,
                                  Status = usuario.Status,
                                  DataHoraCadastro = usuario.DataHoraCadastro,
                                  DataHoraAlteracao = usuario.DataHoraAlteracao,
                                  IdUsuario = usuario.IdUsuario,
                                  IdUsuarioAlteracao = usuario.IdUsuarioAlteracao,
                                  IdUsuarioCadastro = usuario.IdUsuarioCadastro,
                                  PrimeiroAcesso = usuario.PrimeiroAcesso,
                                  Email = usuario.Email
                              })
                              .FirstOrDefault();

                if (entity == null) return null;

                var clinicaUsuarioRep = new Clinica_UsuarioRepository();
                entity.Clinicas = clinicaUsuarioRep.GetByUsuario(key, IdClinicaLogada);

                return entity;
            }
        }

        public UsuarioForm GetForEdit(int key)
        {
            using (var context = new DatabaseContext())
            {
                var entity = context.Usuario.FirstOrDefault(a => a.IdUsuario == key);
                if (entity == null) return null;

                var clinicasUsuarioRep = new Clinica_UsuarioRepository();
                var clinicasUsuario = clinicasUsuarioRep.GetByUsuario(entity.IdUsuario);

                return new UsuarioForm
                {
                    IdUsuario = entity.IdUsuario,
                    Status = entity.Status,
                    Nome = entity.Nome,
                    Email = entity.Email,
                    Clinicas = clinicasUsuario
                };
            }
        }

        public List<UsuarioListResult> GetList(UsuarioFilterQuery filterView)
        {
            using (var context = new DatabaseContext())
            {
                List<UsuarioListResult> result = null;

                if (filterView == null || filterView.IsEmpty)
                {
                    result = (from usuario in context.Usuario
                              join clinica_usuario in context.Clinica_Usuario on usuario.IdUsuario equals clinica_usuario.IdUsuario
                              where (usuario.Status == (int)DefaultStatusEnum.Ativo) //  && clinica_usuario.IdClinica == IdClinicaLogada
                              select new UsuarioListResult
                              {
                                  IdUsuario = usuario.IdUsuario,
                                  Status = usuario.Status,
                                  Nome = usuario.Nome,
                                  Email = usuario.Email
                              })
                              .OrderByDescending(a => a.IdUsuario)
                              .Take(ROWS_LIMIT)
                              .ToList();
                } else
                {
                    result = (from usuario in context.Usuario
                              join clinica_usuario in context.Clinica_Usuario on usuario.IdUsuario equals clinica_usuario.IdUsuario
                              where (filterView.Nome.Trim() == null || usuario.Nome.Contains(filterView.Nome))
                                 && (filterView.Email.Trim() == null || usuario.Email.Contains(filterView.Email))
                                 && (filterView.Status.HasValue == false || usuario.Status == filterView.Status.Value)
                                 && (clinica_usuario.IdClinica == IdClinicaLogada)
                              select new UsuarioListResult
                              {
                                  IdUsuario = usuario.IdUsuario,
                                  Status = usuario.Status,
                                  Nome = usuario.Nome,
                                  Email = usuario.Email
                              })
                          .ToList();
                }

                return result;
            }
        }

        public bool Save(UsuarioForm entity)
        {
            if (entity.IdUsuario == 0)
                return Insert(entity);

            return Update(entity);
        }

        private bool Insert(UsuarioForm model)
        {
            using (var context = new DatabaseContext())
            {
                var emailExistente = context.Usuario.FirstOrDefault(a => a.Email == model.Email);
                if (emailExistente != null) throw new BusinessException($"O e-mail {model.Email} já está sendo utilizado, escolha outro e-mail para continuar");

                var entity = new Usuario();

                entity.DataHoraCadastro = CurrentDateTime;
                entity.DataHoraAlteracao = CurrentDateTime;
                entity.IdUsuarioCadastro = IdUsuarioLogado;
                entity.IdUsuarioAlteracao = IdUsuarioLogado;
                entity.Status = (int)DefaultStatusEnum.Ativo;
                entity.Nome = model.Nome;
                entity.Email = model.Email;
                entity.Senha = PasswordHash.CreateHash(DEFAULT_PASSWORD);
                entity.SenhaSalt = PasswordHash.CreateSalt();
                entity.PrimeiroAcesso = true;

                context.Set<Usuario>().Add(entity);

                foreach (var item in model.Clinicas)
                {
                    var clinica = new Clinica_Usuario();
                    clinica.DataHoraCadastro = CurrentDateTime;
                    clinica.DataHoraAlteracao = CurrentDateTime;
                    clinica.IdUsuarioCadastro = IdUsuarioLogado;
                    clinica.IdUsuarioAlteracao = IdUsuarioLogado;
                    clinica.Status = (int)DefaultStatusEnum.Ativo;
                    clinica.IdUsuario = entity.IdUsuario;
                    clinica.IdClinica = item.IdClinica;
                    clinica.IdUsuarioGrupo = item.IdUsuarioGrupo;

                    context.Set<Clinica_Usuario>().Add(clinica);
                }

                var result = context.SaveChanges() > 0;
                if (result) RegistrarLogAtividade(entity, PERMISSAO_CADASTRO);
                return result;
            }
        }

        private bool Update(UsuarioForm model)
        {
            using (var context = new DatabaseContext())
            {
                var entity = GetByKeyFull(model.IdUsuario);
                if (entity.IdUsuario == 1)
                    throw new BusinessException($"O usuário '{entity.Nome}' não pode ser alterado");

                if (entity.Email != model.Email)
                    throw new BusinessException("O e-mail não pode ser alterado.");

                context.Entry(entity).State = EntityState.Modified;

                entity.Nome = model.Nome;
                entity.DataHoraAlteracao = CurrentDateTime;
                entity.IdUsuarioAlteracao = IdUsuarioLogado;

                var clinicasAssociadas = context.Clinica_Usuario.Where(a => a.IdUsuario == entity.IdUsuario);
                foreach (var item in clinicasAssociadas) context.Clinica_Usuario.Remove(item);

                foreach (var item in model.Clinicas)
                {
                    var clinica = new Clinica_Usuario();
                    clinica.DataHoraCadastro = CurrentDateTime;
                    clinica.DataHoraAlteracao = CurrentDateTime;
                    clinica.IdUsuarioCadastro = IdUsuarioLogado;
                    clinica.IdUsuarioAlteracao = IdUsuarioLogado;
                    clinica.Status = (int)DefaultStatusEnum.Ativo;
                    clinica.IdUsuario = entity.IdUsuario;
                    clinica.IdClinica = item.IdClinica;
                    clinica.IdUsuarioGrupo = item.IdUsuarioGrupo;

                    context.Set<Clinica_Usuario>().Add(clinica);
                }

                var result = context.SaveChanges() > 0;
                if (result) RegistrarLogAtividade(entity, PERMISSAO_ALTERACAO);
                return result;
            }
        }
        #endregion

        /// <summary>
        /// Realiza a verificação do usuário para autenticação.
        /// </summary>
        /// <param name="email">Email do usuário</param>
        /// <param name="password">Senha do usuário</param>
        /// <returns>Entidade com o usuário encontrado</returns>
        public AuthenticationResult Authentication(AuthenticationQuery query)
        {
            using (var context = new DatabaseContext())
            {
                try
                {
                    var user = (from usuario in context.Usuario
                                join clinica in context.Clinica_Usuario on usuario.IdUsuario equals clinica.IdUsuario
                                where usuario.Email.Equals(query.Username) && clinica.IdClinica == query.IdClinica
                                select new
                                {
                                    Nome = usuario.Nome,
                                    Senha = usuario.Senha,
                                    IdUsuario = usuario.IdUsuario,
                                    IdUsuarioGrupo = clinica.IdUsuarioGrupo,
                                    PrimeiroAcesso = usuario.PrimeiroAcesso
                                })
                                .FirstOrDefault();

                    if (user == null || PasswordHash.ValidatePassword(query.Password, user.Senha) == false) return null;

                    return new AuthenticationResult
                    {
                        Nome = user.Nome,
                        IdUsuario = user.IdUsuario,
                        PrimeiroAcesso = user.PrimeiroAcesso,
                        IdUsuarioGrupo = user.IdUsuarioGrupo
                    };
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Apenas reseta a senha de acesso do usuário para a senha padrão.
        /// </summary>
        /// <param name="id">Código do usuário que terá a senha resetada</param>
        /// <returns></returns>
        public Usuario ResetarSenha(int id)
        {
            using (var context = new DatabaseContext())
            {
                var entity = context.Usuario.FirstOrDefault(a => a.IdUsuario == id);
                if (entity == null) throw new BusinessException("Usuário não encontrado");

                entity.Senha = PasswordHash.CreateHash(DEFAULT_PASSWORD);
                entity.SenhaSalt = PasswordHash.CreateSalt();
                entity.DataHoraAlteracao = DateTime.Now;
                entity.IdUsuarioAlteracao = GetLoggedUser().IdUsuario;
                entity.PrimeiroAcesso = true;

                context.Entry(entity).State = EntityState.Modified;

                var result = context.SaveChanges() > 0;
                if (result) RegistrarLogAtividade(entity, RESETAR_SENHA);

                return entity;
            }
        }

        /// <summary>
        /// Apenas altera a senha de acesso do usuário. Somente o usuário logado poderá alterar sua senha.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AlterarSenha(AlterarSenhaModel model)
        {
            if (model.IdUsuario != GetLoggedUser().IdUsuario) throw new BusinessException("Não é permitido alterar a senha de outro usuário");
            if (model.Senha != model.Confirmar) throw new BusinessException("As senhas informadas não conferem");

            using (var context = new DatabaseContext())
            {
                var entity = context.Usuario.FirstOrDefault(a => a.IdUsuario == model.IdUsuario);
                if (entity == null) throw new BusinessException("Usuário não encontrado");

                entity.Senha = PasswordHash.CreateHash(model.Senha);
                entity.SenhaSalt = PasswordHash.CreateSalt();
                entity.DataHoraAlteracao = DateTime.Now;
                entity.IdUsuarioAlteracao = GetLoggedUser().IdUsuario;
                entity.PrimeiroAcesso = false;

                context.Entry(entity).State = EntityState.Modified;

                var result = context.SaveChanges() > 0;
                if (result) RegistrarLogAtividade(entity, ALTERAR_SENHA);

                return result;
            }
        }

        /// <summary>
        /// Retorna o último acesso realizado por um usuário.
        /// </summary>
        /// <param name="idUsuario">IdUsuario do usuário a ter o último acesso recuperado.</param>
        /// <returns></returns>
        public Usuario_LogAcesso GetUltimoAcesso(int idUsuario)
        {
            using (var context = new DatabaseContext())
            {
                var entity = context.Usuario_LogAcesso.Where(a => a.IdUsuario == idUsuario).OrderByDescending(a => a.DataHoraCadastro).FirstOrDefault();
                return entity;
            }
        }

        /// <summary>
        /// Registra o momento em que a autenticação aconteceu.
        /// </summary>
        /// <param name="entity">Entidade com os dados que serão salvos.</param>
        /// <returns>DLog registrado com sucesso</returns>
        public bool Save(Usuario_LogAcesso entity)
        {
            using (var context = new DatabaseContext())
            {
                entity.DataHoraCadastro = CurrentDateTime;
                entity.DataHoraAlteracao = CurrentDateTime;
                entity.IdUsuarioCadastro = IdUsuarioLogado;
                entity.IdUsuarioAlteracao = IdUsuarioLogado;
                entity.Status = (int)DefaultStatusEnum.Ativo;

                context.Set<Usuario_LogAcesso>().Add(entity);
                return context.SaveChanges() > 0;
            }
        }

        public List<UsuarioLogAtividadeResult> GetLogAtividades(int idUsuario, DateTime dataReferencia)
        {
            using (var context = new DatabaseContext())
            {
                var sqlQuery = "SP_LogAtividadeUsuario @IdUsuario, @DataReferencia";
                var result = context.Database.SqlQuery<UsuarioLogAtividadeResult>(sqlQuery, new SqlParameter("IdUsuario", idUsuario)
                                                                                          , new SqlParameter("DataReferencia", dataReferencia)).ToList();

                return result;
            }
        }

        public List<UsuarioLogAcessoResult> GetLogAcessos(int idUsuario, DateTime dataReferencia)
        {
            using (var context = new DatabaseContext())
            {
                var sqlQuery = "SP_LogAcessoUsuario @IdUsuario, @DataReferencia";
                var result = context.Database.SqlQuery<UsuarioLogAcessoResult>(sqlQuery, new SqlParameter("IdUsuario", idUsuario)
                                                                                       , new SqlParameter("DataReferencia", dataReferencia)).ToList();

                return result;
            }
        }

        #region # Relatórios
        public List<RelatorioLogAcessoUsuarioResult> RelatorioLogAcessoUsuario(RelatorioLogAcessoUsuarioModel model)
        {
            using (var context = new DatabaseContext())
            {
                var sqlQuery = "SP_RelatorioLogAcessoUsuario @IdUsuario, @DataReferenciaInicio, @DataReferenciaFim";
                return context.Database.SqlQuery<RelatorioLogAcessoUsuarioResult>(sqlQuery
                                                                                 , new SqlParameter("IdUsuario", model.IdUsuario)
                                                                                 , new SqlParameter("DataReferenciaInicio", model.DataReferenciaInicio)
                                                                                 , new SqlParameter("DataReferenciaFim", model.DataReferenciaFim)
                                                                                 ).ToList();
            }
        }

        public List<RelatorioLogAtividadeUsuarioResult> RelatorioLogAtividcadeUsuario(RelatorioLogAtividadeUsuarioModel model)
        {
            using (var context = new DatabaseContext())
            {
                var sqlQuery = "SP_RelatorioLogAtividadeUsuario @IdUsuario, @DataReferencia, @IdAplicacao, @IndicePermissao";
                return context.Database.SqlQuery<RelatorioLogAtividadeUsuarioResult>(sqlQuery
                                                                                 , new SqlParameter("IdUsuario", model.IdUsuario)
                                                                                 , new SqlParameter("DataReferencia", model.DataReferencia)
                                                                                 , new SqlParameter("IdAplicacao", model.Aplicacao)
                                                                                 , new SqlParameter("IndicePermissao", model.Permissao)
                                                                                 ).ToList();
            }
        }
        #endregion
    }
}
