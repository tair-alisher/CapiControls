using CapiControls.Data.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace CapiControls.Controllers
{
    public class InterviewController : Controller
    {
        private readonly IInterviewRepository repository;
        public InterviewController(IInterviewRepository repository) =>
            this.repository = repository;

        public IActionResult Index()
        {
            var firstInterview = repository.GetInterviewsByQuestionnaire("f13757168d804a2cb2c84590e7dc3e9b$1", 100, 100).First();

            return View(firstInterview);
        }
    }
}
