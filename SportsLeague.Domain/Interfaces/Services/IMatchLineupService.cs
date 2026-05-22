using SportsLeague.Domain.Entities;

namespace SportsLeague.Domain.Interfaces.Services
{
    public interface IMatchLineupService
    {
        Task<MatchLineup> AddPlayerToLineupAsync(int matchId, int playerId, bool isStarter, string position);
        Task<IEnumerable<MatchLineup>> GetFullLineupAsync(int matchId);
        Task<IEnumerable<MatchLineup>> GetLineupByTeamAsync(int matchId, int teamId);
        Task RemovePlayerFromLineupAsync(int matchId, int lineupId);
    }
}