using CapiControls.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CapiControls.Web.Controllers
{
    public class ControlController : Controller
    {
        protected string HouseholdTitle = "Household";
        protected string IndividualTitle = "Individual";

        private readonly IRegionService _regionService;
        private readonly IQuestionnaireService _questionnaireService;

        public ControlController(
            IFileService fileService,
            IRegionService regionService,
            IQuestionnaireService questionnaireService)
        {
            fileService.DeleteOldFiles();
            _regionService = regionService;
            _questionnaireService = questionnaireService;
        }

        [Authorize(Policy = "IsUser")]
        public IActionResult Index()
        {
            return View();
        }

        protected SelectList GetQuestionnairesSelectList(string groupTitle, string selectedValue = null)
        {
            return new SelectList(
                _questionnaireService.GetQuestionnairesByGroupName(groupTitle),
                "Identifier",
                "Title",
                selectedValue
            );
        }

        protected SelectList GetRegionsSelectList(string selectedValue = null)
        {
            return new SelectList(
                _regionService.GetRegions(),
                "Name",
                "Title",
                selectedValue
            );
        }
    }
}