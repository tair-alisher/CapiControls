using Microsoft.AspNetCore.Mvc;

namespace CapiControls.Controllers
{
    public abstract class BaseController : Controller
    {
        public string HouseholdTitle = "Household";
        public string IndividualTitle = "Individual";
    }
}