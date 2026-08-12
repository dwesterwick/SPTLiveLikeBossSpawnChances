using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Spt.Tables;

namespace LiveLikeBossSpawnChances.Helpers
{
    public static class DatabaseHelpers
    {
        public static Location GetAndVerifyLocation(this LocationTable locationTable, string locationId)
        {
            Location? location = locationTable.GetLocation(locationId);
            if (location == null)
            {
                throw new InvalidOperationException($"Cannot find location \"${locationId}\" in database.");
            }

            return location;
        }

        public static IEnumerable<Location> EnumerateLocations(this LocationTable locationTable)
        {
            return locationTable.GetDictionary().Values;
        }

        public static IEnumerable<string> EnumerateLocationIDs(this LocationTable locationTable)
        {
            return locationTable.GetDictionary().Keys;
        }

        public static bool Contains(this IEnumerable<string> ids, MongoId id) => ids.Any(id.Equals);
    }
}
