using CapiControls.Data.Interfaces;
using CapiControls.Models.Local;
using CapiControls.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;

namespace CapiControls.Controllers
{
    public class GroupController : Controller
    {
        private readonly IGroupRepository repository;

        public GroupController(IGroupRepository repository)
        {
            this.repository = repository;
        }

        public IActionResult Index(int page = 1)
        {
            int pageSize = 10;
            int count = repository.CountAll();
            var groups = repository.GetAll(pageSize, page);

            var pageViewModel = new PageViewModel(count, page, pageSize);
            var viewModel = new IndexViewModel<Group>
            {
                PageViewModel = pageViewModel,
                Items = groups
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Group group)
        {
            if (ModelState.IsValid)
            {
                repository.Create(group);
                return RedirectToAction("Index");
            }

            return View();
        }

        public IActionResult Details(Guid id)
        {
            var group = repository.Get(id);
            return View(group);
        }

        [HttpGet]
        public IActionResult Edit(Guid id)
        {
            var group = repository.Get(id);
            return View(group);
        }

        [HttpPost]
        public IActionResult Edit(Group group)
        {
            if (ModelState.IsValid)
            {
                repository.Update(group);
                return RedirectToAction("Index");
            }

            return View();
        }

        [HttpGet]
        public IActionResult Delete(Guid id)
        {
            if (repository.Get(id) != null)
            {
                repository.Delete(id);
            }

            return RedirectToAction("Index");
        }
    }
}