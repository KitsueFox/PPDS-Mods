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
    private static KeyCode open_gui;

    private static bool mod_menu;

    public override void OnEarlyInitializeMelon()
    {
        instance = this;
        spawnduck = KeyCode.K;
        openduck = KeyCode.J;
        open_gui = KeyCode.F9;
    }

    public override void OnLateUpdate() //TODO: Replace with GUI
    {
        if (Input.GetKeyDown(spawnduck)) SpawnDuck();

        if (Input.GetKeyDown(openduck)) OpenDuck();
        
        if (Input.GetKeyDown(open_gui)) OpenMenu();
    }

    public static void DrawMenu()
    {
        var backgroundcolor = new Color(128f, 0f, 128f, 0.5f);
        GUI.contentColor = Color.white;
        GUI.backgroundColor = backgroundcolor;
        GUI.Box(new Rect((float)(Screen.width / 2 - 150), 1f, 355f,290f), "");
    }

    private static void OpenMenu()
    {
        mod_menu = !mod_menu;

        if (mod_menu)
        {
            instance.LoggerInstance.Msg("Open Menu");

            MelonEvents.OnGUI.Subscribe(DrawMenu, 100);
        }
        else
        {
            instance.LoggerInstance.Msg("Close Menu");
            
            MelonEvents.OnGUI.Unsubscribe(DrawMenu);
        }
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
        generalManager.ChangeDuck(0);
        //instance.LoggerInstance.Msg(generalManager.Ducks.Count); //DEBUG ONLY
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
            if (!currentduck.IsInSeasonContainer) continue;
            generalManager.AddDuck(currentduck, currentduck.duckID, true, false);
            currentduck.OnSeasonClick();
            instance.LoggerInstance.Msg("Opened " + currentduck);
        }
    }
}