namespace GrupoOrto.ERP.Entities.Interfaces
{
    /// <summary>
    /// Classe mais baixa na estrutura de objetos. Permite polimorfismo.
    /// </summary>
    public interface IBaseView
    {
        /// <summary>
        /// Valida as propriedades da classe
        /// </summary>
        /// <returns></returns>
        bool isValid();
    }
}
