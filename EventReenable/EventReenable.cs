using Event_Reenable;
using HarmonyLib;
using MelonLoader;
using System;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;

[assembly: MelonInfo(typeof(EventReenable), "Event Reenabler", "0.1.1", "BlackyFox", "https://github.com/KitsueFox/PPDS-Mods")]
[assembly: MelonGame("Turbolento Games", "Placid Plastic Duck Simulator")]

namespace Event_Reenable
{

    public class EventReenable : MelonMod
    {
        internal static EventReenable _instance { get; set; }

        public override void OnEarlyInitializeMelon()
        {
            _instance = this;
        }

        public override void OnInitializeMelon()
        {
            var harmony = new HarmonyLib.Harmony("Event_Reenable");
            try
            {
                harmony.PatchAll(typeof(SeasonalPatch_DLCManager));
                _instance.LoggerInstance.Msg("DLCManager Patched!");
            }
            catch (Exception e)
            {

                _instance.LoggerInstance.Msg(e);
            }
        }
    }

    //Harmony Patch
    [HarmonyPatch(typeof(DLCManager), "SeasonalSetSetup")]
    public class SeasonalPatch_DLCManager
    {
        static bool Prefix(ref DLCManager __instance, SetToggle ___seasonalSetToggle)
        {
            ___seasonalSetToggle.gameObject.SetActive(true);
            ___seasonalSetToggle.Setup(true);
            return false;
        }
    }
}