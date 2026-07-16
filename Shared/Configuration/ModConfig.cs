using System.Runtime.Serialization;

namespace DansDevTools.Configuration
{
    [DataContract]
    public class ModConfig
    {
        [DataMember(Name = "enabled", IsRequired = true)]
        public bool Enabled { get; set; } = false;

        [DataMember(Name = "debug", IsRequired = true)]
        public bool Debug { get; set; } = false;

        [DataMember(Name = "scav_cooldown_time", IsRequired = true)]
        public int ScavCooldownTime { get; set; } = 1500;

        [DataMember(Name = "free_labs_access", IsRequired = true)]
        public bool FreeLabsAccess { get; set; } = false;

        [DataMember(Name = "free_labyrinth_access", IsRequired = true)]
        public bool FreeLabyrinthAccess { get; set; } = false;

        [DataMember(Name = "min_level_for_flea", IsRequired = true)]
        public int MinLevelForFlea { get; set; } = 15;

        [DataMember(Name = "full_length_scav_raids", IsRequired = true)]
        public bool FullLengthScavRaids { get; set; } = false;

        [DataMember(Name = "always_have_airdrops", IsRequired = true)]
        public bool AlwaysHaveAirdrops { get; set; } = false;

        [DataMember(Name = "bosses_always_spawn", IsRequired = true)]
        public bool BossesAlwaysSpawn { get; set; } = false;

        [DataMember(Name = "season_always_summer", IsRequired = true)]
        public bool SeasonAlwaysSummer { get; set; } = false;

        public ModConfig()
        {

        }
    }
}
