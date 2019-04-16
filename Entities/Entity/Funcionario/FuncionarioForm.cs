using Entities.Entity.LocalizacaoGeografica;
using Entities.Entity.Table;
using System;
using System.Collections.Generic;

namespace Entities.Entity.Funcionario
{
    public class FuncionarioForm : BaseFormModel
    {
        public int IdFuncionario { get; set; }
        public byte Status { get; set; }
        public int? IdUsuario { get; set; }
        public int IdClinica { get; set; }
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

        public List<FuncionarioTelefoneDetail> Telefones { get; set; } = new List<FuncionarioTelefoneDetail>();
        public List<FuncionarioEmailDetail> Emails { get; set; } = new List<FuncionarioEmailDetail>();

        public LocalizacaoGeograficaResult LocalizacaoGeografica { get; set; }
    }
}
