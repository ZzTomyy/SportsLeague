using System.ComponentModel.DataAnnotations;

namespace SportsLeague.API.DTOs.Request
{
    public class CreateMatchLineupDTO
    {
        [Required]
        public int PlayerId { get; set; }

        [Required]
        public bool IsStarter { get; set; }

        [Required]
        public string Position { get; set; } = string.Empty; // Ejemplo: "GK", "CB", "CDM", "ST" [cite: 16]
    }
}
