using CapiControls.BLL.Interfaces;
using CapiControls.Controls.Interfaces.Form3;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CapiControls.Web.Controllers
{
    public class F3ControlController : ControlController
    {
        private readonly IF3Controls _f3Controls;

        public F3ControlController(
            IFileService fileService,
            IRegionService regionService,
            IQuestionnaireService questionnaireService,
            IF3Controls f3Controls) : base(fileService, regionService, questionnaireService)
        {
            _f3Controls = f3Controls;
        }

        [HttpGet]
        [Authorize(Policy = "IsUser")]
        public IActionResult F3R1Units()
        {
            ViewBag.Questionnaires = GetQuestionnairesSelectList(HouseholdTitle);
            ViewBag.Regions = GetRegionsSelectList();

            return View();
        }

        [HttpPost]
        [Authorize(Policy = "IsUser")]
        public IActionResult F3R1Units(string questionnaireId, string region)
        {
            string filePath = _f3Controls.ExecuteF3R1UnitsControl(questionnaireId, region);
            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

            return File(fileBytes, "application/msword", $"F3R1Units-{region ?? "all"}.docx");
        }

        [HttpGet]
        [Authorize(Policy = "IsUser")]
        public IActionResult F3R2Units()
        {
            ViewBag.Questionnaires = GetQuestionnairesSelectList(HouseholdTitle);
            ViewBag.Regions = GetRegionsSelectList();

            return View();
        }

        [HttpPost]
        [Authorize(Policy = "IsUser")]
        public IActionResult F3R2Units(string questionnaireId, string region)
        {
            string filePath = _f3Controls.ExecuteF3R2UnitsControl(questionnaireId, region);
            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

            return File(fileBytes, "application/msword", $"F3R2Units-{region ?? "all"}.docx");
        }

        [HttpGet]
        [Authorize(Policy = "IsUser")]
        public IActionResult F3R2SupplySources()
        {
            ViewBag.Questionnaires = GetQuestionnairesSelectList(HouseholdTitle);
            ViewBag.Regions = GetRegionsSelectList();

            return View();
        }

        [HttpPost]
        public IActionResult F3R2SupplySources(string questionnaireId, string region)
        {
            string filePath = _f3Controls.ExecuteF3R2SupplySources(questionnaireId, region);
            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

            return File(fileBytes, "application/msword", $"F3R2SupplySources-{region ?? "all"}.docx");
        }
    }
}