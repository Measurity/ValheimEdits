using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;

namespace ValheimEdits.Utils
{
    public static class MethodInfoExtensions
    {
        public static IEnumerable<CodeInstruction> AsInstructions(this MethodInfo method)
        {
            return ReflectionUtils.GetInstructions(method);
        }
    }
}
