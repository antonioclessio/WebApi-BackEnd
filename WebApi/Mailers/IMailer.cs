using Entities.Entity.Table;
using Entities.Entity.Usuario;
using Mvc.Mailer;

namespace WebApi.Mailers
{
    public interface IMailer
    {
        MvcMailMessage NovoUsuario(UsuarioForm entity);
        MvcMailMessage ResetarSenha(Usuario usuario);
    }
}