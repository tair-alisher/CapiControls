using CapiControls.BLL.DTO;
using CapiControls.BLL.Interfaces;
using CapiControls.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    }
}