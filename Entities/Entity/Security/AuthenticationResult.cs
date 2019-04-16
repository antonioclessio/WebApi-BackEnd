namespace Entities.Entity.Security
{
    public class AuthenticationResult
    {
        public int IdUsuario { get; set; }
        public string Nome { get; set; }
        public bool PrimeiroAcesso { get; set; }
        public int IdUsuarioGrupo { get; set; }

        public int IdClinica { get; set; }
    }
}
