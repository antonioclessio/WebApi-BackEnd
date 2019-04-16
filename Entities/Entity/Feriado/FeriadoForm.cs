using System;

namespace Entities.Entity.Feriado
{
    public class FeriadoForm : BaseFormModel
    {
        public int IdFeriado { get; set; }
        public byte Status { get; set; }
        public string Nome { get; set; }
        public DateTime Data { get; set; }
        public byte Tipo { get; set; }
        public int? IdEstado { get; set; }
        public int? IdCidade { get; set; }

        public string NomeCidade { get; set; }
    }
}
