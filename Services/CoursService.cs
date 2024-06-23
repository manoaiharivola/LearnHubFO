using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using LearnHubBackOffice.Models;
using Microsoft.Extensions.Configuration;

namespace LearnHubFO.Services
{
    public class CoursService
    {
        private readonly string _connectionString;

        public CoursService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<List<Cours>> GetAllCoursesAsync()
        {
            var courses = new List<Cours>();
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(
                    "SELECT c.*, f.NomFormateur, f.Email AS FormateurEmail, cc.NomCoursCategorie AS CoursCategorieNom " +
                    "FROM Courses c " +
                    "JOIN Formateurs f ON c.IdFormateur = f.IdFormateur " +
                    "JOIN CoursCategories cc ON c.IdCoursCategorie = cc.IdCoursCategorie",
                    connection);
                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        courses.Add(new Cours
                        {
                            IdCours = reader.GetInt32(reader.GetOrdinal("IdCours")),
                            TitreCours = reader.GetString(reader.GetOrdinal("TitreCours")),
                            Description = reader.GetString(reader.GetOrdinal("Description")),
                            DateCreationCours = reader.GetDateTime(reader.GetOrdinal("DateCreationCours")),
                            DateModificationCours = reader.GetDateTime(reader.GetOrdinal("DateModificationCours")),
                            IdFormateur = reader.GetInt32(reader.GetOrdinal("IdFormateur")),
                            IdCoursCategorie = reader.GetInt32(reader.GetOrdinal("IdCoursCategorie")),
                            Formateur = new Formateur
                            {
                                IdFormateur = reader.GetInt32(reader.GetOrdinal("IdFormateur")),
                                Email = reader.GetString(reader.GetOrdinal("FormateurEmail")),
                                NomFormateur = reader.GetString(reader.GetOrdinal("NomFormateur"))
                            },
                            CoursCategorie = new CoursCategorie
                            {
                                IdCoursCategorie = reader.GetInt32(reader.GetOrdinal("IdCoursCategorie")),
                                NomCoursCategorie = reader.GetString(reader.GetOrdinal("CoursCategorieNom"))
                            }
                        });
                    }
                }
            }
            return courses;
        }
    }
}
