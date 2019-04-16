namespace Entities.Entity.Usuario
{
    public class UsuarioFilterQuery : BaseFilterModel
    {
        public override bool IsEmpty
        {
            get
            {
                return !Status.HasValue
                    && string.IsNullOrEmpty(Nome)
                    && string.IsNullOrEmpty(Email)
                    && !IdClinica.HasValue;
            }
        }

        public byte? Status { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public int? IdClinica { get; set; }
    }
}
