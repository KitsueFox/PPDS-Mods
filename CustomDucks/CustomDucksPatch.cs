using HarmonyLib;
using UnityEngine;

namespace Custom_Ducks
{
    public abstract class CustomDucksPatch
    {
        //Harmony Patches
        [HarmonyPatch(typeof(DuckEntry))]
        public class DuckEntryPatch
        {
            public bool isCustom;
            public GameObject duckCustomObj;
        }
    }
}