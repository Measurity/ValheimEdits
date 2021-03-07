using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using ValheimEdits.Serialization;
using ValheimEdits.Utils;

namespace ValheimEdits.Patches
{
    public class BedPatch
    {
        [HarmonyPatch(typeof(Bed), nameof(Bed.Interact))]
        public static class Interact
        {
            private static readonly string[] canSleepCheckMethods =
            {
                nameof(Bed.CheckEnemies),
                nameof(Bed.CheckExposure),
                nameof(Bed.CheckFire),
                nameof(Bed.CheckWet)
            };

            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var list = instructions.ToList();
                for (var i = 0; i < list.Count; i++)
                {
                    if (i + 3 < list.Count)
                    {
                        if (list[i].IsGetThis())
                        {
                            if (list[i + 1].IsLdloc())
                            {
                                if (Config.Instance.HoboSleeping && list[i + 2].IsMethod(canSleepCheckMethods))
                                {
                                    if (list[i + 3].IsJumpIfTrue())
                                    {
                                        // Always pass check
                                        list[i].opcode = OpCodes.Nop;
                                        list[i + 1].opcode = OpCodes.Nop;
                                        list[i + 2].opcode = OpCodes.Nop;
                                        list[i + 3].opcode = OpCodes.Br;
                                    }
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
