using Entities.Enum;

namespace Entities.Entity.Usuario
{
    public class UsuarioListResult : BaseListModel
    {
        public int IdUsuario { get; set; }
        public byte Status { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        
        public string DescricaoStatus { get { return System.Enum.GetName(typeof(DefaultStatusEnum), (int)Status); } }
    }
}
