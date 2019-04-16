using Entities.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Entity.Funcionario
{
    [NotMapped]
    public class FuncionarioEmailDetail : Table.Funcionario_Email
    {
        public string DescricaoTipo { get { return System.Enum.GetName(typeof(TipoEmailEnum), (int)Tipo); } }
    }
}
