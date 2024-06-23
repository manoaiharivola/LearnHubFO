using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LearnHubFO.Services;
using LearnHubFO.Models;
using LearnHubBackOffice.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using DinkToPdf;
using DinkToPdf.Contracts;
using LearnHubFO.Utils;

namespace LearnHubFO.Controllers
{
    [Authorize]
    public class CoursController : Controller
    {
        private readonly CoursService _coursesService;
        private readonly CoursUtilisateurService _coursUtilisateurService;
        private readonly ChapitreService _chapitreService;
        private readonly PdfGeneratorUtil _pdfGeneratorUtil;
        private readonly ChapitreUtilisateurService _chapitreUtilisateurService;

        public CoursController(CoursService coursesService, CoursUtilisateurService coursUtilisateurService, ChapitreService chapitreService, PdfGeneratorUtil pdfGeneratorUtil, ChapitreUtilisateurService chapitreUtilisateurService)
        {
            _coursesService = coursesService;
            _coursUtilisateurService = coursUtilisateurService;
            _chapitreService = chapitreService;
            _pdfGeneratorUtil = pdfGeneratorUtil;
            _chapitreUtilisateurService = chapitreUtilisateurService;
            _chapitreUtilisateurService = chapitreUtilisateurService;
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

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var chapitres = await _coursesService.GetChapitresByCourseIdAsync(id, int.Parse(userId));
            ViewData["Chapitres"] = chapitres;

            var isFollowing = userId != null && await _coursUtilisateurService.EstCoursSuiviAsync(int.Parse(userId), id);
            ViewData["IsFollowing"] = isFollowing;

            var (totalChapitres, completedChapitres) = await _coursesService.GetChapitreProgressAsync(id, int.Parse(userId));
            ViewData["TotalChapitres"] = totalChapitres;
            ViewData["CompletedChapitres"] = completedChapitres;

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


        [HttpGet]
        public async Task<IActionResult> Chapitre(int id)
        {
            var chapitre = await _chapitreService.GetChapitreByIdAsync(id);
            if (chapitre == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isCompleted = await _chapitreUtilisateurService.IsChapitreCompletedAsync(id, int.Parse(userId));

            chapitre.IsCompleted = isCompleted;

            return View(chapitre);
        }

        [HttpGet]
        public async Task<IActionResult> ExportChapitreToPdf(int id)
        {
            var chapitre = await _chapitreService.GetChapitreByIdAsync(id);

            if (chapitre == null)
            {
                return NotFound();
            }

            var htmlContent = $@"
            <html>
            <head>
                <title>Chapitre {chapitre.Ordre} - {chapitre.TitreChapitre}</title>
            </head>
            <body>
                <h1>Chapitre {chapitre.Ordre} - {chapitre.TitreChapitre}</h1>
                {chapitre.Contenu}
            </body>
            </html>";

            var pdfBytes = _pdfGeneratorUtil.ConvertHtmlToPdf(htmlContent);

            return File(pdfBytes, "application/pdf", $"Chapitre {chapitre.Ordre} - {chapitre.TitreChapitre}.pdf");
        }

        [HttpPost]
        public async Task<IActionResult> MarquerCommeTermine(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _chapitreUtilisateurService.MarquerCommeTermineAsync(id, int.Parse(userId));
            return RedirectToAction("Chapitre", new { id });
        }

        [HttpPost]
        public async Task<IActionResult> NePasMarquerCommeTermine(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _chapitreUtilisateurService.NePasMarquerCommeTermineAsync(id, int.Parse(userId));
            return RedirectToAction("Chapitre", new { id });
        }
    }
}
