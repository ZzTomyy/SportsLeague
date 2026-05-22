using Microsoft.Extensions.Logging;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Enums;
using SportsLeague.Domain.Helpers;
using SportsLeague.Domain.Interfaces.Repositories;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.Domain.Services
{
    public class MatchLineupService : IMatchLineupService
    {
        private readonly IMatchLineupRepository _lineupRepository;
        private readonly IMatchRepository _matchRepository;
        private readonly MatchValidationHelper _validationHelper;
        private readonly ILogger<MatchLineupService> _logger;

        public MatchLineupService(
            IMatchLineupRepository lineupRepository,
            IMatchRepository matchRepository,
            MatchValidationHelper validationHelper,
            ILogger<MatchLineupService> logger)
        {
            _lineupRepository = lineupRepository;
            _matchRepository = matchRepository;
            _validationHelper = validationHelper;
            _logger = logger;
        }

        public async Task<MatchLineup> AddPlayerToLineupAsync(int matchId, int playerId, bool isStarter, string position)
        {
            _logger.LogInformation("Intentando agregar jugador {PlayerId} a la alineación del partido {MatchId}", playerId, matchId);

            // V1: El partido debe existir
            var match = await _matchRepository.GetByIdAsync(matchId);
            if (match == null)
                throw new KeyNotFoundException($"No se encontró el partido con ID {matchId}");

            // V6: El partido debe estar en estado Scheduled
            if (match.Status != MatchStatus.Scheduled)
                throw new InvalidOperationException("Solo se pueden registrar alineaciones en partidos Scheduled");

            // V2 y V3: El jugador debe existir y pertenecer al HomeTeam o AwayTeam del partido
            var player = await _validationHelper.ValidatePlayerInMatchAsync(playerId, match);

            // V4: El jugador no puede estar registrado dos veces en la misma alineación
            var isDuplicated = await _lineupRepository.ExistsByMatchAndPlayerAsync(matchId, playerId);
            if (isDuplicated)
                throw new InvalidOperationException("El jugador ya está registrado en la alineación de este partido");

            // V5: Máximo 11 titulares por equipo por partido (Solo si IsStarter es true)
            if (isStarter)
            {
                int startersCount = await _lineupRepository.CountStartersByTeamAsync(matchId, player.TeamId);
                if (startersCount >= 11)
                    throw new InvalidOperationException("El equipo ya tiene 11 titulares registrados en este partido");
            }

            // Crear el objeto de la entidad
            var lineup = new MatchLineup
            {
                MatchId = matchId,
                PlayerId = playerId,
                IsStarter = isStarter,
                Position = position
            };

            // Usamos CreateAsync que añade y confirma cambios según tu patrón base
            var savedLineup = await _lineupRepository.CreateAsync(lineup);

            // Mapeamos el objeto de navegación cargado en memoria para evitar consultas extra
            savedLineup.Player = player;

            _logger.LogInformation("Jugador {PlayerId} agregado exitosamente a la alineación", playerId);
            return savedLineup;
        }

        public async Task<IEnumerable<MatchLineup>> GetFullLineupAsync(int matchId)
        {
            _logger.LogInformation("Obteniendo alineación completa para el partido {MatchId}", matchId);

            var match = await _matchRepository.GetByIdAsync(matchId);
            if (match == null)
                throw new KeyNotFoundException($"No se encontró el partido con ID {matchId}");

            return await _lineupRepository.GetByMatchAsync(matchId);
        }

        public async Task<IEnumerable<MatchLineup>> GetLineupByTeamAsync(int matchId, int teamId)
        {
            _logger.LogInformation("Obteniendo alineación del partido {MatchId} filtrada por equipo {TeamId}", matchId, teamId);

            var match = await _matchRepository.GetByIdAsync(matchId);
            if (match == null)
                throw new KeyNotFoundException($"No se encontró el partido con ID {matchId}");

            return await _lineupRepository.GetByMatchAndTeamAsync(matchId, teamId);
        }

        public async Task RemovePlayerFromLineupAsync(int matchId, int lineupId)
        {
            _logger.LogInformation("Intentando remover el registro de alineación {LineupId} del partido {MatchId}", lineupId, matchId);

            var lineup = await _lineupRepository.GetByIdAsync(lineupId);
            if (lineup == null || lineup.MatchId != matchId)
                throw new KeyNotFoundException($"No se encontró el registro de alineación con ID {lineupId} en este partido");

            // Tu método DeleteAsync recibe directamente el ID entero y procesa los cambios internamente
            await _lineupRepository.DeleteAsync(lineupId);
        }
    }
}