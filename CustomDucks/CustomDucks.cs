using Custom_Ducks;
using MelonLoader;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

[assembly: MelonInfo(typeof(CustomDucks), "Custom Ducks", "0.1.0", "BlackyFox", "https://github.com/KitsueFox/PPDS-Mods")]
[assembly: MelonGame("Turbolento Games", "Placid Plastic Duck Simulator")]
namespace Custom_Ducks
{
    public class CustomDucks : MelonMod
    {
        public static CustomDucks Instance;
        private static GameObject _testDuck;
        private static GeneralManager _generalManager;
        private static readonly string Path = @"./UserData/CustomDucks/";

        public override void OnEarlyInitializeMelon()
        {
            Instance = this;
        }

        public override void OnInitializeMelon()
        {
            var harmony = new HarmonyLib.Harmony("Custom_Ducks");
            try
            {
                harmony.PatchAll(typeof(CustomDucksPatch.DuckEntryPatch));
                Instance.LoggerInstance.Msg("DuckEntry Patched!");
            }
            catch (Exception e)
            {
                Instance.LoggerInstance.Msg(e);
                throw;
            }
            Directory.CreateDirectory("./UserData/CustomDucks");
            LoadAssets();
        }

        public override void OnLateInitializeMelon()
        {
            base.OnLateInitializeMelon();
        }

        public override void OnLateUpdate()
        {
            var intro = SceneManager.GetActiveScene().name == "Intro";
            GameObject newduck;

            if (intro) return; // Check if Intro Scene is not loaded
            _generalManager = Object.FindObjectOfType<GeneralManager>();
            if (_generalManager == null) return; // Check if GeneralManager is Null
            if (Input.GetKeyDown(KeyCode.C))
            {
                newduck = Object.Instantiate(_testDuck);
                var duckManager = newduck.GetComponent<DuckManager>();
                _generalManager.AddDuck(duckManager, "mr.quacks", false, true);
                newduck.transform.position = _generalManager.SpawnPoint.position;
            }
        }

        private void LoadAssets()
        {
            var bundles = AssetBundle.LoadFromMemory(File.ReadAllBytes(Path + @"custom-set-1.ducks"));
            if (bundles != null)
            {
                Instance.LoggerInstance.Msg(bundles.name + " Loaded");
                bundles.LoadAllAssets();
                bundles.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                _testDuck = bundles.LoadAsset<GameObject>("mr.quacks");
                foreach (var CustomDuck in bundles.LoadAllAssets())
                {
                    if (CustomDuck != null)
                    {

                    }
                }
            }
            else
            {
                Instance.LoggerInstance.Error("Ducks Failed to load");
            }
        }
    }
}