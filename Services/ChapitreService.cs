using System.Data.SqlClient;
using LearnHubBO.Models;

namespace LearnHubFO.Services
{
    public class ChapitreService
    {
        private readonly string _connectionString;

        public ChapitreService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<Chapitre> GetChapitreByIdAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(
                    "SELECT * FROM Chapitres WHERE IdChapitre = @IdChapitre",
                    connection);
                command.Parameters.AddWithValue("@IdChapitre", id);
                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new Chapitre
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
            }
            return null;
        }
    }
}
