using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.Domain.Services
{
    public class SponsorService : ISponsorService
    {
        private readonly ISponsorRepository _sponsorRepository;

        public SponsorService(ISponsorRepository sponsorRepository)
        {
            _sponsorRepository = sponsorRepository;
        }

        public async Task<IEnumerable<Sponsor>> GetAllAsync()
        {
            return await _sponsorRepository.GetAllAsync();
        }

        public async Task<Sponsor?> GetByIdAsync(int id)
        {
            return await _sponsorRepository.GetByIdAsync(id);
        }

        public async Task<Sponsor> CreateAsync(Sponsor sponsor)
        {
            // 🔴 VALIDACIÓN 1: Nombre requerido
            if (string.IsNullOrWhiteSpace(sponsor.Name))
                throw new ArgumentException("Sponsor name is required");

            // 🔴 VALIDACIÓN 2: Email requerido
            if (string.IsNullOrWhiteSpace(sponsor.ContactEmail))
                throw new ArgumentException("Contact email is required");

            // 🔴 VALIDACIÓN 3: Nombre duplicado
            var exists = await _sponsorRepository.ExistsByNameAsync(sponsor.Name);
            if (exists)
                throw new InvalidOperationException("Sponsor name already exists");

            // 🔴 VALIDACIÓN 4: Email válido
            if (!IsValidEmail(sponsor.ContactEmail))
                throw new ArgumentException("Invalid email format");

            sponsor.CreatedAt = DateTime.UtcNow;

            await _sponsorRepository.AddAsync(sponsor);

            return sponsor;
        }

        public async Task UpdateAsync(int id, Sponsor sponsor)
        {
            var existing = await _sponsorRepository.GetByIdAsync(id);

            // 🔴 Validar existencia
            if (existing == null)
                throw new KeyNotFoundException("Sponsor not found");

            // 🔴 Validar nombre duplicado SOLO si cambia
            if (existing.Name != sponsor.Name)
            {
                var exists = await _sponsorRepository.ExistsByNameAsync(sponsor.Name);
                if (exists)
                    throw new InvalidOperationException("Sponsor name already exists");
            }

            // 🔴 Validar email
            if (!IsValidEmail(sponsor.ContactEmail))
                throw new ArgumentException("Invalid email format");

            // 🔁 Actualización
            existing.Name = sponsor.Name;
            existing.ContactEmail = sponsor.ContactEmail;
            existing.Phone = sponsor.Phone;
            existing.WebsiteUrl = sponsor.WebsiteUrl;
            existing.Category = sponsor.Category;
            existing.UpdatedAt = DateTime.UtcNow;

            await _sponsorRepository.UpdateAsync(existing);
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _sponsorRepository.GetByIdAsync(id);

            if (existing == null)
                throw new KeyNotFoundException("Sponsor not found");

            await _sponsorRepository.DeleteAsync(id);
        }

        // 🔴 MÉTODO PARA VALIDAR EMAIL
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}