using Custom_Names;
using HarmonyLib;
using MelonLoader;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

[assembly: MelonInfo(typeof(CustomNames), "CustomNames", "0.1.0", "BlackyFox", "https://github.com/KitsueFox/PPDS-Mods")]
[assembly: MelonGame("Turbolento Games", "Placid Plastic Duck Simulator")]


namespace Custom_Names
{
    public class CustomNames : MelonMod
    {
        public static CustomNames _instance;

        private static readonly KeyCode changename = KeyCode.Keypad0;
        private static GeneralManager _generalManager;
        private static readonly string _savePath = "./UserData/CustomNames.json";
        private static string _savecontent;
        public static Dictionary<string, string> _duckNames = new();
        public static Dictionary<string, int> _ducks = new();
        public static Action<string, string> _duckNameChanged;

        public override void OnEarlyInitializeMelon()
        {
            _instance = this;
        }

        public override void OnInitializeMelon()
        {
            //Save File
            if (!File.Exists(_savePath))
            {
                File.Create(_savePath);
            }
            else
            {
                _savecontent = File.ReadAllText(_savePath);
                _duckNames = JsonConvert.DeserializeObject<Dictionary<string, string>>(_savecontent);
            }

            //Harmony Patching
            var harmony = new HarmonyLib.Harmony("Custom_Names");
            try
            {
                harmony.PatchAll(typeof(GeneralManager_AddDuck));
                _instance.LoggerInstance.Msg("General Manager Start Patched!");

                harmony.PatchAll(typeof(GeneralManager_Start));
                _instance.LoggerInstance.Msg("General Manager Add Patched!");
            }
            catch (Exception e)
            {

                _instance.LoggerInstance.Msg(e);
            }
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            //Hook GeneralManager
            if ("Intro" != sceneName)
            {
                _ducks.Clear();
                _generalManager = Object.FindObjectOfType<GeneralManager>();
                if (_generalManager == null)
                {
                    _instance.LoggerInstance.Error("General Manager Didn't Hook!!");
                    return;
                }
            }
        }

        public override void OnLateUpdate()
        {
            var intro = SceneManager.GetActiveScene().name == "Intro";

            if (intro) return; // Check if Intro Scene is not loaded
            if (Input.GetKeyDown(changename)) { SetName(); }
        }

        private static void SetName()
        {
            var clipboard = GUIUtility.systemCopyBuffer;
            if (clipboard == null) return;
            if (_generalManager == null) { _instance.LoggerInstance.Error("Something went wrong"); return; }
            var currentduck = _generalManager.Ducks[_generalManager.CurrentDuck].GetComponent<DuckManager>();
            if (currentduck == null) { _instance.LoggerInstance.Error("Duck Not selected"); return; }
            currentduck.NameChanged(currentduck.duckID, clipboard);
            _instance.LoggerInstance.Msg("Duck: " + currentduck.duckID + " | Duck Name: " + clipboard);

            Apply_Name(currentduck.duckID, clipboard);
        }

        public static string GetName(string DuckID)
        {
            if (_duckNames.TryGetValue(DuckID, out string duckname)) { return duckname; }
            return null;
        }

        private static void Apply_Name(string duckID, string newduckName)
        {
            if (_duckNames.ContainsKey(duckID))
            {
                _duckNames[duckID] = newduckName;
            }
            else
            {
                _duckNames.Add(duckID, newduckName);
            }
            _duckNameChanged?.Invoke(duckID, newduckName);
            _savecontent = JsonConvert.SerializeObject(_duckNames, Formatting.Indented);
            File.WriteAllText(_savePath, _savecontent);
        }

        //Harmony Patches
        [HarmonyPatch(typeof(GeneralManager), "AddDuck")]
        public class GeneralManager_AddDuck
        {
            static void Postfix(ref GeneralManager __instance ,DuckManager duckManager, string duckID, bool unlock = true, bool addToList = true) //TODO: Fix Add 2
            {
                int count = 0;
                if (CustomNames._ducks.ContainsKey(duckID))
                {
                    count = CustomNames._ducks[duckID];
                    CustomNames._ducks[duckID]++;
                }
                else
                {
                    CustomNames._ducks[duckID] = 1;
                }
                string modifiedDuckID = duckID;
                if (count > 0)
                {
                    modifiedDuckID += "_" + count;
                }
                duckManager.duckID = modifiedDuckID;
                var duckNames = CustomNames.GetName(modifiedDuckID);
                duckManager.NameChanged(modifiedDuckID, duckNames);
            }
        }
        [HarmonyPatch(typeof(GeneralManager), "Start")]
        public class GeneralManager_Start
        {
            static void Postfix(ref DuckManager ___base1Duck)
            {
                var duckNames = CustomNames.GetName("Duck1Base");
                ___base1Duck.NameChanged("Duck1Base", duckNames);
            }
        }
    }
}
