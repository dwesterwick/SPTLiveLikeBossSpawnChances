using LiveLikeBossSpawnChances.Services.Internal;
using LiveLikeBossSpawnChances.Utils;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Spt.Config;

namespace LiveLikeBossSpawnChances.Services
{
    [Injectable(TypePriority = OnLoadOrder.Preload + LiveLikeBossSpawnChances_Server.LOAD_ORDER_OFFSET)]
    internal class DisableWeeklyBossRotationService : AbstractService
    {
        private BotConfig _botConfig;

        public DisableWeeklyBossRotationService(LoggingUtil logger, ConfigUtil config, BotConfig botConfig) : base(logger, config)
        {
            _botConfig = botConfig;
        }

        protected override void OnLoadIfModIsEnabled()
        {
            if (!Config.CurrentConfig.DisableWeeklyBossRotation)
            {
                return;
            }

            Logger.Info("Disabling SPT's weekly boss rotation...");
            _botConfig.WeeklyBoss.Enabled = false;
        }
    }
}
