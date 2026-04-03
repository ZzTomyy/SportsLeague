using SportsLeague.Domain.Entities;

namespace SportsLeague.Domain.Interfaces.Repositories
{
    public interface ITournamentSponsorRepository
    {
        Task<TournamentSponsor> CreateAsync(TournamentSponsor entity);
        Task<bool> ExistsAsync(int sponsorId, int tournamentId);
        Task<List<TournamentSponsor>> GetBySponsorIdAsync(int sponsorId);
        Task<TournamentSponsor?> GetByIdsAsync(int sponsorId, int tournamentId);
        Task DeleteAsync(TournamentSponsor entity);
    }
}