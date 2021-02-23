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
    ///     Changes roof check when using crafting station.
    /// </summary>
    public class CraftingStationPatch
    {
        [HarmonyPatch(typeof(CraftingStation), nameof(CraftingStation.CheckUsable))]
        public static class CheckUsable
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var list = instructions.ToList();
                for (var i = 0; i < list.Count; i++)
                {
                    if (i + 2 < list.Count && list[i].IsGetThis())
                    {
                        if (list[i + 1].IsGetField(typeof(CraftingStation), nameof(CraftingStation.m_craftRequireRoof)))
                        {
                            if (list[i + 2].IsJumpIfFalse())
                            {
                                Debug.Log("Workbench requires roof check found!");
                                if (!Config.Instance.WorkbenchRequiresRoof)
                                {
                                    list[i].opcode = OpCodes.Nop;
                                    list[i + 1].opcode = OpCodes.Nop;
                                    list[i + 2].opcode = OpCodes.Br;                                    
                                }
                            }
                        }
                    }

                    yield return list[i];
                }
            }
        }
    }
}
