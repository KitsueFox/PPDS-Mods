using System;
using Duck_Trainer;
using HarmonyLib;
using MelonLoader;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

[assembly: MelonInfo(typeof(DuckTrainer), "Duck Trainer", "0.0.1", "BlackyFox")]
[assembly: MelonGame("Turbolento Games", "Placid Plastic Duck Simulator")]

namespace Duck_Trainer
{
    public class DuckTrainer : MelonMod
    {
        private static DuckTrainer _instance;

        private static readonly KeyCode _spawnduck = KeyCode.K;
        private static readonly KeyCode _openduck = KeyCode.J;
        private static readonly KeyCode _respawnDucks = KeyCode.H;
        private static readonly KeyCode _openGUI = KeyCode.F9;
        private static KeyCode _flyDuck = KeyCode.Space;

        private static bool _modMenu;
        private static bool _duckMove;

        private static bool _weatherChange;

        //public static bool RespawnDuck = true;
        private static Vector3 _duckMovementInput;
        private static string _duckMoveGUI = "Duck Move (Disable)";

        private static GeneralManager _generalManager;
        private static EnviroZone _enviroZone;
        private static EnviroSky _enviroSky;

        //Melonloader Area
        public override void OnEarlyInitializeMelon()
        {
            _instance = this;
        }

        /*public override void OnInitializeMelon() //TODO:Bring back when Patching is Needed
        {
            var harmony = new HarmonyLib.Harmony("Duck_Trainer");
            FileLog.Log("Before Patch");
            try
            {
                harmony.PatchAll(typeof(GmPatch));
                _instance.LoggerInstance.Msg("Patched!");
            }
            catch (Exception e)
            {
                _instance.LoggerInstance.Msg(e);
            }
        }*/

        public override void OnLateUpdate()
        {
            var intro = SceneManager.GetActiveScene().name == "Intro";

            if (intro) return; // Check if Intro Scene is not loaded
            _generalManager = Object.FindObjectOfType<GeneralManager>();
            _enviroZone = Object.FindObjectOfType<EnviroZone>();
            _enviroSky = Object.FindObjectOfType<EnviroSky>();
            if (Input.GetKeyDown(_spawnduck)) {SpawnDuck();}
            if (Input.GetKeyDown(_openduck)) {OpenDuck();}
            if (Input.GetKeyDown(_openGUI)) {OpenMenu();}
            if (Input.GetKeyDown(_respawnDucks)) {AllRespawn();}
        }

        public override void OnUpdate()
        {
            _duckMovementInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

            DuckMovement();
        }

        //Internal Code
        private static void DrawMenu() //Trainer Menu
        {
            var centerstyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.UpperCenter
            };
            var url = "https://github.com/KitsueFox/PPDS-Mods";
            var backgroundcolor = new Color(128f, 0f, 0f, 0.5f);

            GUI.contentColor = Color.white;
            GUI.backgroundColor = backgroundcolor;
            GUI.Box(new Rect((float)(Screen.width / 2 - 150), 1f, 350f, 290f), "");
            GUI.Label(new Rect((float)(Screen.width / 2 - 50), 270f, 150f, 20f), "Made By BlackyFox", centerstyle);
            if (GUI.Button(new Rect((float)(Screen.width / 2 - 135), 40f, 150f, 50f), "Spawn Duck (K)")) {SpawnDuck();}
            if (GUI.Button(new Rect((float)(Screen.width / 2 - -35), 40f, 150f, 50f), "Open All Duck (J)")) {OpenDuck();}
            if (GUI.Button(new Rect((float)(Screen.width / 2 - 135), 100f, 150f, 50f), "All Duck Quack")) {AllSpeak();}
            if (GUI.Button(new Rect((float)(Screen.width / 2 - -35), 100f, 150f, 50f), _duckMoveGUI))
            {DuckMovement_Check();}
            if (GUI.Button(new Rect((float)(Screen.width / 2 - 135), 160f, 150f, 50f), "Clear Weather")) {WeatherChange();}
        }

        private static void OpenMenu() //Self explanatory
        {
            _modMenu = !_modMenu;

            if (_modMenu)
            {
                _instance.LoggerInstance.Msg("Open Menu");

                MelonEvents.OnGUI.Subscribe(DrawMenu, 100);
                EventSystem.current.SetSelectedGameObject(null);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                _instance.LoggerInstance.Msg("Close Menu");

                MelonEvents.OnGUI.Unsubscribe(DrawMenu);
                if (!_generalManager.DusckSelectionMode) return;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        private static void SpawnDuck() //Spawn A new duck by invoking the spawnCounter
        {
            _instance.LoggerInstance.Msg("Duck Spawned");
            Traverse.Create(_generalManager).Field("spawnCounter").SetValue(1000);
        }

        private static void OpenDuck() //Opening Ducks out of the Presents, Might have to edit
        {
            _generalManager.ChangeDuck(0);
            var gmsAudio =
                Traverse.Create(_generalManager).Field("seasonOpenContainerAudioSource").GetValue() as AudioSource;
            var gmsFX =
                Traverse.Create(_generalManager).Field("seasonOpenContainerFX").GetValue() as ParticleSystem;
            var gmsFloat = new float[] { 0.5f, 0.75f, 1.25f, 1.5f };
            //instance.LoggerInstance.Msg(generalManager.Ducks.Count); //DEBUG ONLY
            for (var i = 0; i <= _generalManager.Ducks.Count; i++)
            {
                _generalManager.ChangeDuck(i);
                if (i >= _generalManager.Ducks.Count)
                {
                    _generalManager.CurrentDuck = 0;
                    return;
                }

                var currentduck =
                    _generalManager.Ducks[_generalManager.CurrentDuck].GetComponent<DuckManager>();
                if (!currentduck.IsInSeasonContainer) continue;
                _generalManager.AddDuck(currentduck, currentduck.duckID, true, false);
                currentduck.OnSeasonClick();
                gmsAudio.pitch = gmsFloat[Random.Range(0, gmsFloat.Length)];
                gmsFX.transform.position = currentduck.transform.position;
                gmsFX.Play();
                gmsAudio.Play();
                _instance.LoggerInstance.Msg("Opened " + currentduck);
            }
        }

        private static void AllSpeak() //Make all Ducks Quack
        {
            for (var i = 0; i <= _generalManager.Ducks.Count; i++)
            {
                _generalManager.ChangeDuck(i);
                if (i >= _generalManager.Ducks.Count)
                {
                    _generalManager.CurrentDuck = 0;
                    return;
                }

                _generalManager.Ducks[_generalManager.CurrentDuck].GetComponent<DuckManager>().PlaySound();
            }
        }

        private static void AllRespawn() //Make all Ducks Respawn
        {
            for (var i = 0; i <= _generalManager.Ducks.Count; i++)
            {
                _generalManager.ChangeDuck(i);
                if (i >= _generalManager.Ducks.Count)
                {
                    _generalManager.CurrentDuck = 0;
                    return;
                }

                _generalManager.Ducks[_generalManager.CurrentDuck].GetComponent<DuckManager>().transform.position =
                    _generalManager.SpawnPoint.position;
            }
        }

        private static void WeatherChange()
        {
            throw new NotImplementedException();
        }

        private static void DuckMovement_Check() //Check if Movement Script is enable may change in the Future
        {
            _duckMove = !_duckMove;
            _duckMoveGUI = _duckMove ? "Duck Move (Enabled)" : "Duck Move (Disable)";
        }

        private static void DuckMovement() //Move Ducks by using WSAD, TODO: Add Direction indicator
        {
            if (!_duckMove) return;
            if (_generalManager.DusckSelectionMode) return;
            var currentduck = _generalManager.Ducks[_generalManager.CurrentDuck].GetComponent<DuckManager>();
            if (currentduck == null) return;
            var cforce = currentduck.GetComponent<ConstantForce>();
            cforce.relativeForce = _duckMovementInput * 20;
            if (Input.GetKey(_flyDuck))
            {
                cforce.relativeForce = new Vector3(0f, 60f, 0f);
            }
        }
    }
}
//Harmony Patches TODO: Make Duck Respawner with DuckManager