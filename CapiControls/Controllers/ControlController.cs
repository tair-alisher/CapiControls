using CapiControls.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CapiControls.Controllers
{
    public class ControlController : Controller
    {
        public string HouseholdTitle = "Household";
        public string IndividualTitle = "Individual";

        public ControlController(IFileService fileService)
        {
            fileService.DeleteOldFiles();
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}