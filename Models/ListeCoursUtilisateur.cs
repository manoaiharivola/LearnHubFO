using LearnHubBackOffice.Models;

public class ListeCoursUtilisateur
{
    public int IdCours { get; set; }
    public string TitreCours { get; set; }
    public string Description { get; set; }
    public DateTime DateCreationCours { get; set; }
    public DateTime DateModificationCours { get; set; }
    public int IdFormateur { get; set; }
    public Formateur Formateur { get; set; }
    public int IdCoursCategorie { get; set; }
    public CoursCategorie CoursCategorie { get; set; }
    public int EstSuivi { get; set; } 
}
