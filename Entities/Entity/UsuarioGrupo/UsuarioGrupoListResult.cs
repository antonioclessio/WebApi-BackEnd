using Entities.Enum;

namespace Entities.Entity.UsuarioGrupo
{
    public class UsuarioGrupoListResult : BaseListModel
    {
        public int IdUsuarioGrupo { get; set; }
        public string Nome { get; set; }
        public byte Status { get; set; }

        public string DescricaoStatus { get { return System.Enum.GetName(typeof(DefaultStatusEnum), (int)Status); } }
    }
}
