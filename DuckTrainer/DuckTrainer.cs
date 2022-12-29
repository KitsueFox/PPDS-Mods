using Duck_Trainer;
using HarmonyLib;
using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(DuckTrainer), "Duck Trainer", "0.0.1", "BlackyFox")]
[assembly: MelonGame("Turbolento Games", "Placid Plastic Duck Simulator")]

namespace Duck_Trainer;

public class DuckTrainer : MelonMod
{
    private static DuckTrainer instance;

    private static KeyCode spawnduck;
    private static KeyCode openduck;

    public override void OnEarlyInitializeMelon()
    {
        instance = this;
        spawnduck = KeyCode.K;
        openduck = KeyCode.J;
    }

    public override void OnLateUpdate() //TODO: Replace with GUI
    {
        if (Input.GetKeyDown(spawnduck)) SpawnDuck();

        if (Input.GetKeyDown(openduck)) OpenDuck();
    }

    private static void SpawnDuck()
    {
        var generalManager = Object.FindObjectOfType<GeneralManager>();
        instance.LoggerInstance.Msg("Duck Spawned");
        Traverse.Create(generalManager).Field("spawnCounter").SetValue(1000);
    }

    private static void OpenDuck()
    {
        var generalManager = Object.FindObjectOfType<GeneralManager>();
        //generalManager.ChangeDuck(-1);
        generalManager.ChangeDuck(0);
        instance.LoggerInstance.Msg(generalManager.Ducks.Count);
        for (var i = 0; i <= generalManager.Ducks.Count; i++)
        {
            generalManager.ChangeDuck(i);
            if (i >= generalManager.Ducks.Count)
            {
                generalManager.CurrentDuck = 0;
                return;
            }

            var currentduck =
                generalManager.Ducks[generalManager.CurrentDuck].GetComponent<DuckManager>();
            if (currentduck.IsInSeasonContainer)
            {
                generalManager.AddDuck(currentduck, currentduck.duckID, true, false);
                currentduck.OnSeasonClick();
                instance.LoggerInstance.Msg("Open " + currentduck);
            }
        }
    }
}