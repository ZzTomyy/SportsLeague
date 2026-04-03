using SportsLeague.Domain.Entities;

namespace SportsLeague.Domain.Interfaces.Services
{
    public interface ITournamentSponsorService
    {
        Task<TournamentSponsor> AddAsync(TournamentSponsor tournamentSponsor);
        Task<IEnumerable<TournamentSponsor>> GetBySponsorIdAsync(int sponsorId);
        Task RemoveAsync(int sponsorId, int tournamentId);
    }
}