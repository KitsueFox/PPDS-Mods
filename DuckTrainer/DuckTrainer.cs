using Duck_Trainer;
using HarmonyLib;
using MelonLoader;
using UnityEngine;
using UnityEngine.EventSystems;

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
        var centerstyle = GUI.skin.GetStyle("Label");
        centerstyle.alignment = TextAnchor.UpperCenter;
        string url = "https://github.com/KitsueFox/PPDS-Mods";
        var backgroundcolor = new Color(128f, 0f, 128f, 0.5f);
        
        GUI.contentColor = Color.white;
        GUI.backgroundColor = backgroundcolor;
        GUI.Box(new Rect((float)(Screen.width / 2 - 150), 1f, 350f,290f), "");
        GUI.Label(new Rect((float)(Screen.width / 2 - 50), 270f, 150f, 20f), "Made By BlackyFox", centerstyle);
        if (GUI.Button(new Rect((float)(Screen.width / 2 - 135), 40f, 150f, 50f), "Spawn Duck (K)")) SpawnDuck();
        if (GUI.Button(new Rect((float)(Screen.width / 2 - -35), 40f, 150f, 50f), "Open All Duck (J)")) OpenDuck();
    }

    private static void OpenMenu()
    {
        mod_menu = !mod_menu;
        var generalManager = Object.FindObjectOfType<GeneralManager>();
        
        if (mod_menu)
        {
            instance.LoggerInstance.Msg("Open Menu");

            MelonEvents.OnGUI.Subscribe(DrawMenu, 100);
            EventSystem.current.SetSelectedGameObject(null);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            instance.LoggerInstance.Msg("Close Menu");
            
            MelonEvents.OnGUI.Unsubscribe(DrawMenu);
            if (!generalManager.DusckSelectionMode) return;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
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