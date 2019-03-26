using CapiControls.Data.Interfaces;
using CapiControls.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CapiControls.Controllers
{
    public class ControlController : BaseController
    {
        private readonly IGroupRepository groupRepository;
        private readonly IQuestionnaireRepository questRepository;
        private readonly IControlService ControlService;

        public ControlController(IGroupRepository groupRepo, IQuestionnaireRepository questRepo, IControlService controlService)
        {
            groupRepository = groupRepo;
            questRepository = questRepo;
            ControlService = controlService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult F3R1Units()
        {
            var questionnaires = ControlService.GetQuestionnairesByGroupName(base.HouseholdTitle);
            ViewBag.Questionnaires = new SelectList(questionnaires, "Identifier", "Title");
            return View();
        }

        [HttpPost]
        public IActionResult F3R1Units(string questionnaireId)
        {
            ControlService.ExecuteF3R1UnitsControl(questionnaireId);

            return RedirectToAction("Index");
        }
    }
}