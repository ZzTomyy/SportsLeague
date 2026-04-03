using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.Domain.Services
{
    public class TournamentSponsorService : ITournamentSponsorService
    {
        private readonly ITournamentSponsorRepository _repository;
        private readonly ISponsorRepository _sponsorRepository;
        private readonly ITournamentRepository _tournamentRepository;

        public TournamentSponsorService(
            ITournamentSponsorRepository repository,
            ISponsorRepository sponsorRepository,
            ITournamentRepository tournamentRepository)
        {
            _repository = repository;
            _sponsorRepository = sponsorRepository;
            _tournamentRepository = tournamentRepository;
        }

        public async Task<TournamentSponsor> AddAsync(TournamentSponsor entity)
        {
            // 🔴 Validar sponsor
            var sponsor = await _sponsorRepository.GetByIdAsync(entity.SponsorId);
            if (sponsor == null)
                throw new KeyNotFoundException("Sponsor not found");

            // 🔴 Validar tournament
            var tournament = await _tournamentRepository.GetByIdAsync(entity.TournamentId);
            if (tournament == null)
                throw new KeyNotFoundException("Tournament not found");

            // 🔴 Validar contrato
            if (entity.ContractAmount <= 0)
                throw new InvalidOperationException("ContractAmount must be greater than 0");

            // 🔴 Validar duplicado
            var exists = await _repository.ExistsAsync(entity.SponsorId, entity.TournamentId);
            if (exists)
                throw new InvalidOperationException("Relation already exists");

            entity.JoinedAt = DateTime.UtcNow;

            await _repository.CreateAsync(entity);

            return entity;
        }

        public async Task<IEnumerable<TournamentSponsor>> GetBySponsorIdAsync(int sponsorId)
        {
            return await _repository.GetBySponsorIdAsync(sponsorId);
        }

        public async Task RemoveAsync(int sponsorId, int tournamentId)
        {
            var entity = await _repository.GetByIdsAsync(sponsorId, tournamentId);

            if (entity == null)
                throw new KeyNotFoundException("Relation not found");

            await _repository.DeleteAsync(entity);
        }
    }
}