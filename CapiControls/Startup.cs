using CapiControls.Controls.Form3;
using CapiControls.Controls.Interfaces;
using CapiControls.Data.Interfaces;
using CapiControls.Data.Repositories.Local;
using CapiControls.Data.Repositories.Server;
using CapiControls.Models.Local;
using CapiControls.Models.Local.Account;
using CapiControls.Services.Interfaces;
using CapiControls.Services.Local;
using Microsoft.AspNetCore.Authentication.Cookies;
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

            services.AddSingleton<IConfiguration>(Configuration);
            //services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Repositories
            services.AddTransient<IInterviewRepository, InterviewRepository>();
            services.AddTransient<IForm3Repository, Form3Repository>();
            services.AddTransient<IPaginatedRepository<Questionnaire>, QuestionnaireRepository>();
            services.AddTransient<IPaginatedRepository<Group>, GroupRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IRepository<Region>, RegionRepository>();

            // Services
            services.AddTransient<IBaseControlService, BaseControlService>();
            services.AddTransient<IF3ControlService, F3ControlService>();
            services.AddTransient<IFileService, FileService>();
            services.AddTransient<IUserService, UserService>();

            // Controls
            services.AddTransient<IF3R1UnitsControl, F3R1UnitsControl>();
            services.AddTransient<IF3R2UnitsControl, F3R2UnitsControl>();

            // Authentication
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(
                options => options.LoginPath = new PathString("/account/login")
            );

            // Authorization
            services.AddAuthorization(options =>
            {
                options.AddPolicy("IsAdministrator", policy => policy.RequireClaim("IS_ADMIN", "true"));
                options.AddPolicy("IsUser", policy => policy.RequireClaim("IS_USER", "true"));
            });

            services.AddMvc(
                options => options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute())
            ).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
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
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Control}/{action=Index}/{id?}");
            });
        }
    }
}
