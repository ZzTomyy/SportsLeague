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
        private readonly IMapper _mapper;

        public SponsorController(ISponsorService sponsorService, IMapper mapper)
        {
            _sponsorService = sponsorService;
            _mapper = mapper;
        }

        // GET: api/sponsor
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var sponsors = await _sponsorService.GetAllAsync();

            var result = _mapper.Map<IEnumerable<SponsorResponseDTO>>(sponsors);

            return Ok(result);
        }

        // GET: api/sponsor/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var sponsor = await _sponsorService.GetByIdAsync(id);

            if (sponsor == null)
                return NotFound("Sponsor not found");

            var result = _mapper.Map<SponsorResponseDTO>(sponsor);

            return Ok(result);
        }

        // POST: api/sponsor
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

        // PUT: api/sponsor/5
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

        // DELETE: api/sponsor/5
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
    }
}