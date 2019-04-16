using Entities.Entity.Table;
using Entities.Entity.Usuario;
using Mvc.Mailer;

namespace WebApi.Mailers
{
    public class Mailer : MailerBase, IMailer
    {
        public Mailer()
        {
            MasterName = "_Layout";
        }

        public virtual MvcMailMessage NovoUsuario(UsuarioForm view)
        {
            ViewData.Model = view;
            MasterName = "_LayoutMailer";

            return Populate(p =>
            {
                p.Subject = "ERP - Novo Usuário";
                p.ViewName = "NovoUsuario";
                p.IsBodyHtml = true;
                p.To.Add(view.Email);
            });
        }

        public virtual MvcMailMessage ResetarSenha(Usuario view)
        {
            ViewData.Model = view;
            MasterName = "_LayoutMailer";

            return Populate(p =>
            {
                p.Subject = "ERP - Resetar Senha";
                p.ViewName = "ResetarSenha";
                p.IsBodyHtml = true;
                p.To.Add(view.Email);
            });
        }
    }
}