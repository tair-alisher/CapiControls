using CapiControls.Data.Interfaces;
using CapiControls.Models.Local;
using CapiControls.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;

namespace CapiControls.Controllers
{
    public class QuestionnaireController : Controller
    {
        private readonly IPaginatedRepository<Questionnaire> questRepository;
        private readonly IPaginatedRepository<Group> groupRepository;

        public QuestionnaireController(IPaginatedRepository<Questionnaire> questRepo, IPaginatedRepository<Group> groupRepo)
        {
            questRepository = questRepo;
            groupRepository = groupRepo;
        }

        [Authorize(Policy = "IsUser")]
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
        [Authorize(Policy = "IsUser")]
        public IActionResult Create()
        {
            var groups = groupRepository.GetAll();
            ViewBag.Groups = new SelectList(groups, "Id", "Title");

            return View();
        }

        [HttpPost]
        [Authorize(Policy = "IsUser")]
        public IActionResult Create(Questionnaire questionnaire)
        {
            if (ModelState.IsValid)
            {
                questRepository.Create(questionnaire);
                return RedirectToAction("Index");
            }

            var groups = groupRepository.GetAll();
            ViewBag.Groups = new SelectList(groups, "Id", "Title", questionnaire.GroupId);

            return View();
        }

        [Authorize(Policy = "IsUser")]
        public IActionResult Details(Guid id)
        {
            var questionnaire = questRepository.Get(id);
            return View(questionnaire);
        }

        [HttpGet]
        [Authorize(Policy = "IsUser")]
        public IActionResult Edit(Guid id)
        {
            var questionnaire = questRepository.Get(id);
            var groups = groupRepository.GetAll();
            ViewBag.Groups = new SelectList(groups, "Id", "Title", questionnaire.GroupId);

            return View(questionnaire);
        }

        [HttpPost]
        [Authorize(Policy = "IsUser")]
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
        [Authorize(Policy = "IsUser")]
        public IActionResult Delete(Guid id)
        {
            if (questRepository.Get(id) != null)
                questRepository.Delete(id);

            return RedirectToAction("Index");
        }
    }
}