using ExtraHours.API.Model;
using ExtraHours.API.Repositories.Interfaces;
using ExtraHours.API.Service.Interface;
using Microsoft.Extensions.Logging;

namespace ExtraHours.API.Service.Implementations
{
    public class ExtraHoursConfigService : IExtraHoursConfigService
    {
        private readonly IExtraHoursConfigRepository _configRepository;
        private readonly ILogger<ExtraHoursConfigService> _logger;

        public ExtraHoursConfigService(
            IExtraHoursConfigRepository configRepository,
            ILogger<ExtraHoursConfigService> logger)
        {
            _configRepository = configRepository;
            _logger = logger;
        }

        public async Task<ExtraHoursConfig> GetConfigAsync()
        {
            try
            {
                _logger.LogInformation("Obteniendo configuración de horas extras");

                var config = await _configRepository.GetConfigAsync();
                if (config == null)
                {
                    _logger.LogWarning("Configuración no encontrada");
                    throw new KeyNotFoundException("Configuración no encontrada");
                }

                _logger.LogInformation("Configuración obtenida exitosamente. ID: {ConfigId}", config.id);
                return config;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo configuración");
                throw;
            }
        }

        public async Task<ExtraHoursConfig> UpdateConfigAsync(ExtraHoursConfig config)
        {
            try
            {
                _logger.LogInformation("Actualizando configuración de horas extras");

                // Validar que los datos sean válidos
                if (config == null)
                {
                    _logger.LogWarning("Configuración recibida es null");
                    throw new ArgumentNullException(nameof(config), "La configuración no puede ser null");
                }

                // Validaciones básicas
                if (config.weeklyExtraHoursLimit <= 0)
                {
                    throw new ArgumentException("El límite semanal de horas extras debe ser mayor a 0");
                }

                if (config.diurnalStart >= config.diurnalEnd)
                {
                    throw new ArgumentException("La hora de inicio diurna debe ser menor que la hora de fin");
                }

                // Asegurarse de que solo existe un registro
                config.id = 1L;

                _logger.LogInformation("Actualizando configuración con ID: {ConfigId}", config.id);

                var result = await _configRepository.UpdateConfigAsync(config);

                _logger.LogInformation("Configuración actualizada exitosamente");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error actualizando configuración: {@Config}", config);
                throw;
            }
        }
    }
}