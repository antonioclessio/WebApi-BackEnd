namespace Entities.Entity
{
    public abstract class BaseFilterModel
    {
        /// <summary>
        /// Verifica se a classe está vazia, ou seja, se as propriedades estão sem valores. Sobrescreva esta propriedade caso haja necessidade de customização.
        /// </summary>
        public virtual bool IsEmpty
        {
            get
            {
                return true;
            }
        }
    }
}
