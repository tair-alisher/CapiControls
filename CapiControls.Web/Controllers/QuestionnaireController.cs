using CapiControls.BLL.DTO;
using CapiControls.BLL.Interfaces;
using CapiControls.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;

namespace CapiControls.Web.Controllers
{
    public class QuestionnaireController : Controller
    {
        private readonly IQuestionnaireService _questionnaireService;
        private readonly IGroupService _groupService;

        public QuestionnaireController(
            IQuestionnaireService questionnaireService,
            IGroupService groupService
        )
        {
            _questionnaireService = questionnaireService;
            _groupService = groupService;
        }

        [Authorize(Policy = "IsUser")]
        public IActionResult Index(int page = 1)
        {
            int pageSize = 10;
            int count = _questionnaireService.CountQuestionnaires();
            var questionnaires = _questionnaireService.GetQuestionnaires(page, pageSize);

            var pageViewModel = new PageViewModel(count, page, pageSize);
            var listModel = new ListViewModel<QuestionnaireDTO>
            {
                PageViewModel = pageViewModel,
                Items = questionnaires
            };

            return View(listModel);
        }

        [HttpGet]
        [Authorize(Policy = "IsUser")]
        public IActionResult Create()
        {
            ViewBag.Groups = new SelectList(_groupService.GetGroups(), "Id", "Title");
            return View();
        }

        [HttpPost]
        [Authorize(Policy = "IsUser")]
        public IActionResult Create(QuestionnaireDTO questionnaire)
        {
            if (ModelState.IsValid)
            {
                _questionnaireService.AddQuestionnaire(questionnaire);
                return RedirectToAction("Index");
            }

            ViewBag.Groups = new SelectList(_groupService.GetGroups(), "Id", "Title", questionnaire.GroupId);
            return View(questionnaire);
        }

        [Authorize(Policy = "IsUser")]
        public IActionResult Details(Guid id)
        {
            var questionnaire = _questionnaireService.GetQuestionnaire(id);
            return View(questionnaire);
        }

        [HttpGet]
        [Authorize(Policy = "IsUser")]
        public IActionResult Edit(Guid id)
        {
            var questionnaire = _questionnaireService.GetQuestionnaire(id);
            ViewBag.Groups = new SelectList(_groupService.GetGroups(), "Id", "Title", questionnaire.GroupId);

            return View(questionnaire);
        }

        [HttpPost]
        [Authorize(Policy = "IsUser")]
        public IActionResult Edit(QuestionnaireDTO questionnaire)
        {
            if (ModelState.IsValid)
            {
                _questionnaireService.UpdateQuestionnaire(questionnaire);
                return RedirectToAction("Index");
            }

            ViewBag.Groups = new SelectList(_groupService.GetGroups(), "Id", "Title", questionnaire.GroupId);
            return View(questionnaire);
        }

        [HttpGet]
        [Authorize(Policy = "IsUser")]
        public IActionResult Delete(Guid id)
        {
            if (_questionnaireService.GetQuestionnaire(id) != null)
                _questionnaireService.DeleteQuestionnaire(id);

            return RedirectToAction("Index");
        }
    }
}