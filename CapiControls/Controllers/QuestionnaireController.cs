using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace CapiControls.Controllers
{
    public class QuestionnaireController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}