using System;

namespace Entities.Entity.Feriado
{
    public class FeriadoFilterQuery : BaseFilterModel
    {
        public override bool IsEmpty => Status.HasValue == false
                                     && Tipo.HasValue == false
                                     && string.IsNullOrEmpty(Nome)
                                     && Data.HasValue == false
                                     && IdEstado.HasValue == false
                                     && IdCidade.HasValue == false;

        public byte? Status { get; set; }
        public byte? Tipo { get; set; }
        public string Nome { get; set; }
        public DateTime? Data { get; set; }
        public int? IdEstado { get; set; }
        public int? IdCidade { get; set; }
    }
}
