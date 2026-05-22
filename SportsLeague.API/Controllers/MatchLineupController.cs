using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using SportsLeague.API.DTOs.Request;
using SportsLeague.API.DTOs.Response;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.API.Controllers
{
    [ApiController]
    [Route("api/match/{matchId}/lineup")]
    public class MatchLineupController : ControllerBase
    {
        private readonly IMatchLineupService _lineupService;
        private readonly IMapper _mapper;

        public MatchLineupController(IMatchLineupService lineupService, IMapper mapper)
        {
            _lineupService = lineupService;
            _mapper = mapper;
        }

        // POST: /api/match/{matchId}/lineup -> Agregar un jugador a la alineación
        [HttpPost]
        public async Task<IActionResult> AddPlayer(int matchId, [FromBody] CreateMatchLineupDTO dto)
        {
            try
            {
                // Ejecuta la lógica de negocio aplicando las 6 validaciones requeridas (V1-V6)
                var result = await _lineupService.AddPlayerToLineupAsync(matchId, dto.PlayerId, dto.IsStarter, dto.Position);

                // Mapea la entidad de salida al DTO estructurado en la carpeta Response
                var mappedResult = _mapper.Map<MatchLineupDTO>(result);

                // Retorna un código HTTP 201 (Created) con la ruta de consulta del recurso creado
                return CreatedAtAction(nameof(GetFullLineup), new { matchId = matchId }, mappedResult);
            }
            catch (KeyNotFoundException ex)
            {
                // Código HTTP 404 si el partido o el jugador no existen (V1, V2)
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                // Código HTTP 409 (Conflict) exigido para fallos en las reglas de negocio (V3, V4, V5, V6)
                return Conflict(new { message = ex.Message });
            }
        }

        // GET: /api/match/{matchId}/lineup -> Obtener la alineación completa del partido
        [HttpGet]
        public async Task<IActionResult> GetFullLineup(int matchId)
        {
            try
            {
                var lineups = await _lineupService.GetFullLineupAsync(matchId);
                var mappedLineups = _mapper.Map<IEnumerable<MatchLineupDTO>>(lineups);

                // Retorna un código HTTP 200 (OK) con el listado completo
                return Ok(mappedLineups);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // GET: /api/match/{matchId}/lineup/team/{teamId} -> Obtener alineación de un equipo específico
        [HttpGet("team/{teamId}")]
        public async Task<IActionResult> GetLineupByTeam(int matchId, int teamId)
        {
            try
            {
                var lineups = await _lineupService.GetLineupByTeamAsync(matchId, teamId);
                var mappedLineups = _mapper.Map<IEnumerable<MatchLineupDTO>>(lineups);

                // Retorna un código HTTP 200 (OK) con la lista filtrada por club
                return Ok(mappedLineups);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // DELETE: /api/match/{matchId}/lineup/{id} -> Eliminar un jugador de la alineación
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemovePlayer(int matchId, int id)
        {
            try
            {
                await _lineupService.RemovePlayerFromLineupAsync(matchId, id);

                // Retorna un código HTTP 204 (No Content) tras una eliminación exitosa
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                // Retorna un código HTTP 404 si el registro con el ID suministrado no existe
                return NotFound(new { message = ex.Message });
            }
        }
    }
}