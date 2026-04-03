using Microsoft.EntityFrameworkCore;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;
using SportsLeague.DataAccess.Context;

namespace SportsLeague.DataAccess.Repositories
{
    public class TournamentSponsorRepository : ITournamentSponsorRepository
    {
        private readonly LeagueDbContext _context;

        public TournamentSponsorRepository(LeagueDbContext context)
        {
            _context = context;
        }

        public async Task<TournamentSponsor> CreateAsync(TournamentSponsor entity)
        {
            await _context.TournamentSponsors.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> ExistsAsync(int sponsorId, int tournamentId)
        {
            return await _context.TournamentSponsors
                .AnyAsync(ts => ts.SponsorId == sponsorId && ts.TournamentId == tournamentId);
        }

        public async Task<List<TournamentSponsor>> GetBySponsorIdAsync(int sponsorId)
        {
            return await _context.TournamentSponsors
                .Include(ts => ts.Tournament)
                .Include(ts => ts.Sponsor)
                .Where(ts => ts.SponsorId == sponsorId)
                .ToListAsync();
        }

        public async Task<TournamentSponsor?> GetByIdsAsync(int sponsorId, int tournamentId)
        {
            return await _context.TournamentSponsors
                .FirstOrDefaultAsync(ts => ts.SponsorId == sponsorId && ts.TournamentId == tournamentId);
        }

        public async Task DeleteAsync(TournamentSponsor entity)
        {
            _context.TournamentSponsors.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}