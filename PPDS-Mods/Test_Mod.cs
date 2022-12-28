using HarmonyLib;
using MelonLoader;
using TestMod;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Security.Policy;

[assembly: MelonInfo(typeof(Test_Mod), "Test Mod", "0.0.1", "BlackyFox")]
[assembly: MelonGame("Turbolento Games", "Placid Plastic Duck Simulator")]

namespace TestMod
{
    public class Test_Mod : MelonMod
    {
        public static Test_Mod instance;

        private static KeyCode spawnduck;
        private static KeyCode openduck;

        public static MelonLogger.Instance log
        {
            get
            {
                return instance.LoggerInstance;
            }
        }
        public override void OnEarlyInitializeMelon()
        {
            instance = this;
            spawnduck = KeyCode.K;
            openduck = KeyCode.J;
        }
       public override void OnLateUpdate()
        {
            if (Input.GetKeyDown(spawnduck))
            {
                SpawnDuck();
            }

            if (Input.GetKeyDown(openduck))
            {
                OpenDuck();
            }
        }

        private static void SpawnDuck()
        {
            var GeneralManager = UnityEngine.Object.FindObjectOfType<GeneralManager>();
            instance.LoggerInstance.Msg("Duck Spawned");
            Traverse.Create(GeneralManager).Field("spawnCounter").SetValue(1000);
        }

        private static void OpenDuck()
        {
            GeneralManager generalManager = UnityEngine.Object.FindObjectOfType<GeneralManager>();
            //generalManager.ChangeDuck(-1);
            generalManager.ChangeDuck(0);
            instance.LoggerInstance.Msg(generalManager.Ducks.Count);
            for (int i=0; i<=generalManager.Ducks.Count; i++)
            {
                generalManager.ChangeDuck(i);
                if (i >= generalManager.Ducks.Count)
                {
                    generalManager.CurrentDuck = 0;
                    return;
                }
                else
                {
                    DuckManager currentduck = generalManager.Ducks[generalManager.CurrentDuck].GetComponent<DuckManager>();
                    if (currentduck.IsInSeasonContainer)
                    {
                        generalManager.AddDuck(currentduck, currentduck.duckID, true, false);
                        currentduck.OnSeasonClick();
                        instance.LoggerInstance.Msg("Open " + currentduck);
                    }
                }
            }
        }
    }
}
