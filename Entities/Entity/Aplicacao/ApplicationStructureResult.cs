using Entities.Entity.Table;
using Entities.Entity.Usuario;
using System.Collections.Generic;

namespace Entities.Entity.Aplicacao
{
    /// <summary>
    /// Classe contendo toda a estrutura necessária para organizar e utilizar a aplicação.
    /// </summary>
    public class ApplicationStructureResult
    {
        /// <summary>
        /// Menu principal da aplicação.
        /// </summary>
        public List<AppMainMenu> MainMenu { get; set; } = new List<AppMainMenu>();

        /// <summary>
        /// Contém a estrutura de cockpit permitida para o usuário logado.
        /// </summary>
        public Table.Aplicacao Cockpit { get; set; }

        /// <summary>
        /// Dados do usuário logado
        /// </summary>
        public UsuarioFullWithClinicaResult UsuarioLogado { get; set; }

        /// <summary>
        /// Clínica que está na sessão. Lembrar da estrutura multi empresa.
        /// </summary>
        public Table.Clinica Clinica { get; set; }
    }
}
