namespace Entities.Entity.Security
{
    public class AuthenticationQuery
    {
        /// <summary>
        /// Construtor que recebe os parâmetros para a busca.
        /// </summary>
        /// <param name="Username">Login</param>
        /// <param name="Password">Senha</param>
        /// <param name="IdClinica">Clínica selecionada</param>
        public AuthenticationQuery(string Username, string Password, int IdClinica)
        {
            this.Username = Username;
            this.Password = Password;
            this.IdClinica = IdClinica;
        }

        /// <summary>
        /// Login
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Senha
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Clínica selecionada
        /// </summary>
        public int IdClinica { get; set; }
    }
}
