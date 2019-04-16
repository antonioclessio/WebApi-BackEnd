using Entities.Enum;
using Entities.Interfaces.V1;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Entity.Table
{
    public class BaseTable : ITable
    {
        internal double FUSO_HORARIO = -3;
        internal DateTime CurrentDateTime { get { return DateTime.Now.AddHours(FUSO_HORARIO); } }

        [DefaultValue(1)]
        public byte Status { get; set; }

        private DateTime? _DataHoraCadastro;
        public DateTime DataHoraCadastro
        {
            get
            {
                if (_DataHoraCadastro == null)
                    _DataHoraCadastro = CurrentDateTime;

                return _DataHoraCadastro.Value;
            }
            set
            {
                _DataHoraCadastro = value;
            }
        }

        private DateTime? _DataHoraAlteracao;
        public DateTime? DataHoraAlteracao
        {
            get
            {
                if (_DataHoraAlteracao == null)
                    _DataHoraAlteracao = CurrentDateTime;

                return _DataHoraAlteracao;
            }
            set
            {
                _DataHoraAlteracao = value;
            }
        }

        public int? IdUsuarioCadastro { get; set; }

        public int? IdUsuarioAlteracao { get; set; }

        [NotMapped]
        public string DescricaoStatus { get { return System.Enum.GetName(typeof(DefaultStatusEnum), (int)Status); } }
    }
}
