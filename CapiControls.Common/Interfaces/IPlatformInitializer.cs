using Microsoft.Extensions.DependencyInjection;

namespace CapiControls.Common.Interfaces
{
    public interface IPlatformInitializer
    {
        void ConfigureServices(IServiceCollection services);
    }
}
