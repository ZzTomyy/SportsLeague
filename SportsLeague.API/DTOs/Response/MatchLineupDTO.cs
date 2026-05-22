namespace SportsLeague.API.DTOs.Response
{
    public class MatchLineupDTO
    {
        public int Id { get; set; }
        public int MatchId { get; set; }
        public int PlayerId { get; set; }
        public string PlayerName { get; set; } = string.Empty; // Nombre completo del jugador [cite: 50]
        public string TeamName { get; set; } = string.Empty;   // Nombre del equipo del jugador [cite: 50]
        public bool IsStarter { get; set; }
        public string Position { get; set; } = string.Empty;   // Posición asignada [cite: 28]
    }
}