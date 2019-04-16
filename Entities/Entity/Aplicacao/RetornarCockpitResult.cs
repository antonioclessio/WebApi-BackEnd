using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Entity.Aplicacao
{
    [NotMapped]
    public class RetornarCockpitResult : Table.Aplicacao
    {
        public List<Table.Aplicacao> Aplicacao_Filha { get; set; } = new List<Table.Aplicacao>();
    }
}
