using LiveLikeBossSpawnChances.Routers.Internal;
using LiveLikeBossSpawnChances.Utils;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Utils;

namespace LiveLikeBossSpawnChances.Routers
{
    [Injectable]
    internal class AdjustSpawnChancesRouter: AbstractStaticRouter
    {
        private static readonly string[] _routeNames = [ "/client/game/start" ];

        private AdjustSpawnChancesUtil _adjustSpawnChancesUtil;

        public AdjustSpawnChancesRouter
        (
            LoggingUtil logger,
            ConfigUtil config,
            JsonUtil jsonUtil,
            AdjustSpawnChancesUtil adjustSpawnChancesUtil
        ) : base(_routeNames, logger, config, jsonUtil)
        {
            _adjustSpawnChancesUtil = adjustSpawnChancesUtil;
        }

        public override ValueTask<string?> HandleRoute(string routeName, RequestData routerData)
        {
            _adjustSpawnChancesUtil.AdjustAllBossSpawnChances(routerData.SessionId);

            return new ValueTask<string?>(routerData.Output);
        }
    }
}
