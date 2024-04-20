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
        public static EventReenable _instance;

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

                harmony.PatchAll(typeof(SeasonalPatch_GeneralManager));
                _instance.LoggerInstance.Msg("GeneralManager Patched!");
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

    [HarmonyPatch(typeof(GeneralManager), "SetSeasonalSpawn")]
    public class SeasonalPatch_GeneralManager //TODO: Try to find a better way of Fixing the Keyname Error
    {
        static void Prefix(DuckSet seasonalSet, List<Dictionary<string, AssetReference>> ducksPerRarity)
        {
            List<DuckEntry> commonDucks = seasonalSet.GetCommonDucks();
            for (int i = 0; i < commonDucks.Count; i++)
            {
                try
                {
                    ducksPerRarity[0].Add(commonDucks[i].duckID, commonDucks[i].duckRef);
                }
                catch (Exception e)
                {
                    EventReenable._instance.LoggerInstance.Msg("\x1b[32m[HarmonyPatch] " + e);
                }
            }
        }
    }
}