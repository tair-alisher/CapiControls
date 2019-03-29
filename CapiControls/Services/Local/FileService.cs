using CapiControls.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using System.Linq;

namespace CapiControls.Services.Local
{
    public class FileService : IFileService
    {
        private readonly IHostingEnvironment HostingEnvironment;

        public FileService(IHostingEnvironment hostEnv)
        {
            HostingEnvironment = hostEnv;
        }

        public void DeleteOldFiles()
        {
            string filesPath = Path.Combine(HostingEnvironment.ContentRootPath, "Files", "Reports");
            var directory = new DirectoryInfo(filesPath);
            var files = directory.GetFiles("*.*").Where(f => f.CreationTime < DateTime.Now.AddHours(-1));

            foreach (var file in files)
            {
                if (File.Exists(file.FullName))
                {
                    File.Delete(file.FullName);
                }
            }
        }
    }
}
