using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LearnHubFO.Services;
using LearnHubFO.Models;
using System.Security.Claims;
using LearnHubBackOffice.Models;

namespace LearnHubFO.Controllers
{
    public class MesCoursController : Controller
    {
        private readonly CoursService _coursesService;

        public MesCoursController(CoursService coursesService)
        {
            _coursesService = coursesService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int pageIndex = 1, int pageSize = 10, string searchTerm = "")
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return RedirectToAction("Login", "Utilisateurs");
            }

            int userIdInt = int.Parse(userId);

            var totalCourses = await _coursesService.GetTotalUserCoursesCountAsync(searchTerm, userIdInt);
            var courses = await _coursesService.GetUserCoursesAsync(pageIndex, pageSize, searchTerm, userIdInt);
            var viewModel = new PagedResult<CoursSuivi>
            {
                Items = courses,
                PageIndex = pageIndex,
                TotalItems = totalCourses,
                PageSize = pageSize
            };
            ViewData["PageSize"] = pageSize;
            ViewData["SearchTerm"] = searchTerm;
            return View(viewModel);
        }
    }
}
