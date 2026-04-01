using Microsoft.EntityFrameworkCore;
using SportsLeague.DataAccess.Context;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;

namespace SportsLeague.DataAccess.Repositories
{
    public class SponsorRepository : ISponsorRepository
    {
        private readonly LeagueDbContext _context;

        public SponsorRepository(LeagueDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Sponsor>> GetAllAsync()
        {
            return await _context.Sponsors.ToListAsync();
        }

        public async Task<Sponsor?> GetByIdAsync(int id)
        {
            return await _context.Sponsors.FindAsync(id);
        }

        public async Task AddAsync(Sponsor sponsor)
        {
            await _context.Sponsors.AddAsync(sponsor);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Sponsor sponsor)
        {
            _context.Sponsors.Update(sponsor);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var sponsor = await _context.Sponsors.FindAsync(id);

            if (sponsor != null)
            {
                _context.Sponsors.Remove(sponsor);
                await _context.SaveChangesAsync();
            }
        }

        // 🔴 MÉTODO PARA VALIDAR NOMBRE DUPLICADO
        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _context.Sponsors
                .AnyAsync(s => s.Name == name);
        }
    }
}