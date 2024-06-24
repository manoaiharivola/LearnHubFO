using LearnHubFO.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LearnHubFO.Api
{
    [Route("api/cours")]
    [ApiController]
    [AllowAnonymous]
    public class CoursUtilisateursApiController : ControllerBase
    {
        private readonly CoursUtilisateurService _coursUtilisateurService;

        public CoursUtilisateursApiController(CoursUtilisateurService coursUtilisateurService)
        {
            _coursUtilisateurService = coursUtilisateurService;
        }

        [HttpPost("{courseId}/utilisateur/{utilisateurId}/suivre")]
        public async Task<IActionResult> SuivreCours(int courseId, int utilisateurId)
        {
            if (utilisateurId <= 0 || courseId <= 0)
            {
                return BadRequest(new { message = "Invalid user ID or course ID" });
            }

            var estSuivi = await _coursUtilisateurService.EstCoursSuiviAsync(utilisateurId, courseId);
            if (estSuivi)
            {
                return Conflict(new { message = "Course already followed by user" });
            }

            await _coursUtilisateurService.SuivreCoursAsync(utilisateurId, courseId);
            return Ok(new { message = "Course followed successfully" });
        }

        [HttpDelete("{courseId}/utilisateur/{userId}/nePlusSuivre")]
        public async Task<IActionResult> NePlusSuivreCours(int courseId, int userId)
        {
            if (userId <= 0 || courseId <= 0)
            {
                return BadRequest(new { message = "Invalid user ID or course ID" });
            }

            var estSuivi = await _coursUtilisateurService.EstCoursSuiviAsync(userId, courseId);
            if (!estSuivi)
            {
                return NotFound(new { message = "Course not followed by user" });
            }

            await _coursUtilisateurService.NePlusSuivreCoursAsync(userId, courseId);
            return Ok(new { message = "Course unfollowed successfully" });
        }
    }
}
