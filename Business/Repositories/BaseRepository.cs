using GrupoOrto.ERP.Entities.Attributes;
using GrupoOrto.ERP.Entities.Entity.Security;
using GrupoOrto.ERP.Entities.Entity.Table;
using GrupoOrto.ERP.Entities.Exceptions;
using GrupoOrto.ERP.Entities.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;

namespace GrupoOrto.ERP.Business.Repositories
{
    /// <summary>
    /// Repositório base com ações padrão.
    /// </summary>
    public abstract class BaseRepository : IDisposable
    {
        #region # Interface
        public void Dispose()
        {
        }
        #endregion

        /// <summary>
        /// No caso do GetByFilter não conter filtro, este exibirá uma lista limitada definida por esta variável.
        /// </summary>
        protected int ROWS_LIMIT = 100;

        /// <summary>
        /// Quando o usuário estiver utilizando um auto complete, não deve ser retornado mtos registros, caso contrário a pesquisa ficará muito lenta.
        /// </summary>
        protected int ROWS_LIMIT_AUTOCOMPLETE = 50;

        /// <summary>
        /// Permissão padrão de CRUD
        /// </summary>
        protected const short PERMISSAO_LEITURA = 1;

        /// <summary>
        /// Permissão padrão de CRUD
        /// </summary>
        protected const short PERMISSAO_CADASTRO = 2;

        /// <summary>
        /// Permissão padrão de CRUD
        /// </summary>
        protected const short PERMISSAO_ALTERACAO = 3;

        /// <summary>
        /// Permissão padrão de CRUD
        /// </summary>
        protected const short PERMISSAO_EXCLUSAO = 4;

        /// <summary>
        /// Fuso horário para cálculo do horário corrente do servidor azure. Esta informação também é calculada na classe BaseTable.
        /// </summary>
        protected const double FUSO_HORARIO = -3;

        /// <summary>
        /// Código da aplicação que está em execução. Deve ser implementado pelo repositório que herdará da BaseRepository.
        /// </summary>
        protected abstract int IdAplicacao { get; }

        /// <summary>
        /// Código da clínica ativa na sessão.
        /// </summary>
        protected int IdClinicaLogada { get { return GetLoggedUser().IdClinica; } }

        /// <summary>
        /// Código do usuário logado
        /// </summary>
        protected int IdUsuarioLogado { get { return GetLoggedUser().IdUsuario; } }

        /// <summary>
        /// Data e hora ajustado com o fuso horário.
        /// </summary>
        protected DateTime CurrentDateTime { get { return DateTime.Now.AddHours(FUSO_HORARIO); } }

        /// <summary>
        /// Retorna os dados do usuário logado, incluindo as claims.
        /// </summary>
        /// <returns></returns>
        protected UsuarioSessaoModel GetLoggedUser()
        {
            using (var context = new DatabaseContext())
            {
                var identity = (ClaimsPrincipal)System.Threading.Thread.CurrentPrincipal;
                var claimsSid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid).ToList();
                var usuarioClaim = claimsSid.FirstOrDefault();
                if (usuarioClaim == null) return null;

                var idUsuario = usuarioClaim.Value;

                var claimsName = identity.Claims.Where(c => c.Type == ClaimTypes.Name).ToList();

                var name = claimsName.First().Value;

                var claimsClinica = identity.Claims.Where(c => c.Type == "Clinica").ToList();
                var idClinica = claimsClinica.First().Value;
                var idUsuarioGrupo = identity.Claims.First(c => c.Type == "IdUsuarioGrupo").Value;

                var permissoes = JsonConvert.DeserializeObject<List<PermissaoAplicacaoIdentityModel>>(identity.Claims.First(c => c.Type == "Permissions").Value);
                var usuario = new UsuarioSessaoModel
                {
                    IdClinica = int.Parse(idClinica),
                    IdUsuario = int.Parse(idUsuario),
                    IdUsuarioGrupo = int.Parse(idUsuarioGrupo),
                    Nome = name,
                    Permissoes = permissoes
                };

                return usuario;
            }
        }

        protected bool VerificaPermissao(int permissao)
        {
            var identity = Thread.CurrentPrincipal.Identity;
            var claims = ((ClaimsIdentity)identity).Claims;
            var hasPermission = claims.FirstOrDefault(a => a.Value == IdAplicacao + "." + permissao) != null;
            return hasPermission;
        }

        /// <summary>
        /// Registra o log da atividade executada pelo usuário logado.
        /// </summary>
        /// <param name="context">Contexto do database</param>
        /// <param name="permissao">Ação realizada pelo usuário com base na permissão</param>
        /// <param name="table">Tabela afetada</param>
        protected void RegistrarLogAtividade(ITable table, short permissao, string descricaoAcao = null)
        {
            using (var context = new DatabaseContext())
            {
                RegistrarLogAtividade(context, table, permissao, descricaoAcao);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Registra o log da atividade executada pelo usuário logado.
        /// </summary>
        /// <param name="context">Contexto do database</param>
        /// <param name="permissao">Ação realizada pelo usuário com base na permissão</param>
        /// <param name="table">Tabela afetada</param>
        protected void RegistrarLogAtividade(DatabaseContext context, ITable table, short permissao, string descricaoAcao = null)
        {
            // Esta exception não deixará esquecer de configurar a ação
            if (permissao == 0) throw new BusinessException("A ação para o registro do log não foi configurado. Verifique.");

            var descricao = GetDescricaoLogAtividade(permissao, descricaoAcao, table);
            if (descricao == null) return;

            var entityName = table.GetType().Name.Replace("_", "");
            var keyName = table.GetType().GetProperty($"Id{entityName}");
            var idRegistroAfetado = (int)table.GetType().GetProperty(keyName.Name).GetValue(table);

            var entity = new Usuario_LogAtividade()
            {
                Acao = permissao,
                DataHoraCadastro = CurrentDateTime,
                IdAplicacao = IdAplicacao,
                IdClinica = IdClinicaLogada,
                IdUsuario = IdUsuarioLogado,
                Tabela = entityName,
                IdRegistroAfetado = idRegistroAfetado,
                DescricaoRegistroAfetado = descricao
            };

            context.Set<Usuario_LogAtividade>().Add(entity);
        }

        /// <summary>
        /// Cria uma descrição formatada descrevendo o registro afetado
        /// </summary>
        /// <param name="permissao">Ação executada, normalmente coincide com as permissões.</param>
        /// <param name="descricaoAcao">Descrição customizada.</param>
        /// <param name="table">Registro afetado</param>
        /// <returns>Desecrição gerada</returns>
        private string GetDescricaoLogAtividade(short permissao, string descricaoAcao, ITable table)
        {
            string descricao = "";

            switch (permissao)
            {
                case PERMISSAO_CADASTRO: descricao = "Cadastro"; break;
                case PERMISSAO_ALTERACAO: descricao = "Alteração"; break;
                case PERMISSAO_EXCLUSAO: descricao = "Exclusão"; break;
                default: descricao = descricaoAcao; break;
            }

            var attr = table.GetType().GetProperties().FirstOrDefault(prop => prop.IsDefined(typeof(LogAtividadeAttribute), false));
            if (attr == null) return descricao;

            descricao += ": " + attr.GetValue(table);
            return descricao;
        }
    }
}
