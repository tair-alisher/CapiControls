using CapiControls.BLL.DTO;
using CapiControls.BLL.Interfaces;
using CapiControls.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace CapiControls.Web.Controllers
{
    public class GroupController : Controller
    {
        private readonly IGroupService _groupService;

        public GroupController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        [Authorize(Policy = "IsUser")]
        public IActionResult Index(int page = 1)
        {
            int pageSize = 10;
            int count = _groupService.CountGroups();
            var groups = _groupService.GetGroups(page, pageSize);

            var pageViewModel = new PageViewModel(count, page, pageSize);
            var listModel = new ListViewModel<GroupDTO>
            {
                PageViewModel = pageViewModel,
                Items = groups
            };

            return View(groups);
        }

        [HttpGet]
        [Authorize(Policy = "IsUser")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Policy = "IsUser")]
        public IActionResult Create(GroupDTO group)
        {
            if (ModelState.IsValid)
            {
                _groupService.AddGroup(group);
                return RedirectToAction("Index");
            }

            return View(group);
        }

        [Authorize(Policy = "IsUser")]
        public IActionResult Details(Guid id)
        {
            var group = _groupService.GetGroup(id);
            return View(group);
        }

        [HttpGet]
        [Authorize(Policy = "IsUser")]
        public IActionResult Edit(Guid id)
        {
            var group = _groupService.GetGroup(id);
            return View(group);
        }

        [HttpPost]
        [Authorize(Policy = "IsUser")]
        public IActionResult Edit(GroupDTO group)
        {
            if (ModelState.IsValid)
            {
                _groupService.UpdateGroup(group);
                return RedirectToAction("Index");
            }

            return View(group);
        }

        [HttpGet]
        [Authorize(Policy = "IsUser")]
        public IActionResult Delete(Guid id)
        {
            if (_groupService.GetGroup(id) != null)
                _groupService.DeleteGroup(id);

            return RedirectToAction("Index");
        }
    }
}