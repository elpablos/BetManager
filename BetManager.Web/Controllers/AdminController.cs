using BetManager.Web.Controllers.Admins.Jobs.Create;
using BetManager.Web.Mvc;
using System.Web.Mvc;

namespace BetManager.Web.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult CreateJob()
        {
            var result = Handler.Get<CreateJobBuilder>().Build();
            return View(result.Model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateJob(CreateJobModel model)
        {
            var result = Handler.Get<CreateJobHandler>().Handle(model);
            return RedirectToAction("Index", "Admin");
        }
    }
}