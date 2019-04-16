using Entities.Entity.Aplicacao;
using Entities.Entity.Table;
using Entities.Enum;
using System.Collections.Generic;
using System.Linq;

namespace Business.Repositories.v1
{
    public class ApplicationRepository : BaseRepository
    {
        protected override int IdAplicacao => 0;

        #region # Application Structure
        /// <summary>
        /// Retorna toda a estrutura necessária para utilização da aplicação.
        /// </summary>
        /// <param name="idClinica"></param>
        /// <returns></returns>
        public ApplicationStructureResult GetApplicationStructure(int? idClinica = null)
        {
            var appStructure = new ApplicationStructureResult();
            appStructure.MainMenu.AddRange(RetornarMainMenu());

            appStructure.Cockpit = RetornarCockpit();

            UsuarioRepository usuarioRepository = new UsuarioRepository();
            var usuarioLogado = usuarioRepository.GetByKeyFullWithClinica(IdUsuarioLogado);
            usuarioLogado.Senha = usuarioLogado.SenhaSalt = null;
            appStructure.UsuarioLogado = usuarioLogado;

            ClinicaRepository clinicaRep = new ClinicaRepository();
            appStructure.Clinica = clinicaRep.GetByKeyFull(IdClinicaLogada);

            return appStructure;
        }

        /// <summary>
        /// Retorna o menu estruturado
        /// </summary>
        /// <returns>Lista de menus</returns>
        private List<MenuByParentResult> RetornarMainMenu()
        {
            using (var context = new DatabaseContext())
            {
                var appMainMenu = new List<MenuByParentResult>();

                var usuarioGrupoRep = new UsuarioGrupoRepository();
                var usuarioGrupoUserLogado = usuarioGrupoRep.GetByUsuarioClinica(IdUsuarioLogado, IdClinicaLogada);
                if (usuarioGrupoUserLogado.Permissoes == null) return new List<MenuByParentResult>();
                var listaAplicacoesPermitidas = usuarioGrupoUserLogado.Permissoes.Select(a => a.IdAplicacao).ToList();

                // Recuperando o primeiro nível do menu.
                var mainMenuFirstLevel = RetornarMenuByParent(null);

                // Para cada item do primeiro nível do menu
                foreach (var menuItem in mainMenuFirstLevel)
                {
                    // Busca os itens que compõe o menu, isto inclui as aplicações e submenus.
                    var subMenuEntity = RetornarMenuByParent(menuItem.IdMainMenu);

                    // Este for each irá verificar se trata-se de um submenu ou de uma aplicação
                    foreach (var subMenu in subMenuEntity)
                    {
                        // Se for uma aplicação e esta estiver na lista de permissões do grupo do usuário...
                        if (subMenu.IdAplicacao.HasValue && listaAplicacoesPermitidas.Contains(subMenu.IdAplicacao.Value))
                        {
                            // Adiciona no menu que será retornado.
                            menuItem.Itens.Add(subMenu);
                        }
                        else // ... ou se for um subMenu
                        {
                            // Retornando os filhos do subMenu e varrendo cada item
                            var subSubMenuEntity = RetornarMenuByParent(subMenu.IdMainMenu);
                            foreach (var subSubMenu in subSubMenuEntity)
                            {
                                if (subSubMenu.IdAplicacao.HasValue && listaAplicacoesPermitidas.Contains(subSubMenu.IdAplicacao.Value))
                                {
                                    subMenu.Itens.Add(subSubMenu);
                                }
                                else
                                {
                                    var ultimoNivelMenuEntity = RetornarMenuByParent(subSubMenu.IdMainMenu);
                                    foreach (var ultimoNivel in ultimoNivelMenuEntity)
                                    {
                                        if (ultimoNivel.Aplicacao != null && ultimoNivel.Aplicacao.IdAplicacaoPai.HasValue)
                                        //if (ultimoNivel.Aplicacao != null)
                                        {
                                            var idAplicacao = ultimoNivel.Aplicacao.IdAplicacaoPai.HasValue ?
                                                              ultimoNivel.Aplicacao.IdAplicacaoPai.Value :
                                                              ultimoNivel.Aplicacao.IdAplicacao;

                                            var app = usuarioGrupoUserLogado.Permissoes.First(a => a.IdAplicacao == idAplicacao);
                                            if (app.Permissoes.Split('.').ToList().Contains(ultimoNivel.Aplicacao.Permissao.ToString())) subSubMenu.Itens.Add(ultimoNivel);

                                            continue;
                                        }

                                        // Se não for uma aplicação ou se não estiver na lista de itens permitidos pelo grupo, então passa para o próximo item.
                                        if (ultimoNivel.IdAplicacao.HasValue || !listaAplicacoesPermitidas.Contains(ultimoNivel.IdAplicacao.Value)) continue;
                                            subSubMenu.Itens.Add(ultimoNivel);
                                    }

                                    // Se o subMenu estiver com algum filho, então adiciona, caso contrário o subMenu é ignorado.
                                    if (subSubMenu.Itens.Count() > 0) subMenu.Itens.Add(subSubMenu);
                                }
                            }

                            // Se o subMenu estiver com algum filho, então adiciona, caso contrário o subMenu é ignorado.
                            if (subMenu.Itens.Count() > 0) menuItem.Itens.Add(subMenu);
                        }
                    }

                    appMainMenu.Add(menuItem);
                }

                // Retorna somente os menus que contém filhos.
                return appMainMenu.Where(a => a.Itens.Count > 0).ToList();
            }
        }

        /// <summary>
        /// Retorna os menus a partir do Id do menu pai.
        /// </summary>
        /// <param name="idMainMenu"></param>
        /// <returns></returns>
        private List<MenuByParentResult> RetornarMenuByParent(int? idMainMenu)
        {
            using (var context = new DatabaseContext())
            {
                //string includeAplicacao = string.Format("{0}.{1}", typeof(Aplicacao).Name, typeof(Aplicacao_Permissao).Name);
                var structure = (from entity in context.AppMainMenu
                                 join tmpAplicacao in context.Aplicacao on entity.IdAplicacao equals tmpAplicacao.IdAplicacao into aplicacoes
                                 from aplicacao in aplicacoes.DefaultIfEmpty()
                                 where entity.Status == (int)DefaultStatusEnum.Ativo && entity.IdParentMenu == idMainMenu
                                 select new MenuByParentResult
                                 {
                                     Favorito = entity.Favorito,
                                     Icon = entity.Icon,
                                     TemDivisor = entity.TemDivisor,
                                     Status = entity.Status,
                                     Ordem = entity.Ordem,
                                     Label = entity.Label,
                                     IdParentMenu = entity.IdParentMenu,
                                     IdMainMenu = entity.IdMainMenu,
                                     IdAplicacao = entity.IdAplicacao,
                                     Aplicacao = aplicacao
                                 })
                                .OrderBy(a => a.Ordem)
                                .ToList();
                return structure;
            }
        }

        /// <summary>
        /// Retorna a lista de cockpits
        /// </summary>
        /// <returns></returns>
        private RetornarCockpitResult RetornarCockpit()
        {
            using (var context = new DatabaseContext())
            {
                var cockpit = context.Aplicacao.FirstOrDefault(a => a.IdAplicacao == (int)AplicacaoEnum.Neutro);
                if (cockpit == null) return null;

                var result = new RetornarCockpitResult()
                {
                    Componente = cockpit.Componente,
                    DataHoraCadastro = cockpit.DataHoraCadastro,
                    Descricao = cockpit.Descricao,
                    IdAplicacao = cockpit.IdAplicacao,
                    IdAplicacaoPai = cockpit.IdAplicacaoPai,
                    Multiplo = cockpit.Multiplo,
                    Nome = cockpit.Nome,
                    Permissao = cockpit.Permissao,
                    Status = cockpit.Status
                };
                result.Aplicacao_Filha.AddRange(context.Aplicacao.Where(a => a.IdAplicacaoPai == cockpit.IdAplicacao));
                return result;
            }
        }
        #endregion

        /// <summary>
        /// Retorna todas as aplicações que compõe o sistema, bem como suas permissões.
        /// </summary>
        /// <returns>Lista de aplicações com suas permissões</returns>
        public List<GetAplicacoesResult> GetAplicacoes()
        {
            using (var context = new DatabaseContext())
            {
                var result = new List<GetAplicacoesResult>();
                var listEntity = context.Aplicacao.Where(a => a.Status == (int)DefaultStatusEnum.Ativo).ToList();

                foreach (var item in listEntity)
                {
                    result.Add(new GetAplicacoesResult {
                        IdAplicacao = item.IdAplicacao,
                        Nome = item.Nome,
                        Permissoes = context.Aplicacao_Permissao.Where(a => a.IdAplicacao == item.IdAplicacao).ToList()
                    });
                }

                return result.OrderBy(a => a.Nome).ToList();
            }
        }
    }
}
