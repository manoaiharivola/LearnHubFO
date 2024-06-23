using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LearnHubFO.Services;
using LearnHubFO.Models;
using LearnHubBackOffice.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace LearnHubFO.Controllers
{
    [Authorize]
    public class CoursController : Controller
    {
        private readonly CoursService _coursesService;
        private readonly CoursUtilisateurService _coursUtilisateurService;

        public CoursController(CoursService coursesService, CoursUtilisateurService coursUtilisateurService)
        {
            _coursesService = coursesService;
            _coursUtilisateurService = coursUtilisateurService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int pageIndex = 1, int pageSize = 10, string searchTerm = "")
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int userIdInt = userId != null ? int.Parse(userId) : 0;

            var totalCourses = await _coursesService.GetTotalCoursesWithFollowingStatusOfUserCountAsync(searchTerm);
            var courses = await _coursesService.GetCoursesWithFollowingStatusOfUserAsync(pageIndex, pageSize, searchTerm, userIdInt) ;
            var viewModel = new PagedResult<ListeCoursUtilisateur>
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

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isFollowing = userId != null && await _coursUtilisateurService.EstCoursSuiviAsync(int.Parse(userId), id);
            ViewData["IsFollowing"] = isFollowing;

            return View(course);
        }

        [HttpPost]
        public async Task<IActionResult> SuivreCours(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            await _coursUtilisateurService.SuivreCoursAsync(int.Parse(userId), id);
            return RedirectToAction("Details", new { id });
        }

        [HttpPost]
        public async Task<IActionResult> NePlusSuivreCours(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            await _coursUtilisateurService.NePlusSuivreCoursAsync(int.Parse(userId), id);
            return RedirectToAction("Details", new { id });
        }
    }
}
