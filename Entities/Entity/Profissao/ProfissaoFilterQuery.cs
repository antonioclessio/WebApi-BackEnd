namespace Entities.Entity.Profissao
{
    public class ProfissaoFilterQuery : BaseFilterModel
    {
        public override bool IsEmpty => string.IsNullOrEmpty(Nome) && Status.HasValue == false;
        public string Nome { get; set; }
        public byte? Status { get; set; }
    }
}
