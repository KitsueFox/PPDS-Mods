using HarmonyLib;
using MelonLoader;

[assembly: MelonInfo(typeof(EventReenable), "Event Reenabler", "0.1.1", "BlackyFox", "https://github.com/KitsueFox/PPDS-Mods")]
[assembly: MelonGame("Turbolento Games", "Placid Plastic Duck Simulator")]

    public class EventReenable : MelonMod
    {
        public override void OnInitializeMelon()
        {
            var harmony = new HarmonyLib.Harmony("Event_Reenable");
                harmony.PatchAll(typeof(SeasonalPatch_DLCManager));
        }
    }

    //Harmony Patch
    [HarmonyPatch(typeof(DLCManager), "SeasonalSetSetup")]
    public class SeasonalPatch_DLCManager
    {
        static bool Prefix(SetToggle ___seasonalSetToggle)
        {
            ___seasonalSetToggle.gameObject.SetActive(true);
            ___seasonalSetToggle.Setup(true);
            return false;
        }
    }