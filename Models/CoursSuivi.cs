using LearnHubBackOffice.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LearnHubFO.Models
{
    public class CoursSuivi
    {
        public int IdCours { get; set; }
        public string TitreCours { get; set; }
        public int IdFormateur { get; set; }
        public int IdCoursCategorie { get; set; }
        public DateTime DateCreationCours { get; set; }
        public DateTime DateModificationCours { get; set; }
        public Formateur Formateur { get; set; }
        public CoursCategorie CoursCategorie { get; set; }
        public string? Description { get; set; }
        public DateTime DateCreationCoursUtilisateur { get; set; }

        public int? TotalChapitres { get; set; } = 0;

        public int? CompletedChapitres { get; set; } = 0;
    }
}
