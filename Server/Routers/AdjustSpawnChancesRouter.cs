using LiveLikeBossSpawnChances.Routers.Internal;
using LiveLikeBossSpawnChances.Utils;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Utils;

namespace LiveLikeBossSpawnChances.Routers
{
    [Injectable]
    internal class AdjustSpawnChancesRouter: AbstractStaticRouter
    {
        private const string GAME_STARTED_ENDPOINT = "/client/game/start";
        private const string CREATE_PROFILE_ENDPOINT = "/client/game/profile/create";
        private const string CHECK_PROFILE_STATUS_ENDPOINT = "/client/profile/status";

        private static readonly string[] _routeNames = [GAME_STARTED_ENDPOINT, CREATE_PROFILE_ENDPOINT, CHECK_PROFILE_STATUS_ENDPOINT];

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
            if (CanRefreshBossSpawnChances(routeName, routerData.SessionId))
            {
                _adjustSpawnChancesUtil.AdjustAllBossSpawnChances(routerData.SessionId);
            }

            return new ValueTask<string?>(routerData.Output);
        }

        private bool CanRefreshBossSpawnChances(string routeName, MongoId sessionId)
        {
            if (_adjustSpawnChancesUtil.AdjustmentsApplied)
            {
                // Only adjust spawn chances if no other profiles have connected since starting the server
                if (routeName == CREATE_PROFILE_ENDPOINT)
                {
                    return false;
                }

                // Only adjust spawn chances if enough time has elapsed since the last time they were adjusted
                if ((routeName == CHECK_PROFILE_STATUS_ENDPOINT) && !_adjustSpawnChancesUtil.CanRefreshByTime)
                {
                    return false;
                }

                // After the first player connects, only their profile can be used to adjust spawn chances
                if (!_adjustSpawnChancesUtil.IsSessionIdOfLastAdjustmentsApplied(sessionId))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
