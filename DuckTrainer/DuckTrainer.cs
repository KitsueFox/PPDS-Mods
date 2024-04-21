using Duck_Trainer;
using Enviro;
using HarmonyLib;
using MelonLoader;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
// ReSharper disable PossibleLossOfFraction

[assembly: MelonInfo(typeof(DuckTrainer), "Duck Trainer", "0.1.7", "BlackyFox", "https://github.com/KitsueFox/PPDS-Mods")]
[assembly: MelonGame("Turbolento Games", "Placid Plastic Duck Simulator")]

namespace Duck_Trainer
{
    public class DuckTrainer : MelonMod
    {
        internal static DuckTrainer Instance { get; set; }

        private static readonly KeyCode Spawnduck = KeyCode.K;
        private static readonly KeyCode Openduck = KeyCode.J;
        private static readonly KeyCode RespawnDucks = KeyCode.H;
        private static readonly KeyCode OpenGUI = KeyCode.F9;
        private static readonly KeyCode FlyDuck = KeyCode.Space;

        private static bool _modMenu;
        public static bool Achievements { get; set; }
        public static bool DuckMove { get; set; }
        public static bool DuckRespawn { get; set; }
        public static bool CtrlSnowPlow { get; set; }

        private static Vector3 _movementInput;
        public static readonly float DistancetoResapwn = 500000f;
        private static string _duckMoveGUI = "Duck Move (Disable)";
        private static string _duckResapwnGUI = "Auto Respawn (Disable)";
        private static string _snowplowGUI = "Snowplow (Disable)";

        private static GeneralManager _generalManager;
        private static Snowplow _snowplow;

        //Melonloader Area
        public override void OnEarlyInitializeMelon()
        {
            Instance = this;
        }

        public override void OnInitializeMelon()
        {
            DuckTrainerSettings.RegisterSettings();
            Achievements = DuckTrainerSettings.Achievements.Value;
            var harmony = new HarmonyLib.Harmony("Duck_Trainer");
            try
            {
                harmony.PatchAll(typeof(DuckTrainerPatch.DuckManagerPatchUpdate));
                Instance.LoggerInstance.Msg("DuckManager Patched!");

                harmony.PatchAll(typeof(DuckTrainerPatch.SnowPlowCameraPatch));
                Instance.LoggerInstance.Msg("SnowPlow Camera Patch");

                harmony.PatchAll(typeof(DuckTrainerPatch.SnowPlowMovement));
                Instance.LoggerInstance.Msg("SnowPlow Movement Patch");
                
                harmony.PatchAll(typeof(DuckTrainerPatch.AchievementsDisabler));
                if (!Achievements) {
                    Instance.LoggerInstance.Msg("Achievements Disabled!");
                }
            }
            catch (Exception e)
            {
                Instance.LoggerInstance.Msg(e);
            }

            DuckRespawn = DuckTrainerSettings.AutoRespawn.Value;
            _duckResapwnGUI = DuckRespawn ? "Auto Respawn (Enable)" : "Auto Respawn (Disable)";
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName) //Check if DLC Scene is Active
        {
            GameObject console;
            if ("Intro" == sceneName && !Achievements)
            {
                MelonEvents.OnGUI.Subscribe(DrawWarning, 100);
            }
            else
            {
                MelonEvents.OnGUI.Unsubscribe(DrawWarning);
            }

            if ("dlc2Env" == sceneName)
            {
                _snowplow = Object.FindObjectOfType<Snowplow>();
            }

            if ("Intro" != sceneName)
            {
                _generalManager = Object.FindObjectOfType<GeneralManager>();
                if (_generalManager == null)
                {
                    Instance.LoggerInstance.Error("General Manager Didn't Hook!!");
                }
                else
                {
                    try
                    {
                        console = GameObject.Find("BaseEnvironment/CommandConsole/Canvas");
                        Instance.LoggerInstance.Msg(console.ToString());
                        console.SetActive(true);
                    }
                    catch (Exception e)
                    {
                        Instance.LoggerInstance.Msg(e);
                    }

                }
            }
        }

        public override void OnLateUpdate()
        {
            var intro = SceneManager.GetActiveScene().name == "Intro";

            if (intro)
            {
                return;
            } // Check if Intro Scene is not loaded 
            
            if (Input.GetKeyDown(Spawnduck)) { SpawnDuck(); }
            if (Input.GetKeyDown(Openduck)) { OpenDuck(); } // <-- For Christmas Event Only
            if (Input.GetKeyDown(OpenGUI)) { OpenMenu(); }
            if (Input.GetKeyDown(RespawnDucks)) { AllRespawn(); }
        }

        public override void OnUpdate()
        {
            _movementInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

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
            GUI.Box(new Rect(Screen.width / 2 - 150, 1f, 350f, 290f), "");
            if (!Achievements)
            {
                GUI.Label(new Rect(Screen.width / 2 - 50, 250f, 150f, 20f), "Achievements Disable!", centerstyle);
            }
            GUI.Label(new Rect(Screen.width / 2 - 50, 270f, 150f, 20f), "Made By BlackyFox", centerstyle);
            if (GUI.Button(new Rect(Screen.width / 2 - 135, 20f, 150f, 50f), "Spawn Duck (K)")) { SpawnDuck(); }
            if (GUI.Button(new Rect(Screen.width / 2 - -35, 20f, 150f, 50f), "All Ducks Respawn (H)")) { AllRespawn(); }
            if (GUI.Button(new Rect(Screen.width / 2 - 135, 80f, 150f, 50f), "All Duck Quack")) { AllSpeak(); }
            if (GUI.Button(new Rect(Screen.width / 2 - -35, 80f, 150f, 50f), _duckMoveGUI))
            { DuckMovement_Check(); }
            if (GUI.Button(new Rect(Screen.width / 2 - 135, 140f, 150f, 50f), "Clear Weather")) { WeatherChange(); }
            if (GUI.Button(new Rect(Screen.width / 2 - -35, 140f, 150f, 50f), _duckResapwnGUI))
            { DuckRespawn_Check(); }
            if (GUI.Button(new Rect(Screen.width / 2 - 135, 200f, 150f, 50f), _snowplowGUI)) { SnowPlow(); }
            if (GUI.Button(new Rect(Screen.width / 2 - -35, 200f, 150f, 50f), "Mod Page")) { Application.OpenURL(url); }
        }

        private static void DrawWarning()
        {
            var style = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleLeft,
                fontSize = 20,
            };

            GUI.contentColor = Color.red;
            GUI.Label(new Rect(10, 10, 400, 20), "Achievements Disable!", style);
        }

        private static void OpenMenu() //Self explanatory
        {
            _modMenu = !_modMenu;

            if (_modMenu)
            {
                Instance.LoggerInstance.Msg("Open Menu");

                MelonEvents.OnGUI.Subscribe(DrawMenu, 100);
                EventSystem.current.SetSelectedGameObject(null);
                if (Cursor.visible) {return;}
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Instance.LoggerInstance.Msg("Close Menu");

                MelonEvents.OnGUI.Unsubscribe(DrawMenu);
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        private static void SpawnDuck() //Spawn A new duck by invoking the spawnCounter
        {
            Instance.LoggerInstance.Msg("Duck Spawned");
            Traverse.Create(_generalManager).Field("spawnCounter").SetValue(1000);
        }

        public static void SnowPlow() //Snow Plow Control
        {
            var snowdlc = SceneManager.GetActiveScene().name == "dlc2Env";
            var plowon = Traverse.Create(_snowplow).Field("isOn").GetValue<bool>();
            if (snowdlc && !DuckMove && plowon)
            {
                CtrlSnowPlow = !CtrlSnowPlow;
                _snowplowGUI = CtrlSnowPlow ? "Snowplow (Enable)" : "Snowplow (Disable)";
            }
            else if (snowdlc && !DuckMove && !plowon)
            {
                CtrlSnowPlow = false;
                _snowplowGUI = "Snowplow (Disable)";
            }
            else
            {
                CtrlSnowPlow = false;
                _snowplowGUI = "Snowplow (N/A)";
            }
        }

        private static void OpenDuck() //Opening Ducks out of the Presents. Christmas Event may remove
        {
            var gmsAudio =
                Traverse.Create(_generalManager).Field("seasonOpenContainerAudioSource").GetValue() as AudioSource;
            var gmsFX =
                Traverse.Create(_generalManager).Field("seasonOpenContainerFX").GetValue() as ParticleSystem;
            var gmsFloat = new[] { 0.5f, 0.75f, 1.25f, 1.5f };
            var lastduck = _generalManager.CurrentDuck;
            _generalManager.ChangeDuck(0);
            for (var i = 0; i <= _generalManager.Ducks.Count; i++)
            {
                _generalManager.ChangeDuck(i);
                if (i >= _generalManager.Ducks.Count)
                {
                    _generalManager.CurrentDuck = lastduck;
                    return;
                }

                var currentduck =
                    _generalManager.Ducks[_generalManager.CurrentDuck].GetComponent<DuckManager>();
                if (!currentduck.IsInSeasonContainer) {continue;}
                _generalManager.AddDuck(currentduck, currentduck.duckID, true, false);
                currentduck.OnSeasonClick();
                if (gmsAudio != null && gmsFX != null)
                {
                    gmsAudio.pitch = gmsFloat[Random.Range(0, gmsFloat.Length)];
                    gmsFX.transform.position = currentduck.transform.position;
                    gmsFX.Play();
                    gmsAudio.Play();
                }

                Instance.LoggerInstance.Msg("Opened " + currentduck);
            }
        }


        private static void AllSpeak() //Make all Ducks Quack
        {
            var lastduck = _generalManager.CurrentDuck;
            _generalManager.ChangeDuck(0);
            for (var i = 0; i <= _generalManager.Ducks.Count-1; i++)
            {
                _generalManager.ChangeDuck(i);
                if (i >= _generalManager.Ducks.Count)
                {
                    _generalManager.CurrentDuck = lastduck;
                    return;
                }

                _generalManager.Ducks[_generalManager.CurrentDuck]?.GetComponent<DuckManager>().PlaySound();
            }
        }

        private static void AllRespawn() //Make all Ducks Respawn
        {
            for (var i = 0; i <= _generalManager.Ducks.Count-1; i++)
            {
                _generalManager.ChangeDuck(i);
                if (i >= _generalManager.Ducks.Count)
                {
                    _generalManager.CurrentDuck = 0;
                    return;
                }

                if (_generalManager.Ducks[_generalManager.CurrentDuck] != null)
                {
                    _generalManager.Ducks[_generalManager.CurrentDuck].GetComponent<DuckManager>().transform.position =
                        _generalManager.SpawnPoint.position;
                }
            }
        }

        private static void WeatherChange() //Forces Weather to Clear
        {
            var _enviroweathermodule = Object.FindObjectOfType<EnviroWeatherModule>();
            var basegm = SceneManager.GetActiveScene().name == "MainScene";
            var snowdlc = SceneManager.GetActiveScene().name == "dlc2Env";
            if (basegm)
            {
                _enviroweathermodule.ChangeWeather("Clear Sky");
            }
            else if (snowdlc)
            {
                _enviroweathermodule.ChangeWeather("Clear SkyWinter");
            }
            else
            {
                Instance.LoggerInstance.Error("Changing Weather is not support in Current Scene yet");
            }

        }

        private static void DuckMovement_Check() //Check if Movement Script is enable may change in the Future
        {
            if (!CtrlSnowPlow)
            {
                DuckMove = !DuckMove;
                _duckMoveGUI = DuckMove ? "Duck Move (Enabled)" : "Duck Move (Disable)";
            }
            else
            {
                DuckMove = false;
                _duckMoveGUI = "Duck Move (N/A)";
            }
        }

        private static void DuckMovement() //Move Ducks by using WSAD and Space to Fly, TODO: Add Direction indicator
        {
            if (!DuckMove) {return;}
            if (_generalManager.DusckSelectionMode) {return;}
            var currentduck = _generalManager.Ducks[_generalManager.CurrentDuck].GetComponent<DuckManager>();
            if (currentduck == null) {return;}
            var cforce = currentduck.GetComponent<ConstantForce>();
            cforce.relativeForce = _movementInput * 20;
            if (Input.GetKey(FlyDuck))
            {
                cforce.relativeForce = new Vector3(0f, 60f, 0f);
                currentduck.SetBuoyanciesStatus(false);
            }
            else
            {
                currentduck.SetBuoyanciesStatus(true);
            }
        }

        private static void DuckRespawn_Check() //Check for Duck Respawn is Enabled
        {
            DuckRespawn = !DuckRespawn;
            DuckTrainerSettings.AutoRespawn.Value = DuckRespawn;
            _duckResapwnGUI = DuckRespawn ? "Auto Respawn (Enable)" : "Auto Respawn (Disable)";
            MelonPreferences.Save();
        }
    }
}