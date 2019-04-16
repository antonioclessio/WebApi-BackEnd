using Entities.Enum;
using System;

namespace Entities.Entity.Funcionario
{
    public class FuncionarioDetailResult : BaseDetailModel
    {
        public int IdFuncionario { get; set; }
        public byte Status { get; set; }
        public int? IdUsuario { get; set; }
        public int? IdClinica { get; set; }
        public string Nome { get; set; }
        public int? IdLocalizacaoGeografica { get; set; }
        public string NumeroEndereco { get; set; }
        public byte? EstadoCivil { get; set; }
        public string CPF { get; set; }
        public string RG { get; set; }
        public DateTime? DataNascimento { get; set; }
        public DateTime? DataAdmissao { get; set; }
        public DateTime? DataDesligamento { get; set; }
        public string CarteiraProfissionalNro { get; set; }
        public string CarteiraProfissionalSerie { get; set; }
        public string Observacao { get; set; }
        public string Complemento { get; set; }

        public string DescricaoStatus { get { return System.Enum.GetName(typeof(DefaultStatusEnum), (int)Status); } }
        public string CPFFormatado
        {
            get
            {
                if (string.IsNullOrEmpty(CPF)) return string.Empty;

                return Convert.ToUInt64(CPF).ToString(@"000\.000\.000\-00");
            }
        }
    }
}
