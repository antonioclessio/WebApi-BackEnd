using Business.Repositories.v1;
using Entities.Entity.Security;
using Entities.Entity.Table;
using Entities.Exceptions;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace WebApi.Authentication
{
    public class AuthenticationProvider : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            var username = context.Parameters.Where(f => f.Key == "username").Select(f => f.Value).SingleOrDefault();
            var password = context.Parameters.Where(f => f.Key == "password").Select(f => f.Value).SingleOrDefault();
            var refresh_token = context.Parameters.Where(f => f.Key == "refresh_token").Select(f => f.Value).SingleOrDefault();
            var idClinica = context.Parameters.Where(f => f.Key == "idClinica").Select(f => f.Value).SingleOrDefault();

            if (refresh_token != null)
            {
                context.Validated();
            }
            else
            {
                if (username == null || username.Length == 0 || password == null || password.Length == 0 || idClinica == null || idClinica.Length == 0)
                {
                    context.Rejected();
                    context.SetError("invalid_client", "Informe as credenciais corretamente");
                }
                else
                {
                    context.OwinContext.Set<string>("IdClinica", idClinica[0]);
                    context.Validated();
                }
            }
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            using (var repository = new UsuarioRepository())
            {
                try
                {
                    int idClinica = int.Parse(context.OwinContext.Get<string>("IdClinica"));
                    // Verifica os dados de acesso do usuário
                    AuthenticationResult authenticationResult = repository.Authentication(new AuthenticationQuery(context.UserName, context.Password, idClinica));
                    if (authenticationResult == null)
                    {
                        context.Rejected();
                        context.SetError("UserNotFound", "Dados de autenticação inválidos");
                        return;
                    }

                    var identity = PrepareClaims(context, authenticationResult, idClinica);

                    context.Validated(identity);

                    // Salva o identity na thread criada para o acesso do usuário.
                    Thread.CurrentPrincipal = new ClaimsPrincipal(identity);

                    RegistraLogAcesso(authenticationResult, idClinica);
                }
                catch (BusinessException ex)
                {
                    context.Rejected();
                    context.SetError("UserNotFound", ex.Message);
                    return;
                }
                catch (Exception ex)
                {
                    context.Rejected();
                    context.SetError("UserNotFound", ex.Message);
                    throw new BusinessException("Ocorreu um erro ao realizar a autenticação. Tente novamente mais tarde");
                }
            }
        }

        /// <summary>
        /// Prepara as claims com base nas permissões retornadas pelo usuário
        /// </summary>
        /// <param name="context">Contexto da execução</param>
        /// <param name="authenticationResult">Resultado da autenticação com os dados do usuário</param>
        /// <param name="idClinica">Clinica selecionada no momento do login</param>
        /// <returns>Identity criado</returns>
        private ClaimsIdentity PrepareClaims(OAuthGrantResourceOwnerCredentialsContext context, AuthenticationResult authenticationResult, int idClinica)
        {
            // Busca os dados da última clínica autenticada. Caso seja o primeiro acesso, a clínica será a primeira retornada da lista de clínicas vinculadas.
            var repClinica = new ClinicaRepository();
            var clinica = repClinica.GetByKey(idClinica);

            // Recupera os dados do grupo do usuário para a clínica logada.
            var usuarioGrupoRep = new UsuarioGrupoRepository();
            var grupo = usuarioGrupoRep.GetByUsuarioClinica(authenticationResult.IdUsuario, idClinica);

            // Faz o parse da lista de permissões do banco para uma estrutura fácil de ser serializada e salva no identity.
            var listPermissaoAplicacao = JsonConvert.SerializeObject(ParsePermissaoAplicacao(grupo.Permissoes));

            // Cria o identity.
            var identity = new ClaimsIdentity(context.Options.AuthenticationType);

            identity.AddClaim(new Claim(ClaimTypes.Name, authenticationResult.Nome));
            identity.AddClaim(new Claim(ClaimTypes.Sid, authenticationResult.IdUsuario.ToString()));
            identity.AddClaim(new Claim("Clinica", clinica.IdClinica.ToString()));
            identity.AddClaim(new Claim("IdUsuarioGrupo", authenticationResult.IdUsuarioGrupo.ToString()));
            identity.AddClaim(new Claim("Permissions", listPermissaoAplicacao));

            foreach (var item in grupo.Permissoes)
            {
                var permissoes = item.Permissoes.Split('.').ToList();
                foreach (var permissao in permissoes) identity.AddClaim(new Claim(ClaimTypes.Role, item.IdAplicacao + "." + permissao));
            }

            return identity;
        }

        /// <summary>
        /// Registra o log de sucesso da autenticação.
        /// </summary>
        /// <param name="authenticationResult">Resultado da autenticação com os dados do usuário</param>
        /// <param name="idClinica">Clinica selecionada no momento do login</param>
        private void RegistraLogAcesso(AuthenticationResult authenticationResult, int idClinica)
        {
            var repLogAcesso = new UsuarioRepository();
            var ultimoAcesso = repLogAcesso.GetUltimoAcesso(authenticationResult.IdUsuario);

            repLogAcesso.Save(new Usuario_LogAcesso
            {
                IdUsuario = authenticationResult.IdUsuario,
                IPOrigem = HttpContext.Current.Request.UserHostAddress,
                // Se for o primeiro acesso, loga na primeira Usuario da lista de associados, ou então loga na última clínica acessada.
                IdClinica = idClinica
            });
        }

        private List<PermissaoAplicacaoIdentityModel> ParsePermissaoAplicacao(List<UsuarioGrupo_Permissao> source)
        {
            var objRetorno = new List<PermissaoAplicacaoIdentityModel>();
            foreach (var item in source)
            {
                var newItem = new PermissaoAplicacaoIdentityModel();
                newItem.IdAplicacao = item.IdAplicacao;
                newItem.Permissoes.AddRange(item.Permissoes.Split('.').Select(int.Parse).ToList());
                objRetorno.Add(newItem);
            }

            return objRetorno;
        }
    }
}