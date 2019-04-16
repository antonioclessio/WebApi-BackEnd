using Entities.Interfaces.V1;

namespace Entities.Entity
{
    public abstract class BaseDetailModel : IBaseView
    {
        /// <summary>
        /// Retorna se a entidade está valida. Sobrescreva este método para customizar a validação caso necessário.
        /// </summary>
        /// <returns>True / false</returns>
        public virtual bool isValid()
        {
            return true;
        }
    }
}
