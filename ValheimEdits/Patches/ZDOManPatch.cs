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
    public class ZDOManPatch
    {
        [HarmonyPatch(typeof(ZDOMan), nameof(ZDOMan.SendZDOs))]
        public static class SendZDOs
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var list = instructions.ToList();
                for (var i = 0; i < list.Count; i++)
                {
                    if (i + 1 < list.Count && list[i].IsGetThis())
                    {
                        if (list[i + 1].IsGetField(typeof(ZDOMan), nameof(ZDOMan.m_dataPerSec)))
                        {
                            Debug.Log("DataRateLimit check found!");
                            list[i].opcode = OpCodes.Nop;
                            list[i + 1] = new CodeInstruction(OpCodes.Ldc_I4, Config.Instance.DataRateLimit);
                        }
                    }

                    yield return list[i];
                }
            }
        }
    }
}
