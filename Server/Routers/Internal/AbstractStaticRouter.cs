using LiveLikeBossSpawnChances.Utils;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Utils;

namespace LiveLikeBossSpawnChances.Routers.Internal
{
    internal abstract class AbstractStaticRouter : AbstractTypedStaticRouter<EmptyRequestData>
    {
        public AbstractStaticRouter(IEnumerable<string> _routeNames, LoggingUtil logger, ConfigUtil config, JsonUtil jsonUtil)
            : base(_routeNames, logger, config, jsonUtil)
        {

        }
    }
}
