using LiveLikeBossSpawnChances.Helpers;
using LiveLikeBossSpawnChances.Services.Internal;
using LiveLikeBossSpawnChances.Utils;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;

namespace LiveLikeBossSpawnChances.Services
{
    [Injectable(TypePriority = OnLoadOrder.PostSptModLoader + LiveLikeBossSpawnChances_Server.LOAD_ORDER_OFFSET)]
    internal class DebugService : AbstractService
    {
        private AdjustSpawnChancesUtil _adjustSpawnChancesUtil;

        public DebugService(LoggingUtil logger, ConfigUtil config, AdjustSpawnChancesUtil adjustSpawnChancesUtil) : base (logger, config)
        {
            _adjustSpawnChancesUtil = adjustSpawnChancesUtil;
        }

        protected override void OnLoadIfModIsEnabled()
        {
            if (!DebugHelpers.IsDebugConfiguration())
            {
                return;
            }

            Logger.Warning("DEBUG: Adjusting boss spawn chances using default player values...");
            _adjustSpawnChancesUtil.AdjustAllBossSpawnChancesUsingDefaultPlayerValues();
        }
    }
}
