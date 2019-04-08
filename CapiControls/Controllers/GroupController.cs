using CapiControls.Data.Interfaces;
using CapiControls.Models.Local;
using CapiControls.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace CapiControls.Controllers
{
    public class GroupController : Controller
    {
        private readonly IPaginatedRepository<Group> repository;

        public GroupController(IPaginatedRepository<Group> repository)
        {
            this.repository = repository;
        }

        [Authorize(Policy = "IsUser")]
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
        [Authorize(Policy = "IsUser")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Policy = "IsUser")]
        public IActionResult Create(Group group)
        {
            if (ModelState.IsValid)
            {
                repository.Create(group);
                return RedirectToAction("Index");
            }

            return View();
        }

        [Authorize(Policy = "IsUser")]
        public IActionResult Details(Guid id)
        {
            var group = repository.Get(id);
            return View(group);
        }

        [HttpGet]
        [Authorize(Policy = "IsUser")]
        public IActionResult Edit(Guid id)
        {
            var group = repository.Get(id);
            return View(group);
        }

        [HttpPost]
        [Authorize(Policy = "IsUser")]
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
        [Authorize(Policy = "IsUser")]
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