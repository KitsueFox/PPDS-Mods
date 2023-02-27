using MelonLoader;
using Newtonsoft.Json;
using HarmonyLib;
using Custom_Names;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
using System;

[assembly: MelonInfo(typeof(CustomNames), "CustomNames", "0.1.0", "BlackyFox", "https://github.com/KitsueFox/PPDS-Mods")]
[assembly: MelonGame("Turbolento Games", "Placid Plastic Duck Simulator")]


namespace Custom_Names
{
    public class CustomNames : MelonMod
    {
        public static CustomNames _instance;

        private static readonly KeyCode changename = KeyCode.Keypad0;
        private static GeneralManager _generalManager;
        private static Saves _saves;
        private static string _savePath = "./UserData/CustomNames.json";
        private static string _savecontent;
        public static Dictionary<string, string> _duckNames = new Dictionary<string, string>();
        public static Action<string, string> _duckNameChanged;

        public override void OnEarlyInitializeMelon()
        {
            _instance = this;
        }

        public override void OnInitializeMelon()
        {
            if (!File.Exists(_savePath)) 
            { 
                File.Create(_savePath); 
            }
            else
            {
                _savecontent = File.ReadAllText(_savePath);
                _duckNames = JsonConvert.DeserializeObject<Dictionary<string, string>>(_savecontent);
            }
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
            //Scene
            if ("Intro" != sceneName)
            {
                _generalManager = Object.FindObjectOfType<GeneralManager>();
                if (_generalManager == null)
                {
                    _instance.LoggerInstance.Error("General Manager Didn't Hook!!");
                    return;
                }
                _saves = Object.FindObjectOfType<Saves>();
                if (_saves == null)
                {
                    _instance.LoggerInstance.Error("SavesManager did not hook");
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
            Action<string, string> duckNameChanged = _duckNameChanged;
            if (duckNameChanged != null)
            {
                duckNameChanged(duckID, newduckName);
            }
            _savecontent = JsonConvert.SerializeObject(_duckNames, Formatting.Indented);
            File.WriteAllText(_savePath, _savecontent);
        }

        //Harmony Patches
        [HarmonyPatch(typeof(GeneralManager), "AddDuck")]
        public class GeneralManager_AddDuck
        {
            static void Postfix(DuckManager duckManager, string duckID, bool unlock = true, bool addToList = true)
            {
                var duckNames = CustomNames.GetName(duckID);
                duckManager.NameChanged(duckID, duckNames);
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
