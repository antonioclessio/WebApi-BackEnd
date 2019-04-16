using System.Collections.Generic;

namespace Entities.Entity.UsuarioGrupo
{
    /// <summary>
    /// Resultado da consulta que retorna um grupo de usuário filtrando por usuário e clínica.
    /// </summary>
    public class RetornarPorUsuarioClinicaResult
    {
        /// <summary>
        /// Grupo encontrado
        /// </summary>
        public Table.UsuarioGrupo Grupo { get; set; }

        /// <summary>
        /// Permissões associadas ao grupo encontrado
        /// </summary>
        public List<Table.UsuarioGrupo_Permissao> Permissoes { get; set; }
    }
}
