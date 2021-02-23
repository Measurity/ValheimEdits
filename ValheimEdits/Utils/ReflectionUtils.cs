using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace ValheimEdits.Utils
{
    public static class ReflectionUtils
    {
        public static Delegate CreateDelegate(this MethodInfo methodInfo, object target)
        {
            Func<Type[], Type> getType;
            var isAction = methodInfo.ReturnType == typeof(void);
            var types = methodInfo.GetParameters().Select(p => p.ParameterType);

            if (isAction)
            {
                getType = Expression.GetActionType;
            }
            else
            {
                getType = Expression.GetFuncType;
                types = types.Concat(new[] {methodInfo.ReturnType});
            }

            if (methodInfo.IsStatic)
            {
                return Delegate.CreateDelegate(getType(types.ToArray()), methodInfo);
            }
            return Delegate.CreateDelegate(getType(types.ToArray()), target, methodInfo.Name);
        }

        public static IEnumerable<CodeInstruction> GetInstructions(MethodInfo method)
        {
            if (method == null) throw new ArgumentNullException(nameof(method)); 
            
            DynamicMethod dynMethod = new DynamicMethod(method.Name, method.ReturnType, method.GetParameters().Select(p => p.ParameterType).ToArray(), false);
            return PatchProcessor.ReadMethodBody(method, dynMethod.GetILGenerator()).Select(pair => new CodeInstruction(pair.Key, pair.Value));
        }
    }
}
