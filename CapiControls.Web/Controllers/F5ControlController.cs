using CapiControls.BLL.Interfaces;
using CapiControls.Controls.Interfaces.Form5;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CapiControls.Web.Controllers
{
    public class F5ControlController : ControlController
    {
        private readonly IF5Controls _f5Controls;

        public F5ControlController(
            IFileService fileService,
            IRegionService regionService,
            IQuestionnaireService questionnaireService,
            IF5Controls f5Controls) : base(fileService, regionService, questionnaireService)
        {
            _f5Controls = f5Controls;
        }

        [HttpGet]
        [Authorize(Policy = "IsUser")]
        public IActionResult F5Units()
        {
            ViewBag.Questionnaires = GetQuestionnairesSelectList(HouseholdTitle);
            ViewBag.Regions = GetRegionsSelectList();

            return View();
        }

        [HttpPost]
        [Authorize(Policy = "IsUser")]
        public IActionResult F5Units(string questionnaireId, string region)
        {
            string filePath = _f5Controls.ExecuteF5Controls(questionnaireId, region);
            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

            return File(fileBytes, "application/msword", $"F5Units-{region ?? "all"}.docx");
        }
    }
}