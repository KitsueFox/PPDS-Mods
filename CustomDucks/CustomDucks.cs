using System;
using System.Collections;
using System.Reflection;
using System.IO;
using System.Runtime.Remoting.Messaging;
using Custom_Ducks;
using MelonLoader;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

[assembly: MelonInfo(typeof(CustomDucks), "Custom Ducks", "0.1.0", "BlackyFox", "https://github.com/KitsueFox/PPDS-Mods")]
[assembly: MelonGame("Turbolento Games", "Placid Plastic Duck Simulator")]
namespace Custom_Ducks
{
    public class CustomDucks : MelonMod
    {
        private static CustomDucks _instance;
        private static GameObject _testDuck;
        private static GeneralManager _generalManager;
        private static AssetBundle assets;
        private static DuckSet custom_ducks;
        private static readonly string Path = @"./UserData/CustomDucks/";

        public override void OnEarlyInitializeMelon()
        {
            _instance = this;
        }

        public override void OnInitializeMelon()
        {
            Directory.CreateDirectory("./UserData/CustomDucks");
            LoadAssets();
        }

        private void LoadAssets()
        {
            var bundles = AssetBundle.LoadFromMemory(File.ReadAllBytes(Path + @"custom-set-1.ducks"));
            if (bundles != null)
            {
                _instance.LoggerInstance.Msg(bundles.name + " Loaded");
                _testDuck = bundles.LoadAsset<GameObject>("_CustomDuck");
            }
            else
            {
                _instance.LoggerInstance.Error("Ducks Failed to load");
            }

        }

        public override void OnLateUpdate()
        {
            var intro = SceneManager.GetActiveScene().name == "Intro";
            
            if (intro) return; // Check if Intro Scene is not loaded
            _generalManager = Object.FindObjectOfType<GeneralManager>();
            if (_generalManager == null) return; // Check if GeneralManager is Null
            if (Input.GetKeyDown(KeyCode.C))
            {
                Object.Instantiate(_testDuck, _generalManager.SpawnPoint);
                var duckmanager = _testDuck.GetComponent<DuckManager>() as DuckManager;
                _generalManager.AddDuck(duckmanager, "_CustomDuck", true,true);
            }
        }
    }
}