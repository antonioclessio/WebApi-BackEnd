using Entities.Enum;
using System;

namespace Entities.Entity.Feriado
{
    public class FeriadoListResult : BaseListModel
    {
        public int IdFeriado { get; set; }
        public byte Status { get; set; }
        public string Nome { get; set; }
        public DateTime Data { get; set; }
        public byte Tipo { get; set; }
        public string Estado { get; set; }
        public string Cidade { get; set; }

        public string DescricaoStatus { get { return System.Enum.GetName(typeof(DefaultStatusEnum), (int)Status); } }
    }
}