using Entities.Entity.Table;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Entity.Aplicacao
{
    [NotMapped]
    public class MenuByParentResult : AppMainMenu
    {
        public Table.Aplicacao Aplicacao { get; set; }
        
        public List<AppMainMenu> Itens { get; set; } = new List<AppMainMenu>();
    }
}
