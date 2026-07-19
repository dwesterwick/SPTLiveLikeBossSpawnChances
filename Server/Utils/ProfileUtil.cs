using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Helpers;
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

            long pmcSecondsInRaid = pmcData?.Stats?.Eft?.TotalInGameTime ?? 0;
            long scavSecondsInRaid = scavData?.Stats?.Eft?.TotalInGameTime ?? 0;
            double totalHoursInRaid = (pmcSecondsInRaid + scavSecondsInRaid) / 3600.0;

            return Math.Round(totalHoursInRaid, 2);
        }
    }
}
