using Entities.Enum;

namespace Entities.Entity.Profissao
{
    public class ProfissaoDetailResult : BaseDetailModel
    {
        public int IdProfissao { get; set; }
        public byte Status { get; set; }
        public string Nome { get; set; }

        public string DescricaoStatus { get { return System.Enum.GetName(typeof(DefaultStatusEnum), (int)Status); } }
    }
}
