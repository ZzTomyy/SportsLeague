using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Services;
using SportsLeague.API.DTOs.Request;
using SportsLeague.API.DTOs.Response;

namespace SportsLeague.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SponsorController : ControllerBase
    {
        private readonly ISponsorService _sponsorService;
        private readonly ITournamentSponsorService _tournamentSponsorService;
        private readonly IMapper _mapper;

        public SponsorController(
            ISponsorService sponsorService,
            ITournamentSponsorService tournamentSponsorService,
            IMapper mapper)
        {
            _sponsorService = sponsorService;
            _tournamentSponsorService = tournamentSponsorService;
            _mapper = mapper;
        }

        // =========================
        // 🔹 SPONSOR CRUD
        // =========================

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var sponsors = await _sponsorService.GetAllAsync();
            var result = _mapper.Map<IEnumerable<SponsorResponseDTO>>(sponsors);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var sponsor = await _sponsorService.GetByIdAsync(id);

            if (sponsor == null)
                return NotFound("Sponsor not found");

            var result = _mapper.Map<SponsorResponseDTO>(sponsor);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SponsorRequestDTO dto)
        {
            try
            {
                var sponsor = _mapper.Map<Sponsor>(dto);
                var created = await _sponsorService.CreateAsync(sponsor);
                var result = _mapper.Map<SponsorResponseDTO>(created);

                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] SponsorRequestDTO dto)
        {
            try
            {
                var sponsor = _mapper.Map<Sponsor>(dto);
                await _sponsorService.UpdateAsync(id, sponsor);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _sponsorService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // =========================
        // 🔥 TOURNAMENT SPONSOR
        // =========================

        // 🔹 POST: Agregar torneo a sponsor
        [HttpPost("{sponsorId}/tournaments")]
        public async Task<IActionResult> AddTournament(int sponsorId, [FromBody] TournamentSponsorRequestDTO dto)
        {
            try
            {
                var entity = new TournamentSponsor
                {
                    SponsorId = sponsorId,
                    TournamentId = dto.TournamentId,
                    ContractAmount = dto.ContractAmount
                };

                var created = await _tournamentSponsorService.AddAsync(entity);

                var result = _mapper.Map<TournamentSponsorResponseDTO>(created);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 🔹 GET: Obtener torneos por sponsor
        [HttpGet("{sponsorId}/tournaments")]
        public async Task<IActionResult> GetTournaments(int sponsorId)
        {
            var list = await _tournamentSponsorService.GetBySponsorIdAsync(sponsorId);

            var result = _mapper.Map<IEnumerable<TournamentSponsorResponseDTO>>(list);

            return Ok(result);
        }

        // 🔹 DELETE: Eliminar relación
        [HttpDelete("{sponsorId}/tournaments/{tournamentId}")]
        public async Task<IActionResult> RemoveTournament(int sponsorId, int tournamentId)
        {
            try
            {
                await _tournamentSponsorService.RemoveAsync(sponsorId, tournamentId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}