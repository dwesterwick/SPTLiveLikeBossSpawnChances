using LiveLikeBossSpawnChances.Configuration;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace LiveLikeBossSpawnChances.Helpers
{
    public static class SharedConfigHelpers
    {
        public static bool IsModEnabled(this ModConfig? modConfig) => modConfig?.Enabled == true;
        public static bool IsVerboseLoggingEnabled(this ModConfig? modConfig) => modConfig?.Debug?.VerboseLogging == true;

        public static double InterpolateForFirstCol(this double[][] array, double value)
        {
            ValidateArray(array);

            if (array.Length == 1)
            {
                return array.Last()[1];
            }

            if (value <= array[0][0])
            {
                return array[0][1];
            }

            for (int i = 1; i < array.Length; i++)
            {
                if (array[i][0] >= value)
                {
                    if (array[i][0] - array[i - 1][0] == 0)
                    {
                        return array[i][1];
                    }

                    return array[i - 1][1] + (value - array[i - 1][0]) * (array[i][1] - array[i - 1][1]) / (array[i][0] - array[i - 1][0]);
                }
            }

            return array.Last()[1];
        }

        public static void ValidateArray(this double[][] array)
        {
            if (array.Length == 0)
            {
                throw new ArgumentOutOfRangeException("The array must have at least one row.");
            }

            if (array.Any(x => x.Length != 2))
            {
                throw new ArgumentOutOfRangeException("All rows in the array must have two columns.");
            }
        }
    }
}
