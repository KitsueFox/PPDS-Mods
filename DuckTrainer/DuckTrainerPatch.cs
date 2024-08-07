using HarmonyLib;
using System.Reflection.Emit;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Duck_Trainer
{
    public abstract class DuckTrainerPatch //Duck Respawn Patch for DuckManager
    {
        //Harmony Patches
        [HarmonyPatch(typeof(DuckManager), "Update")]
        public class DuckManagerPatchUpdate
        {
            static bool Prefix(ref DuckManager __instance, Rigidbody ___rb)
            {
                if (DuckTrainer.DuckRespawn == true)
                {
                    if (__instance.gameObject.transform.position.sqrMagnitude > DuckTrainer.DistancetoResapwn)
                    {
                        ___rb.velocity = Vector3.zero;
                        __instance.gameObject.transform.position = __instance.GeneralManager.SpawnPoint.position;
                        __instance.PlaySound();
                    }

                    return true;
                }
                else
                {
                    return true;
                }
            }
        }

        [HarmonyPatch(typeof(GeneralManager), "CameraUpdate")] //Override Camera to Snowplow in GeneralManager
        public class SnowPlowCameraPatch
        {
            static bool Prefix(ref GeneralManager __instance)
            {
                if (DuckTrainer.CtrlSnowPlow == true)
                {
                    var snowplow = GameObject.Find("Snowplow");
                    __instance.CameraRig.transform.position = Vector3.Lerp(__instance.CameraRig.transform.position,
                        snowplow.transform.position, Time.deltaTime * __instance.CameraSpeed);
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        [HarmonyPatch(typeof(Snowplow), "Update")] //Override Snowplow Controls
        public class SnowPlowMovement
        {
            static bool Prefix(ref Snowplow __instance, bool ___isOn)
            {
                if (DuckTrainer.CtrlSnowPlow && !DuckTrainer.DuckMove && ___isOn)
                {
                    var o = __instance.gameObject;
                    o.transform.Translate(0, 0, Input.GetAxis("Vertical") / 4);
                    o.transform.rotation =
                        o.transform.rotation * Quaternion.Euler(new Vector3(0, Input.GetAxis("Horizontal") / 2, 0));
                    return false;
                }
                else if (DuckTrainer.CtrlSnowPlow && !DuckTrainer.DuckMove && !___isOn)
                {
                    DuckTrainer.SnowPlow();
                    return true;
                }
                else
                {
                    return true;
                }
            }
        }

        [HarmonyPatch(typeof(AchievementsManager), "UnlockAchievement")]
        public class AchievementsDisabler
        {
            static bool Prefix(ref AchievementType achievement)
            {
                if (DuckTrainer.Achievements == false) {
                    DuckTrainer.Instance.LoggerInstance.Msg(
                        "Achievements Disable! You missed " + achievement.ToString());
                    return false;
                }
                else
                {
                    DuckTrainer.Instance.LoggerInstance.Msg(
                        "Achievement Collected " + achievement.ToString());
                    return true;
                }
            }
        }

        /*[HarmonyPatch(typeof(StageSelector), "ChooseStage")]
        public class StageSelectorPatch
        {
            static bool Prefix(ref StageSelector __instance, GameObject ___nextStageArrow, GameObject ___prevStageArrow, Text ___stageNameText)
            {
                bool flag = true;
                ___nextStageArrow.SetActive(flag);
                ___prevStageArrow.SetActive(flag);
                
            }
        }*/
    }
}