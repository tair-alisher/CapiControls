using CapiControls.Data.Interfaces;
using CapiControls.Models.Local;
using CapiControls.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CapiControls.Controllers
{
    public class F3ControlController : ControlController
    {
        private readonly IPaginatedRepository<Group> groupRepository;
        private readonly IPaginatedRepository<Questionnaire> questRepository;
        private readonly IF3ControlService F3ControlService;

        public F3ControlController(
            IPaginatedRepository<Group> groupRepo,
            IPaginatedRepository<Questionnaire> questRepo,
            IBaseControlService baseControlService,
            IF3ControlService f3ControlService,
            IFileService fileService) : base(fileService, baseControlService)
        {
            groupRepository = groupRepo;
            questRepository = questRepo;
            F3ControlService = f3ControlService;
        }

        [HttpGet]
        [Authorize(Policy = "IsUser")]
        public IActionResult F3R1Units()
        {
            ViewBag.Questionnaires = GetQuestionnairesSelectList(base.HouseholdTitle);
            ViewBag.Regions = GetRegionsSelectList();
            return View();
        }

        [HttpPost]
        [Authorize(Policy = "IsUser")]
        public IActionResult F3R1Units(string questionnaireId, string region)
        {
            string filePath = F3ControlService.ExecuteF3R1UnitsControl(questionnaireId, region);
            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

            return File(fileBytes, "application/msword", $"F3R1Units-{region}.docx");
        }

        [HttpGet]
        [Authorize(Policy = "IsUser")]
        public IActionResult F3R2Units()
        {
            ViewBag.Questionnaires = GetQuestionnairesSelectList(base.HouseholdTitle);
            ViewBag.Regions = GetRegionsSelectList();
            return View();
        }

        [HttpPost]
        [Authorize(Policy = "IsUser")]
        public IActionResult F3R2Units(string questionnaireId, string region)
        {
            string filePath = F3ControlService.ExecuteF3R2UnitsControl(questionnaireId, region);
            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

            return File(fileBytes, "application/msword", $"F3R2Units-{region}.docx");
        }

        
    }
}