using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace CapiControls.Controls
{
    public abstract class BaseControl
    {
        private readonly IHostingEnvironment HostingEnvironment;

        public BaseControl(IHostingEnvironment hostEnv)
        {
            HostingEnvironment = hostEnv;
        }

        protected string BuildFilePath(string fileName)
        {
            string rootDir = HostingEnvironment.ContentRootPath;
            return Path.Combine(rootDir, "Files", fileName);
        }
    }
}
