using FreelanceMarketplace.Models;
using FreelanceMarketplace.Services;
using Microsoft.AspNetCore.Mvc;

namespace FreelanceMarketplace.Controllers
{
    public class JobController : Controller
    {
        private readonly IJobService _jobService;

        public JobController(IJobService jobService)
        {
            _jobService = jobService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Job model)
        {
            ModelState.Remove("Category");
            ModelState.Remove("ClientProfile");
            ModelState.Remove("User");
            ModelState.Remove("Proposals");
            ModelState.Remove("Skills");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.ClientProfileId <= 0)
            {
                model.ClientProfileId = 1;
            }

            await _jobService.CreateJobAsync(model);

            return RedirectToAction("Index", "Home");
        }
    }

}
