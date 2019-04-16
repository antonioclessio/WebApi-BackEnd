using Entities.Entity.Table;
using System.Data.Entity;

namespace Business
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext() : base("name=Entities")
        {
            Configuration.LazyLoadingEnabled = true;
            Configuration.ProxyCreationEnabled = false;
        }

        public DbSet<AppMainMenu> AppMainMenu { get; set; }
        public DbSet<Aplicacao> Aplicacao { get; set; }
        public DbSet<Aplicacao_Permissao> Aplicacao_Permissao { get; set; }
        
        public DbSet<Cidade> Cidade { get; set; }
        public DbSet<LocalizacaoGeografica> LocalizacaoGeografica { get; set; }
        public DbSet<Feriado> Feriado { get; set; }
        public DbSet<Profissao> Profissao { get; set; }

        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<UsuarioGrupo> UsuarioGrupo { get; set; }
        public DbSet<UsuarioGrupo_Permissao> UsuarioGrupo_Permissao { get; set; }
        public DbSet<Usuario_LogAcesso> Usuario_LogAcesso { get; set; }
        public DbSet<Usuario_LogAtividade> Usuario_LogAtividade { get; set; }

        public DbSet<Funcionario> Funcionario { get; set; }
        public DbSet<Funcionario_Email> Funcionario_Email { get; set; }
        public DbSet<Funcionario_Telefone> Funcionario_Telefone { get; set; }
    }
}
