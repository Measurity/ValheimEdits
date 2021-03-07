using BepInEx;
using HarmonyLib;

namespace ValheimEdits
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public class Mod : BaseUnityPlugin
    {
        public const string PluginAuthor = "Measurity";
        public const string PluginGuid = "com.github.measurity.valheimedits";
        public const string PluginName = "ValheimEdits";
        public const string PluginVersion = "1.1.0.0";

        private static readonly Harmony harmony = new(PluginGuid);

        private void Awake()
        {
            harmony.PatchAll();
        }

        private void OnDestroy()
        {
            harmony.UnpatchSelf();
        }
    }
}
