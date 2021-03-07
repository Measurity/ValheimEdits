using HarmonyLib;
using UnityEngine;
using ValheimEdits.Serialization;

namespace ValheimEdits.Patches
{
    public class SkillsPatch
    {
        [HarmonyPatch(typeof(Skills), nameof(Skills.OnDeath))]
        public static class OnDeath
        {
            public static bool Prefix()
            {
                Debug.Log($"Skill loss enabled: {(Config.Instance.SkillLossOnDeath ? "true" : "false")}");
                return Config.Instance.SkillLossOnDeath;
            }
        }
    }
}
