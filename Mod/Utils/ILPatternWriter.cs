using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace ValheimEdits.Utils
{
    public class ILPatternWriter
    {
        private readonly List<Func<CodeInstruction, bool>> pattern;
        private readonly List<Func<CodeInstruction, CodeInstruction[]>> writes;

        private ILPatternWriter(List<Func<CodeInstruction, bool>> pattern,
            List<Func<CodeInstruction, CodeInstruction[]>> writes)
        {
            this.pattern = pattern;
            this.writes = writes;
        }

        public static ILPatternWriterBuilder Builder => new();

        public int FindPatternIndex(List<CodeInstruction> il)
        {
            for (var i = 0; i < il.Count - pattern.Count; i++)
            {
                var hasFullMatch = true;
                foreach (var instMatcher in pattern)
                {
                    if (!instMatcher(il[i]))
                    {
                        hasFullMatch = false;
                        break;
                    }
                }
                if (hasFullMatch)
                {
                    return i;
                }
            }

            return -1;
        }

        public bool Apply(List<CodeInstruction> il)
        {
            var patternStart = FindPatternIndex(il);
            if (patternStart < 0)
            {
                return false;
            }

            for (int i = 0; i < il.Count; i++)
            {
                var newIlForCurrentIl = writes[i](il[patternStart + i]);
                for (int j = 0; j < newIlForCurrentIl.Length; j++)
                {
                    var newIl = newIlForCurrentIl[j];
                    newIl = newIl switch
                    {
                        null => new CodeInstruction(OpCodes.Nop),
                        _ => newIl
                    };
                    
                    if (j == 0)
                    {
                        il[patternStart + i + j] = newIl;
                    }
                    else if (j > 0)
                    {
                        il.Insert(patternStart + i + j, newIl);
                    }
                }
            }

            return true;
        }

        public class ILPatternWriterBuilder
        {
            private readonly List<Func<CodeInstruction, bool>> pattern = new();
            private readonly List<Func<CodeInstruction, CodeInstruction[]>> writes = new();

            public ILPatternWriterBuilder Match(params Func<CodeInstruction, bool>[] parts)
            {
                pattern.AddRange(parts);
                return this;
            }

            public ILPatternWriterBuilder Match(Func<CodeInstruction, bool> part)
            {
                pattern.Add(part);
                return this;
            }

            public ILPatternWriterBuilder Write(CodeInstruction newInstruction)
            {
                writes.Add(_ => new[] {newInstruction});
                return this;
            }

            public ILPatternWriterBuilder Nop(int skipIl = 1)
            {
                for (var i = 0; i < skipIl; i++)
                {
                    writes.Add(null);
                }
                return this;
            }

            public ILPatternWriterBuilder Write(Action<CodeInstruction> transform)
            {
                writes.Add(inst =>
                {
                    transform(inst);
                    return new[] {inst};
                });
                return this;
            }

            public ILPatternWriterBuilder Write(Func<CodeInstruction, CodeInstruction> transform)
            {
                writes.Add(inst => new[] {transform(inst)});
                return this;
            }

            public ILPatternWriterBuilder Write(Func<CodeInstruction, CodeInstruction[]> transform)
            {
                writes.Add(transform);
                return this;
            }

            public ILPatternWriter Build() => new(pattern, writes);
        }
    }
}
