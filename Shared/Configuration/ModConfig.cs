using System.Runtime.Serialization;

namespace LiveLikeBossSpawnChances.Configuration
{
    [DataContract]
    public class ModConfig
    {
        [DataMember(Name = "enabled", IsRequired = true)]
        public bool Enabled { get; set; } = false;

        [DataMember(Name = "debug", IsRequired = true)]
        public DebugConfig Debug { get; set; } = new DebugConfig();

        [DataMember(Name = "adjustment_factors", IsRequired = true)]
        public AdjustmentFactorsConfig AdjustmentFactors { get; set; } = new AdjustmentFactorsConfig();

        [DataMember(Name = "adjustments_disabled_after_player_level", IsRequired = true)]
        public int AdjustmentsDisabledAfterPlayerLevel { get; set; } = 40;

        [DataMember(Name = "thresholds", IsRequired = true)]
        public ThresholdsConfig Thresholds { get; set; } = new ThresholdsConfig();

        [DataMember(Name = "ignored_bosses", IsRequired = true)]
        public Dictionary<string, string[]> IgnoredBosses { get; set; } = new Dictionary<string, string[]>();

        [DataMember(Name = "blocked_bosses", IsRequired = true)]
        public string[] BlockedBosses { get; set; } = [];

        [DataMember(Name = "chance_progression_rate", IsRequired = true)]
        public double[][] ChanceProgressionRate { get; set; } = Array.Empty<double[]>();

        public ModConfig()
        {

        }
    }
}
