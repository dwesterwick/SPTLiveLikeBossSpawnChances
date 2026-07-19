using DansDevTools.Configuration;
using LiveLikeBossSpawnChances.Helpers;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Services;
using System.Numerics;

namespace LiveLikeBossSpawnChances.Utils
{
    [Injectable(InjectionType.Singleton)]
    public class AdjustSpawnChancesUtil
    {
        private double _lastAdjustmentMultiplierApplied = 1;

        private LoggingUtil _logger;
        private ConfigUtil _config;
        private DatabaseService _databaseService;
        private ProfileUtil _profileUtil;

        public AdjustSpawnChancesUtil(LoggingUtil logger, ConfigUtil config, DatabaseService databaseService, ProfileUtil profileUtil)
        {
            _logger = logger;
            _config = config;
            _databaseService = databaseService;
            _profileUtil = profileUtil;
        }

        public void AdjustAllBossSpawnChancesUsingDefaultPlayerValues() => AdjustAllBossSpawnChances(null);

        public void AdjustAllBossSpawnChances(MongoId? sessionId)
        {
            int pmcLevel = GetPmcLevel(sessionId);
            double playerHours = GetPlayerHours(sessionId);

            double minProgressionFactor = GetMinProgressionFactor(pmcLevel, playerHours);

            MinMaxConfig adjustmentRange = _config.CurrentConfig.Thresholds.AdjustmentRange;
            double adjustmentMultiplier = adjustmentRange.Min + (adjustmentRange.Max - adjustmentRange.Min) * minProgressionFactor;

            AdjustAllBossSpawnChances(adjustmentMultiplier);
        }

        public void AdjustAllBossSpawnChances(double adjustmentMultiplier)
        {
            double correctedAdjustmentMultiplier = adjustmentMultiplier / _lastAdjustmentMultiplierApplied;
            if (correctedAdjustmentMultiplier == 1)
            {
                return;
            }

            _logger.Info($"Scaling boss spawn chances to {Math.Round(adjustmentMultiplier * 100)}% of their default spawn chances");

            foreach (Location location in _databaseService.EnumerateLocations())
            {
                AdjustBossSpawnChancesForLocation(location, correctedAdjustmentMultiplier);
            }

            _lastAdjustmentMultiplierApplied = adjustmentMultiplier;
        }

        private void AdjustBossSpawnChancesForLocation(Location location, double adjustmentMultiplier)
        {
            if (location?.Base?.BossLocationSpawn == null)
            {
                return;
            }

            foreach (BossLocationSpawn bossLocationSpawn in location.Base.BossLocationSpawn)
            {
                if (_config.CurrentConfig.IgnoredBosses.TryGetValue(location.Base.Name!, out string[]? ignoredBosses) && (ignoredBosses != null))
                {
                    if (ignoredBosses.Contains(bossLocationSpawn.BossName))
                    {
                        continue;
                    }
                }

                double originalChance = bossLocationSpawn.BossChance ?? 0;

                if (_config.CurrentConfig.BlockedBosses.Contains(bossLocationSpawn.BossName))
                {
                    bossLocationSpawn.BossChance = 0;
                }
                else
                {
                    bossLocationSpawn.BossChance = Math.Max(0, Math.Min(100, Math.Round(originalChance * adjustmentMultiplier)));
                }

                if (originalChance != bossLocationSpawn.BossChance)
                {
                    _logger.Debug($"Changed spawn chance of {bossLocationSpawn.BossName} on {location.Base.Name} from {originalChance}% to {bossLocationSpawn.BossChance}%");
                }
            }
        }

        private int GetPmcLevel(MongoId? sessionId)
        {
            int? pmcLevel = null;

            if (sessionId != null)
            {
                pmcLevel = _profileUtil.GetPmcLevel(sessionId.Value);
            }
            
            if (pmcLevel == null)
            {
                pmcLevel = _config.CurrentConfig.Debug.DefaultPlayerLevel;

                _logger.Error($"Could not retrieve PMC profile. Assuming player level is {pmcLevel}.");
            }

            return pmcLevel.Value;
        }

        private double GetPlayerHours(MongoId? sessionId)
        {
            double? playerHours = null;

            if (sessionId != null)
            {
                playerHours = _profileUtil.GetPlayerHours(sessionId.Value);
            }

            if (playerHours == null)
            {
                playerHours = _config.CurrentConfig.Debug.DefaultPlayerHours;

                _logger.Error($"Could not retrieve PMC profile. Assuming player hours are {playerHours}.");
            }

            return playerHours.Value;
        }

        private double GetMinProgressionFactor(int pmcLevel, double playerHours)
        {
            double minProgressionFactor = 1;

            int maxLevelForAdjustments = _config.CurrentConfig.AdjustmentsDisabledAfterPlayerLevel;
            if (pmcLevel > maxLevelForAdjustments)
            {
                _logger.Debug($"Player level is {pmcLevel}, and no adjustments will be made after level {maxLevelForAdjustments}");
                return minProgressionFactor;
            }

            if (_config.CurrentConfig.AdjustmentFactors.PlayerLevel)
            {
                double pmcLevelProgressionFactor = GetProgressionFactor(_config.CurrentConfig.Thresholds.PlayerLevel, pmcLevel);
                _logger.Debug($"Calculated progression factor of {Math.Round(pmcLevelProgressionFactor, 2)} for player level {pmcLevel}");

                minProgressionFactor = Math.Min(minProgressionFactor, pmcLevelProgressionFactor);
            }

            if (_config.CurrentConfig.AdjustmentFactors.PlayerHours)
            {
                double playerHoursProgressionFactor = GetProgressionFactor(_config.CurrentConfig.Thresholds.PlayerHours, playerHours);
                _logger.Debug($"Calculated progression factor of {Math.Round(playerHoursProgressionFactor, 2)} for player hours {playerHours}");

                minProgressionFactor = Math.Min(minProgressionFactor, playerHoursProgressionFactor);
            }

            return minProgressionFactor;
        }

        private double GetProgressionFactor(MinMaxConfig range, double value)
        {
            double fraction = (value - range.Min) / (range.Max - range.Min);
            double factor = SharedConfigHelpers.InterpolateForFirstCol(_config.CurrentConfig.ChanceProgressionRate, fraction);

            return factor;
        }
    }
}
