using GrupoOrto.ERP.Business.Interfaces;
using GrupoOrto.ERP.Business.Repositories;
using SimpleInjector;

namespace GrupoOrto.ERP.Business
{
    public static class DIStartup
    {
        public static InstanceProducer[] Config()
        {
            var container = new Container();
            container.Register<IProfissaoRepository, ProfissaoRepository>();
            container.Verify();

            return container.GetRootRegistrations();
        }
    }
}
