using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace LiveLikeBossSpawnChances.Configuration
{
    [DataContract]
    public class DebugConfig
    {
        [DataMember(Name = "verbose_logging", IsRequired = true)]
        public bool VerboseLogging { get; set; } = false;

        [DataMember(Name = "default_player_level", IsRequired = true)]
        public int DefaultPlayerLevel { get; set; } = 70;

        [DataMember(Name = "default_player_hours", IsRequired = true)]
        public int DefaultPlayerHours { get; set; } = 2000;

        public DebugConfig()
        {

        }
    }
}
