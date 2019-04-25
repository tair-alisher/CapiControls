using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace CapiControls.Web.Controllers
{
    public class ControlController : Controller
    {
        protected string HouseholdTitle = "Household";
        protected string IndividualTitle = "Individual";

        public IActionResult Index()
        {
            return View();
        }
    }
}