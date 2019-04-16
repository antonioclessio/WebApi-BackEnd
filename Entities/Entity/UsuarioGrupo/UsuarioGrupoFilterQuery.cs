namespace Entities.Entity.UsuarioGrupo
{
    public class UsuarioGrupoFilterQuery : BaseFilterModel
    {
        public override bool IsEmpty
        {
            get
            {
                return !Status.HasValue
                    && string.IsNullOrEmpty(Nome);
            }
        }

        public byte? Status { get; set; }
        public string Nome { get; set; }
    }
}
