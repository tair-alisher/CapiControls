using CapiControls.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CapiControls.Controllers
{
    public class ControlController : Controller
    {
        public string HouseholdTitle = "Household";
        public string IndividualTitle = "Individual";

        private readonly IBaseControlService BaseControlService;

        public ControlController(IFileService fileService, IBaseControlService baseControlService)
        {
            fileService.DeleteOldFiles();
            BaseControlService = baseControlService;
        }

        [Authorize(Policy = "IsUser")]
        public IActionResult Index()
        {
            return View();
        }

        protected SelectList GetQuestionnairesSelectList(string groupTitle)
        {
            var questionnaires = BaseControlService.GetQuestionnairesByGroupName(groupTitle);
            return new SelectList(questionnaires, "Identifier", "Title");
        }

        protected SelectList GetRegionsSelectList()
        {
            var regions = BaseControlService.GetRegions();
            return new SelectList(regions, "Name", "Title");
        }
    }
}