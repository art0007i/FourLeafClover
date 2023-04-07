using HarmonyLib;
using NeosModLoader;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using FrooxEngine;
using System.Reflection.Emit;

namespace FourLeafClover
{
    // Apparently C# doesn't allow emojis in class names. Unfortunate
    public class FourLeafClover : NeosMod
    {
        public override string Name => "🍀";
        public override string Author => "art0007i";
        public override string Version => "1.0.0";
        public override string Link => "https://github.com/art0007i/FourLeafClover/";

        [AutoRegisterConfigKey]
        public static ModConfigurationKey<bool> KEY_ENABLED = new("enabled", "Makes you luckier, by using the power of 🍀", ()=>true);
        public static ModConfiguration config;

        public override void OnEngineInit()
        {
            config = GetConfiguration();
            Harmony harmony = new Harmony("me.art0007i.🍀");
            harmony.PatchAll();
        }


        [HarmonyPatch]
        class FourLeafCloverPatch
        {
            static IEnumerable<MethodBase> TargetMethods()
            {
                var func = AccessTools.Method(typeof(BaseX.CollectionsExtensions), nameof(BaseX.CollectionsExtensions.GetRandomWithWeight));
                yield return func.MakeGenericMethod(new Type[] { typeof(object) });
            }
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codes)
            {
                var lookForAdd = false;
                foreach (var code in codes)
                {
                    if (code.opcode == OpCodes.Ldarg_1)
                    {
                        lookForAdd = true;
                    }
                    if(lookForAdd && code.opcode == OpCodes.Add)
                    {
                        lookForAdd = false;
                        var cc = new CodeInstruction(OpCodes.Call, typeof(FourLeafCloverPatch).GetMethod(nameof(ProcessNumber)));
                        yield return cc;
                    }
                    yield return code;
                }
            }

            public static float ProcessNumber(float f)
            {
                if (!config.GetValue(KEY_ENABLED)) return f;
                return f == 0 ? 0 : 1 / f;
            }
        }
    }
}