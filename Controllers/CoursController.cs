using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LearnHubFO.Services;

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
        public async Task<IActionResult> Index()
        {
            var courses = await _coursesService.GetAllCoursesAsync();
            return View(courses);
        }
    }
}