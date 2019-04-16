using GrupoOrto.ERP.Entities.Entity;
using GrupoOrto.ERP.Entities.Interfaces;
using System.Web.Http;

namespace GrupoOrto.ERP.WebApi.Interfaces
{
    /// <summary>
    /// Interface responsável por implementar o CRUD no padrão.
    /// </summary>
    /// <typeparam name="TEntity">Entidade que representa a tabela no contexto.</typeparam>
    /// <typeparam name="TFilterView">Entidade que contém as propriedades que formarão os critérios de pesquisa</typeparam>
    /// <typeparam name="TFormView">Entidade que contém as propriedades necessárias para manipulação dos dados.</typeparam>
    public interface IApiController<TEntity, TFilterView, TFormView>
        where TEntity : ITable
        where TFilterView : BaseFilterModel
    {
        /// <summary>
        /// Retorna a lista de dados com base nos filtros informados.
        /// </summary>
        /// <param name="filterView">Critérios de pesquisa</param>
        /// <returns>Lista de dados</returns>
        IHttpActionResult GetList(TFilterView filter);

        /// <summary>
        /// Retorna os detalhes de um determinado registro. Não necessariamente retorna todos os campos, apenas os necessários para compor a tela de detalhes.
        /// </summary>
        /// <param name="key">Chave de pesquisa do registro.</param>
        /// <returns>Entidade alimentada</returns>
        IHttpActionResult GetByKey(int key);

        /// <summary>
        /// Retorna todos os dados de um determinado registro, sem exceções.
        /// </summary>
        /// <param name="key">Chave de pesquisa do registro.</param>
        /// <returns>Entidade alimentada</returns>
        IHttpActionResult GetByKeyFull(int key);

        /// <summary>
        /// Retorna os dados necessários para compor a tela de cadastro.
        /// </summary>
        /// <param name="key">Chave de pesquisa do registro.</param>
        /// <returns>Entidade alimentada</returns>
        IHttpActionResult GetForEdit(int key);

        /// <summary>
        /// Salva um registro no banco de dados.
        /// </summary>
        /// <param name="entity">Entidade contendo os dados que são salvos</param>
        /// <returns>Registro salvo com sucesso ou não</returns>
        IHttpActionResult Post(TFormView entity);

        /// <summary>
        /// Remove um determinado registro da base de dados.
        /// </summary>
        /// <param name="key">Chave de pesquisa do registro.</param>
        /// <returns>Registro excluido com sucesso ou não</returns>
        IHttpActionResult Delete(int key);
    }
}