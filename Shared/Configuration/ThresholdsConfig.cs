using DansDevTools.Configuration;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace LiveLikeBossSpawnChances.Configuration
{
    [DataContract]
    public class ThresholdsConfig
    {
        [DataMember(Name = "adjustmentRange", IsRequired = true)]
        public MinMaxConfig AdjustmentRange { get; set; } = new MinMaxConfig(0.2, 1);

        [DataMember(Name = "player_level", IsRequired = true)]
        public MinMaxConfig PlayerLevel { get; set; } = new MinMaxConfig(10, 30);

        [DataMember(Name = "player_hours", IsRequired = true)]
        public MinMaxConfig PlayerHours { get; set; } = new MinMaxConfig(15, 60);

        public ThresholdsConfig()
        {

        }
    }
}
