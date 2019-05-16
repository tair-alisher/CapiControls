using CapiControls.BLL.Interfaces;
using CapiControls.BLL.Services;
using CapiControls.Common.Interfaces;
using CapiControls.Controls.Controls.Form1;
using CapiControls.Controls.Controls.Form3;
using CapiControls.Controls.Interfaces.Form1;
using CapiControls.Controls.Interfaces.Form3;
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
            services.AddScoped<IRegionService, RegionService>();

            services.AddScoped<IInterviewService, InterviewService>();
            services.AddScoped<IForm3Service, Form3Service>();
            services.AddScoped<IForm1Service, Form1Service>();

            services.AddScoped<IF3R1UnitsControl, F3R1UnitsControl>();
            services.AddScoped<IF3R2UnitsControl, F3R2unitsControl>();
            services.AddScoped<IF3Controls, F3Controls>();

            services.AddScoped<IF1R2HhMembersControl, F1R2HhMembersControl>();
            services.AddScoped<IF1Controls, F1Controls>();
        }
    }
}
