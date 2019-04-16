using Entities.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Entity.Funcionario
{
    [NotMapped]
    public class FuncionarioTelefoneDetail : Table.Funcionario_Telefone
    {
        public string DescricaoTipo { get { return System.Enum.GetName(typeof(TipoTelefoneEnum), (int)Tipo); } }
    }
}
