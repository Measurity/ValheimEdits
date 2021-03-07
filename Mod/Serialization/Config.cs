using System.ComponentModel;

namespace ValheimEdits.Serialization
{
    public class Config : ConfigBase<Config>
    {
        public Config() : base("config.xml")
        {
        }

        public bool WorkbenchRequiresRoof { get; set; } = true;

        [Description("If true, allows sleeping even if it is really inconvenient to character")]
        public bool HoboSleeping { get; set; } = false;

        public bool SkillLossOnDeath { get; set; } = true;
    }
}
