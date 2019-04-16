namespace Entities.Entity.Profissao
{
    public class ProfissaoForm : BaseFormModel
    {
        public int IdProfissao { get; set; }
        public byte Status { get; set; }
        public string Nome { get; set; }
    }
}
