using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;
using ValheimEdits.Serialization;
using ValheimEdits.Utils;

namespace ValheimEdits.Patches
{
    /// <summary>
    ///     Changes hardcoded data rate limit to fix lag issues.
    /// </summary>
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
