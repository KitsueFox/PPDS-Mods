using Custom_Names;
using HarmonyLib;
using MelonLoader;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

[assembly: MelonInfo(typeof(CustomNames), "CustomNames", "0.1.1", "BlackyFox", "https://github.com/KitsueFox/PPDS-Mods")]
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
        public static bool AutoName;
        public static bool NewDuck = false;
        public static Dictionary<string, string> _duckNames = new();
        public static Dictionary<string, int> _ducks = new();
        public static DuckManager currentduck;

        public override void OnEarlyInitializeMelon()
        {
            _instance = this;
        }

        public override void OnInitializeMelon()
        {
            CustomNameSettings.RegisterSettings();
            AutoName = CustomNameSettings.AutoName.Value;

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
                _instance.LoggerInstance.Msg("General Manager Add Patched!");

                //harmony.PatchAll(typeof(GeneralManager_Start));
                //_instance.LoggerInstance.Msg("General Manager Start Patched!");

                harmony.PatchAll(typeof(SpawnUpdate_Patch));
                _instance.LoggerInstance.Msg("Spawn Update Patched!");
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
            
            if (intro) { return; } // Check if Intro Scene is not loaded
            if (Input.GetKeyDown(changename)) { DuckRename(); }
            
            //Duck renaming can only occur OnLateUpdate, otherwise it won't rename when spawned! - Thank you pladisdev for solving this issue!
            if (currentduck != null)
            {
                DuckRename(currentduck);
                currentduck = null;
            }
        }

        public static void DuckRename(DuckManager duckManager = null, string duckName = null) //Void can be used for other mods
        {
            if (duckName == null)
            {
                var clipboard = GUIUtility.systemCopyBuffer;
                if (clipboard == null) { return; }
                duckName = clipboard;
            }
            if (duckManager == null)
            {
                if (_generalManager == null) { _instance.LoggerInstance.Error("Something went wrong"); return; }
                var currentduck = _generalManager.Ducks[_generalManager.CurrentDuck].GetComponent<DuckManager>();
                if (currentduck == null) { _instance.LoggerInstance.Error("Duck Not selected"); return; }
                duckManager = currentduck;
            }
            duckManager.NameChanged(duckManager.duckID, duckName);
            _instance.LoggerInstance.Msg("Duck: " + duckManager.duckID + " | Duck Name: " + duckName);
            Apply_Name(duckManager.duckID, duckName);
        }

        public static string GetName(string DuckID) //Void can be used for other mods
        {
            if (_duckNames.TryGetValue(DuckID, out string duckname)) { return duckname; }
            return null;
        }

        public static void Apply_Name(string duckID, string newduckName) //Void can be used for other mods
        {
            if (_duckNames.ContainsKey(duckID))
            {
                _duckNames[duckID] = newduckName;
            }
            else
            {
                _duckNames.Add(duckID, newduckName);
            }
            _savecontent = JsonConvert.SerializeObject(_duckNames, Formatting.Indented);
            File.WriteAllText(_savePath, _savecontent);
        }

        //Harmony Patches
        [HarmonyPatch(typeof(GeneralManager), "AddDuck")]
        public class GeneralManager_AddDuck
        {
            //TODO: Fix Add 2
            static void Postfix(ref GeneralManager __instance ,DuckManager duckManager, string duckID, bool unlock = true, bool addToList = true)
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
                if (CustomNames.NewDuck && CustomNames.AutoName)
                {
                    CustomNames._instance.LoggerInstance.Msg("New Duck!");
                    CustomNames.NewDuck = false;
                    CustomNames.currentduck = duckManager;
                }
                else
                {
                    var duckNames = CustomNames.GetName(modifiedDuckID);
                    duckManager.NameChanged(modifiedDuckID, duckNames);
                }
            }
        }
        /*[HarmonyPatch(typeof(GeneralManager), "Start")]
        public class GeneralManager_Start
        {
            static void Postfix(ref DuckManager ___base1Duck)
            {
                var duckNames = CustomNames.GetName("Duck1Base");
                ___base1Duck.NameChanged("Duck1Base", duckNames);
            }
        }*/
        [HarmonyPatch(typeof(GeneralManager), "SpawnUpdate")]
        public class SpawnUpdate_Patch
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
            {
                var code = new List<CodeInstruction>(instructions);

                int insertionIndex = -1;
                Label returnDuck = il.DefineLabel();
                for (int i = 0; i < code.Count - 1; i++)
                {
                    if (code[i].opcode == OpCodes.Stfld && code[i+1].opcode == OpCodes.Ldarg_0)
                    {
                        insertionIndex = i;
                        code[i].labels.Add(returnDuck);
                        break;
                    }
                }

                var instructionsToInsert = new List<CodeInstruction>();
                //
                // CustomNames.NewDuck = True
                //
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldc_I4_1, (sbyte)4));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Stsfld, AccessTools.Field(typeof(CustomNames), "NewDuck")));

                if (insertionIndex != -1)
                {
                    code.InsertRange(insertionIndex, instructionsToInsert);
                }

                return code;
            }
        }
    }
}
