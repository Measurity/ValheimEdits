using System.Linq;
using BepInEx;
using HarmonyLib;

namespace ValheimEdits
{
    [BepInPlugin("com.github.measurity.valheimedits", "ValheimEdits", "1.0.0.0")]
    public class Mod : BaseUnityPlugin
    {
        private static readonly Harmony harmony = new(typeof(Mod).GetCustomAttributes(typeof(BepInPlugin), false)
            .Cast<BepInPlugin>()
            .First()
            .GUID);

        private void Awake()
        {
            Serialization.Config.Load();
            Serialization.Config.Instance.Save();
            harmony.PatchAll();
        }

        private void OnDestroy()
        {
            harmony.UnpatchAll();
        }
    }
}
