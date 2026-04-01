using SportsLeague.Domain.Entities;

namespace SportsLeague.Domain.Interfaces.Repositories
{
    public interface ISponsorRepository
    {
        Task<IEnumerable<Sponsor>> GetAllAsync();
        Task<Sponsor?> GetByIdAsync(int id);
        Task AddAsync(Sponsor sponsor);
        Task UpdateAsync(Sponsor sponsor);
        Task DeleteAsync(int id);
        Task<bool> ExistsByNameAsync(string name);
    }
}