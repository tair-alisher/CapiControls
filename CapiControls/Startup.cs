using CapiControls.Controls.Form3;
using CapiControls.Controls.Interfaces;
using CapiControls.Data.Interfaces;
using CapiControls.Data.Repositories.Local;
using CapiControls.Data.Repositories.Server;
using CapiControls.Services.Interfaces;
using CapiControls.Services.Local;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CapiControls
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            // Repositories
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddTransient<IInterviewRepository, InterviewRepository>();
            services.AddTransient<IForm3Repository, Form3Repository>();
            services.AddTransient<IQuestionnaireRepository, QuestionnaireRepository>();
            services.AddTransient<IGroupRepository, GroupRepository>();

            // Services
            services.AddTransient<IF3ControlService, F3ControlService>();
            services.AddTransient<IFileService, FileService>();

            // Controls
            services.AddTransient<IF3R1UnitsControl, F3R1UnitsControl>();
            services.AddTransient<IF3R2UnitsControl, F3R2UnitsControl>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Control}/{action=Index}/{id?}");
            });
        }
    }
}
