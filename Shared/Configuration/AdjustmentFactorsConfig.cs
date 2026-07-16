using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace LiveLikeBossSpawnChances.Configuration
{
    [DataContract]
    public class AdjustmentFactorsConfig
    {
        [DataMember(Name = "player_level", IsRequired = true)]
        public bool PlayerLevel { get; set; } = true;

        [DataMember(Name = "player_hours", IsRequired = true)]
        public bool PlayerHours { get; set; } = true;

        public AdjustmentFactorsConfig()
        {

        }
    }
}
