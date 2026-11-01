using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Helpers.Profile;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;

namespace LiveLikeBossSpawnChances.Utils
{
    [Injectable(InjectionType.Singleton)]
    public class ProfileUtil
    {
        private ProfileHelper _profileHelper;

        public ProfileUtil(ProfileHelper profileHelper)
        {
            _profileHelper = profileHelper;
        }

        public PmcData? GetPmcProfile(MongoId sessionId) => _profileHelper.GetPmcProfile(sessionId);
        public PmcData? GetScavProfile(MongoId sessionId) => _profileHelper.GetScavProfile(sessionId);

        public int? GetPmcLevel(MongoId sessionId)
        {
            PmcData? pmcData = GetPmcProfile(sessionId);
            return pmcData?.Info?.Level;
        }

        public double? GetPlayerHours(MongoId sessionId)
        {
            PmcData? pmcData = GetPmcProfile(sessionId);
            if (pmcData == null)
            {
                return null;
            }

            PmcData? scavData = GetScavProfile(sessionId);

            // SPT 3.11 tracking method
            long pmcTotalInGameTime = pmcData?.Stats?.Eft?.TotalInGameTime ?? 0;
            long scavTotalInGameTime = scavData?.Stats?.Eft?.TotalInGameTime ?? 0;
            double totalTotalInGameTimeHours = (pmcTotalInGameTime + scavTotalInGameTime) / 3600.0;

            // SPT 4.x tracking method
            long pmcLifeTime = pmcData?.Stats?.Eft?.OverallCounters?.Items?.FirstOrDefault(counter => counter.Key?.Contains("LifeTime") == true)?.Value ?? 0;
            long scavLifeTime = scavData?.Stats?.Eft?.OverallCounters?.Items?.FirstOrDefault(counter => counter.Key?.Contains("LifeTime") == true)?.Value ?? 0;
            double totalLifeTimeHours = (pmcLifeTime + scavLifeTime) / 3600.0;

            double highestInGameTime = Math.Max(totalTotalInGameTimeHours, totalLifeTimeHours);
            return Math.Round(highestInGameTime, 2);
        }
    }
}
