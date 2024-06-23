using System.Data.SqlClient;
using LearnHubFO.Models;

namespace LearnHubFO.Services
{
    public class ChapitreService
    {
        private readonly string _connectionString;

        public ChapitreService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<ConsulterChapitre> GetChapitreByIdAsync(int id)
        {
            ConsulterChapitre chapitre = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(
                    "SELECT * FROM Chapitres WHERE IdChapitre = @IdChapitre", connection);
                command.Parameters.AddWithValue("@IdChapitre", id);
                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        chapitre = new ConsulterChapitre
                        {
                            IdChapitre = reader.GetInt32(reader.GetOrdinal("IdChapitre")),
                            TitreChapitre = reader.GetString(reader.GetOrdinal("TitreChapitre")),
                            Ordre = reader.GetInt32(reader.GetOrdinal("Ordre")),
                            Contenu = reader.GetString(reader.GetOrdinal("Contenu")),
                            IdCours = reader.GetInt32(reader.GetOrdinal("IdCours")),
                            DateCreationChapitre = reader.GetDateTime(reader.GetOrdinal("DateCreationChapitre")),
                            DateModificationChapitre = reader.GetDateTime(reader.GetOrdinal("DateModificationChapitre"))
                        };
                    }
                }
                if (chapitre != null)
                {
                    // Get the previous chapitre
                    command = new SqlCommand(
                        "SELECT TOP 1 IdChapitre FROM Chapitres WHERE IdCours = @IdCours AND Ordre < @Ordre ORDER BY Ordre DESC",
                        connection);
                    command.Parameters.AddWithValue("@IdCours", chapitre.IdCours);
                    command.Parameters.AddWithValue("@Ordre", chapitre.Ordre);
                    var previousChapitreId = await command.ExecuteScalarAsync();
                    chapitre.PreviousChapitreId = previousChapitreId as int?;

                    // Get the next chapitre
                    command = new SqlCommand(
                        "SELECT TOP 1 IdChapitre FROM Chapitres WHERE IdCours = @IdCours AND Ordre > @Ordre ORDER BY Ordre ASC",
                        connection);
                    command.Parameters.AddWithValue("@IdCours", chapitre.IdCours);
                    command.Parameters.AddWithValue("@Ordre", chapitre.Ordre);
                    var nextChapitreId = await command.ExecuteScalarAsync();
                    chapitre.NextChapitreId = nextChapitreId as int?;
                }
            }
            return chapitre;
        }
    }
}
