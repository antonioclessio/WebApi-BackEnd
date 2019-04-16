using Entities.Enum;
using System;

namespace Entities.Entity.Funcionario
{
    public class FuncionarioListResult : BaseListModel
    {
        public int IdFuncionario { get; set; }
        public string Nome { get; set; }
        public string CPF { get; set; }
        public byte Status { get; set; }

        public string CPFFormatado
        {
            get
            {
                return Convert.ToUInt64(CPF).ToString(@"000\.000\.000\-00");
            }
        }

        public string DescricaoStatus { get { return System.Enum.GetName(typeof(DefaultStatusEnum), (int)Status); } }
    }
}