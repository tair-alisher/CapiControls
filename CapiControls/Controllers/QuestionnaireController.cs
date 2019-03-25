using CapiControls.Data.Interfaces;
using CapiControls.Models.Local;
using CapiControls.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;

namespace CapiControls.Controllers
{
    public class QuestionnaireController : Controller
    {
        private readonly IQuestionnaireRepository questRepository;
        private readonly IGroupRepository groupRepository;

        public QuestionnaireController(IQuestionnaireRepository questRepo, IGroupRepository groupRepo)
        {
            questRepository = questRepo;
            groupRepository = groupRepo;
        }

        public IActionResult Index(int page = 1)
        {
            int pageSize = 10;
            int count = questRepository.CountAll();
            var questionnaires = questRepository.GetAll(pageSize, page);

            var pageViewModel = new PageViewModel(count, page, pageSize);
            var viewModel = new IndexViewModel<Questionnaire>
            {
                PageViewModel = pageViewModel,
                Items = questionnaires
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var groups = groupRepository.GetAll();
            ViewBag.Groups = new SelectList(groups, "Id", "Title");
            return View();
        }

        [HttpPost]
        public IActionResult Create(Questionnaire questionnaire)
        {
            if (ModelState.IsValid)
            {
                questRepository.Create(questionnaire);
                return RedirectToAction("Index");
            }

            return View();
        }

        public IActionResult Details(Guid id)
        {
            var questionnaire = questRepository.Get(id);
            return View(questionnaire);
        }

        [HttpGet]
        public IActionResult Edit(Guid id)
        {
            var questionnaire = questRepository.Get(id);
            var groups = groupRepository.GetAll();
            ViewBag.Groups = new SelectList(groups, "Id", "Title", questionnaire.GroupId);

            return View(questionnaire);
        }

        [HttpPost]
        public IActionResult Edit(Questionnaire questionnaire)
        {
            if (ModelState.IsValid)
            {
                questRepository.Update(questionnaire);
                return RedirectToAction("index");
            }

            return View();
        }

        [HttpGet]
        public IActionResult Delete(Guid id)
        {
            if (questRepository.Get(id) != null)
                questRepository.Delete(id);

            return RedirectToAction("Index");
        }
    }
}