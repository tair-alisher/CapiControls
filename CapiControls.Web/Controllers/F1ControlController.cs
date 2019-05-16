using CapiControls.BLL.Interfaces;
using CapiControls.Controls.Interfaces.Form1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CapiControls.Web.Controllers
{
    public class F1ControlController : ControlController
    {
        private readonly IF1Controls _f1Controls;

        public F1ControlController(
            IFileService fileService,
            IRegionService regionService,
            IQuestionnaireService questionnaireService,
            IF1Controls f1Controls) : base(fileService, regionService, questionnaireService)
        {
            _f1Controls = f1Controls;
        }

        [HttpGet]
        [Authorize(Policy = "IsUser")]
        public IActionResult F1R2HhMembers()
        {
            ViewBag.Questionnaires = GetQuestionnairesSelectList(IndividualTitle);
            ViewBag.Regions = GetRegionsSelectList();

            return View();
        }

        [HttpPost]
        [Authorize(Policy = "IsUser")]
        public async Task<IActionResult> F1R2HhMembers(string questionnaireId, string region)
        {
            string filePath = await _f1Controls.ExecuteF1R2HhMembersControl(questionnaireId, region);
            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

            return File(fileBytes, "applicaiton/msword", $"F1R2HhMembers-{region ?? "all"}.docx");
        }
    }
}