using CapiControls.Common.Interfaces;
using CapiControls.DAL.Interfaces.Units;
using CapiControls.DAL.Units;
using Microsoft.Extensions.DependencyInjection;

namespace CapiControls.Common
{
    public class PlatformInitializer : IPlatformInitializer
    {
        protected IConfiguration Configuration;

        public PlatformInitializer(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<ILocalUnitOfWork>(unit => new LocalUnitOfWork(
                Configuration.GetConnectionString("LocalConnection"))
            );
            services.AddScoped<IRemoteUnitOfWork>(unit => new RemoteUnitOfWork(
                Configuration.GetConnectionString("RemoteConnection"))
            );
        }
    }
}
