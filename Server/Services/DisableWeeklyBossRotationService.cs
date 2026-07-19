using LiveLikeBossSpawnChances.Services.Internal;
using LiveLikeBossSpawnChances.Utils;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Spt.Config;
using SPTarkov.Server.Core.Servers;

namespace LiveLikeBossSpawnChances.Services
{
    [Injectable(TypePriority = OnLoadOrder.Database + LiveLikeBossSpawnChances_Server.LOAD_ORDER_OFFSET)]
    internal class DisableWeeklyBossRotationService : AbstractService
    {
        private ConfigServer _configServer;
        private BotConfig _botConfig;

        public DisableWeeklyBossRotationService(LoggingUtil logger, ConfigUtil config, ConfigServer configServer) : base(logger, config)
        {
            _configServer = configServer;
            _botConfig = _configServer.GetConfig<BotConfig>();
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
