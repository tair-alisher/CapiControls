using CapiControls.BLL.Interfaces;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using System.Linq;

namespace CapiControls.BLL.Services
{
    public class FileService : IFileService
    {
        private readonly IHostingEnvironment _hostEnv;

        public FileService(IHostingEnvironment hostEnv)
        {
            _hostEnv = hostEnv;
        }

        public void DeleteOldFiles()
        {
            string filesDirectoryPath = Path.Combine(_hostEnv.ContentRootPath, "Files", "Reports");
            var filesDirectory = new DirectoryInfo(filesDirectoryPath);
            var files = filesDirectory.GetFiles("*.*").Where(
                f => f.CreationTime < DateTime.Now.AddHours(-1)
            );

            foreach (var file in files)
                if (File.Exists(file.FullName))
                    File.Delete(file.FullName);
        }
    }
}
