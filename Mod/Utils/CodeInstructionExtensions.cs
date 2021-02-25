using System;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace ValheimEdits.Utils
{
    public static class CodeInstructionExtensions
    {
        public static bool IsGetThis(this CodeInstruction inst) => inst.opcode == OpCodes.Ldarg_0;

        public static bool IsGetField(this CodeInstruction inst, Type type, string fieldName) =>
            inst.opcode == OpCodes.Ldfld && ReferenceEquals(inst.operand, AccessTools.Field(type, fieldName));

        public static bool IsJumpIfFalse(this CodeInstruction inst) =>
            inst.opcode == OpCodes.Brfalse || inst.opcode == OpCodes.Brfalse_S;

        public static bool IsJumpIfTrue(this CodeInstruction inst) =>
            inst.opcode == OpCodes.Brtrue || inst.opcode == OpCodes.Brtrue_S;

        public static bool IsMethod(this CodeInstruction inst, string methodName) =>
            inst.IsCall() && (inst.operand as MethodInfo)?.Name.Equals(methodName) == true;

        public static bool IsMethod(this CodeInstruction inst, params string[] methodNames)
        {
            if (!inst.IsCall())
            {
                return false;
            }
            var method = inst.operand as MethodInfo;
            if (method == null)
            {
                return false;
            }

            foreach (var name in methodNames)
            {
                if (method.Name.Equals(name))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsCall(this CodeInstruction inst) =>
            inst.opcode == OpCodes.Call || inst.opcode == OpCodes.Calli || inst.opcode == OpCodes.Callvirt;

        public static bool IsCall(this CodeInstruction inst, Type type, string methodName) =>
            inst.IsCall() && inst.OperandIs(AccessTools.Method(type, methodName));
    }
}
