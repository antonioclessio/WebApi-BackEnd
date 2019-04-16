using System;
using System.Linq;
using System.Collections.Generic;
using GrupoOrto.ERP.Business.Interfaces;
using GrupoOrto.ERP.Entities.Entity.ContaReceber;
using GrupoOrto.ERP.Entities.Entity.Table;
using GrupoOrto.ERP.Entities.Enum;

namespace GrupoOrto.ERP.Business.Repositories
{
    public class ContaReceberRepository : BaseRepository, IRepository<ContaReceber, ContaReceberListResult, ContaReceberDetailResult, ContaReceberFilterQuery, ContaReceberForm>
    {
        protected override int IdAplicacao { get => (int)AplicacaoEnum.Neutro; }

        #region # Interface
        public bool Delete(int key)
        {
            using (var context = new DatabaseContext())
            {
                throw new NotImplementedException();
            }
        }

        public ContaReceberDetailResult GetByKey(int key)
        {
            using (var context = new DatabaseContext())
            {
                throw new NotImplementedException();
            }
        }

        public ContaReceber GetByKeyFull(int key)
        {
            using (var context = new DatabaseContext())
            {
                throw new NotImplementedException();
            }
        }

        public ContaReceberForm GetForEdit(int key)
        {
            using (var context = new DatabaseContext())
            {
                throw new NotImplementedException();
            }
        }

        public List<ContaReceberListResult> GetList(ContaReceberFilterQuery filterView)
        {
            using (var context = new DatabaseContext())
            {
                throw new NotImplementedException();
            }
        }

        public bool Save(ContaReceberForm entity)
        {
            using (var context = new DatabaseContext())
            {
                throw new NotImplementedException();
            }
        }
        #endregion

        /// <summary>
        /// Retorna a lista de vencimentos do dia para a clínica logada
        /// </summary>
        /// <returns></returns>
        public List<VencimentosDiaResult> GetVencimentosDia()
        {
            using (var context = new DatabaseContext())
            {
                var dataInicial = new DateTime(CurrentDateTime.Year, CurrentDateTime.Month, CurrentDateTime.Day, 0, 0, 0);
                var dataFinal = new DateTime(CurrentDateTime.Year, CurrentDateTime.Month, CurrentDateTime.Day, 23, 59, 59);
                var listEntity = (from contaReceber in context.ContaReceber
                                  join paciente in context.Paciente on contaReceber.IdPaciente equals paciente.IdPaciente
                                  join pessoa in context.Pessoa on paciente.IdPessoa equals pessoa.IdPessoa
                                  join clinica in context.Clinica on paciente.IdClinica equals IdClinicaLogada
                                  where contaReceber.DataVencimento >= dataInicial && contaReceber.DataVencimento <= dataFinal
                                  select new VencimentosDiaResult
                                  {
                                      Nome = pessoa.Nome,
                                      Recebido = contaReceber.DataPagamento.HasValue,
                                      ValorParcela = contaReceber.ValorParcela
                                  }).ToList();

                return listEntity;
            }
        }
    }
}
