using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LearnHubFO.Services;
using LearnHubFO.Models;
using LearnHubBackOffice.Models;

namespace LearnHubFO.Controllers
{
    public class CoursController : Controller
    {
        private readonly CoursService _coursesService;

        public CoursController(CoursService coursesService)
        {
            _coursesService = coursesService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int pageIndex = 1, int pageSize = 10, string searchTerm = "")
        {
            var totalCourses = await _coursesService.GetTotalCoursesCountAsync(searchTerm);
            var courses = await _coursesService.GetCoursesAsync(pageIndex, pageSize, searchTerm);
            var viewModel = new PagedResult<Cours>
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

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var course = await _coursesService.GetCourseByIdAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            var chapitres = await _coursesService.GetChapitresByCourseIdAsync(id);
            ViewData["Chapitres"] = chapitres;

            return View(course);
        }
    }
}
