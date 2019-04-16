using System;

namespace Entities.Entity.Funcionario
{
    public class FuncionarioFilterQuery : BaseFilterModel
    {
        public override bool IsEmpty => Status.HasValue == false 
                                     && string.IsNullOrEmpty(Nome) 
                                     && string.IsNullOrEmpty(CPF)
                                     && DataAdmissao.HasValue == false
                                     && DataDesligamento.HasValue == false;

        public byte? Status { get; set; }
        public string Nome { get; set; }
        public string CPF { get; set; }
        public DateTime? DataAdmissao { get; set; }
        public DateTime? DataDesligamento { get; set; }
    }
}
