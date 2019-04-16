using Entities.Entity;
using Entities.Interfaces.V1;
using System.Collections.Generic;

namespace Business.Interfaces.V1
{
    /// <summary>
    /// Interface responsável por implementar o CRUD no padrão.
    /// </summary>
    /// <typeparam name="TEntity">Entidade que representa a tabela no contexto.</typeparam>
    /// <typeparam name="TListView">Entidade que retorna a lista básica de dados</typeparam>
    /// <typeparam name="TDetailView">Entidade que retorna os detalhes de um determinado registro</typeparam>
    /// <typeparam name="TFilterView">Entidade que contém as propriedades que formarão os critérios de pesquisa</typeparam>
    /// <typeparam name="TFormView">Entidade que contém as propriedades necessárias para manipulação dos dados.</typeparam>
    public interface IRepository<TEntity, TListView, TDetailView, TFilterView, TFormView> : IBaseRepository
        where TEntity : ITable
        where TListView : BaseListModel
        where TDetailView : BaseDetailModel
        where TFilterView : BaseFilterModel
    {
        /// <summary>
        /// Retorna a lista de dados com base nos filtros informados.
        /// </summary>
        /// <param name="filterView">Critérios de pesquisa</param>
        /// <returns>Lista de dados</returns>
        List<TListView> GetList(TFilterView filterView);

        /// <summary>
        /// Retorna os detalhes de um determinado registro. Não necessariamente retorna todos os campos, apenas os necessários para compor a tela de detalhes.
        /// </summary>
        /// <param name="key">Chave de pesquisa do registro.</param>
        /// <returns>Entidade alimentada</returns>
        TDetailView GetByKey(int key);

        /// <summary>
        /// Retorna todos os dados de um determinado registro, sem exceções.
        /// </summary>
        /// <param name="key">Chave de pesquisa do registro.</param>
        /// <returns>Entidade alimentada</returns>
        TEntity GetByKeyFull(int key);

        /// <summary>
        /// Retorna os dados necessários para compor a tela de cadastro.
        /// </summary>
        /// <param name="key">Chave de pesquisa do registro.</param>
        /// <returns>Entidade alimentada</returns>
        TFormView GetForEdit(int key);

        /// <summary>
        /// Salva um registro no banco de dados.
        /// </summary>
        /// <param name="entity">Entidade contendo os dados que são salvos</param>
        /// <returns>Registro salvo com sucesso ou não</returns>
        bool Save(TFormView entity);

        /// <summary>
        /// Remove um determinado registro da base de dados.
        /// </summary>
        /// <param name="key">Chave de pesquisa do registro.</param>
        /// <returns>Registro excluido com sucesso ou não</returns>
        bool Delete(int key);
    }
}
