using CapiControls.Data.Interfaces;
using CapiControls.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CapiControls.Controllers
{
    public class F3ControlController : ControlController
    {
        private readonly IGroupRepository groupRepository;
        private readonly IQuestionnaireRepository questRepository;
        private readonly IF3ControlService F3ControlService;

        public F3ControlController(IGroupRepository groupRepo, IQuestionnaireRepository questRepo, IF3ControlService f3ControlService, IFileService fileService) : base(fileService)
        {
            groupRepository = groupRepo;
            questRepository = questRepo;
            F3ControlService = f3ControlService;
        }

        [HttpGet]
        public IActionResult F3R1Units()
        {
            ViewBag.Questionnaires = GetQuestionnairesSelectList();
            return View();
        }

        [HttpPost]
        public IActionResult F3R1Units(string questionnaireId)
        {
            string filePath = F3ControlService.ExecuteF3R1UnitsControl(questionnaireId);
            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

            return File(fileBytes, "text/csv", "F3R1Units.csv");
        }

        [HttpGet]
        public IActionResult F3R2Units()
        {
            ViewBag.Questionnaires = GetQuestionnairesSelectList();
            return View();
        }

        [HttpPost]
        public IActionResult F3R2Units(string questionnaireId)
        {
            string filePath = F3ControlService.ExecuteF3R2UnitsControl(questionnaireId);
            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

            return File(fileBytes, "text/csv", "F3R2Units.csv");
        }

        private SelectList GetQuestionnairesSelectList()
        {
            var questionnaires = F3ControlService.GetQuestionnairesByGroupName(base.HouseholdTitle);
            return new SelectList(questionnaires, "Identifier", "Title");
        }
    }
}