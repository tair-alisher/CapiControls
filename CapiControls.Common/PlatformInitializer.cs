using CapiControls.BLL.Interfaces;
using CapiControls.BLL.Services;
using CapiControls.Common.Interfaces;
using CapiControls.DAL.Interfaces.Units;
using CapiControls.DAL.Units;
using Microsoft.Extensions.Configuration;
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
            services.AddSingleton(Mapping.Configuration.CreateDefaultMapper());

            services.AddScoped<ILocalUnitOfWork>(localUnit => new LocalUnitOfWork(
                Configuration.GetConnectionString("LocalConnection"))
            );
            services.AddScoped<IRemoteUnitOfWork>(remoteUnit => new RemoteUnitOfWork(
                Configuration.GetConnectionString("RemoteConnection"))
            );

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IQuestionnaireService, QuestionnaireService>();
            services.AddScoped<IGroupService, GroupService>();
            services.AddScoped<IFileService, FileService>();
        }
    }
}
