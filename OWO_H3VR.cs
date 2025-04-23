using BepInEx;
using BepInEx.Logging;
using FistVR;
using HarmonyLib;
using System;
using System.Collections;
using UnityEngine;

namespace OWO_H3VR
{
    [BepInPlugin("org.bepinex.plugins.OWO_H3VR", "OWO_H3VR", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {

#pragma warning disable CS0109
        internal static new ManualLogSource Log;
#pragma warning restore CS0109

        public static OWOSkin owoSkin;
        public static OWOSDK owoSDK;
        public static float lastHealth = 1f;
        static bool jumping = false;

        private void Awake()
        {
            Log = Logger;
            Logger.LogMessage("OWO_H3VR plugin is loaded!");

            owoSDK = new OWOSDK();
            owoSkin = new OWOSkin(owoSDK);

            StartCoroutine(owoSDK.StartConnection());
            StartCoroutine(owoSkin.FinalizeOWOConnection());

            var harmony = new Harmony("owo.patch.h3vr");
            harmony.PatchAll();
        }

        [HarmonyPatch(typeof(FVRFireArm), "Recoil")]
        public class OnRecoil
        {
            [HarmonyPostfix]
            public static void postfix(FVRFireArm __instance, bool twoHandStabilized, bool foregripStabilized)
            {
                if (!owoSkin.CanFeel()) return;

                string sensation = SensationsDictionary.SensationByWeaponType(__instance.RoundType);
                bool isDualHand = (twoHandStabilized || foregripStabilized);

                owoSkin.FeelWithHand($"{sensation}", 2, __instance.m_hand.IsThisTheRightHand, isDualHand);
            }
        }

        #region Movement

        [HarmonyPatch(typeof(FVRMovementManager), "Jump")]
        public class OnJump
        {
            [HarmonyPrefix]
            public static void Prefix(FVRMovementManager __instance)
            {
                if (!owoSkin.CanFeel()) return;

                if (Traverse.Create(__instance).Field("m_isGrounded").GetValue<bool>())
                {
                    owoSkin.Feel("Jump");
                    jumping = true;
                }
            }
        }


        [HarmonyPatch(typeof(FVRMovementManager), "UpdateSmoothLocomotion")]
        public class OnUpdateSmoothLocomotion
        {
            [HarmonyPostfix]
            public static void Postfix(FVRMovementManager __instance)
            {
                if (!owoSkin.CanFeel()) return;

                bool isGrounded = Traverse.Create(__instance).Field("m_isGrounded").GetValue<bool>();

                if (!jumping && !isGrounded)
                {
                    jumping = true;
                }
                else if (jumping && isGrounded)
                {
                    owoSkin.Feel("Landing");
                    jumping = false;
                }

            }
        }

        [HarmonyPatch(typeof(FistVR.FVRPhysicalObject), "OnCollisionEnter", new Type[] { typeof(Collision) })]
        public class OnMeleeCollider
        {
            [HarmonyPostfix]
            public static void Postfix(FistVR.FVRPhysicalObject __instance, Collision col)
            {
                if (!owoSkin.CanFeel()) return;
                if (isCoroutineRunning) return;

                if (!__instance.IsHeld) { return; }
                if (!__instance.MP.IsMeleeWeapon) { return; }

                string collideWith = col.collider.name;
                // Collision with shells or mags shouldn't trigger feedback. Guns are "melee" as well.
                if (collideWith.Contains("Capsule") | collideWith.Contains("Mag")) { return; }
                float speed = col.relativeVelocity.magnitude;
                // Also ignore very light bumps 
                owoSkin.LOG($"##Collision Speed: {speed}");
                if (speed <= 2f) { return; }

                __instance.StartCoroutine(SendMeleeCollision(__instance, col, speed));

            }
        }

        public static bool isCoroutineRunning = false;

        public static IEnumerator SendMeleeCollision(FistVR.FVRPhysicalObject __instance, Collision col, float speed)
        {
            isCoroutineRunning = true;

            // Scale feedback with the speed of the collision
            int intensity = (int)Mathf.Clamp((Math.Min(0.2f + speed / 5.0f, 1.0f)) * 100, 50, 100);
            bool isRightHand = __instance.m_hand.IsThisTheRightHand;
            bool twohanded = __instance.IsAltHeld;

            owoSkin.FeelWithHand("Melee", 0, isRightHand, twohanded, intensity);
            yield return new WaitForSeconds(0.1f);
            isCoroutineRunning = false;
        }

        #endregion

        #region World interaction

        [HarmonyPatch(typeof(FistVR.FVRSceneSettings), "OnPowerupUse", new Type[] { typeof(FistVR.PowerupType) })]
        public class OnPowerupUse
        {
            [HarmonyPostfix]
            public static void Postfix(FistVR.PowerupType type)
            {
                if (!owoSkin.CanFeel()) return;

                // Powerup special effects in Take & Hold mode
                switch (type)
                {
                    case FistVR.PowerupType.Health:
                    case FistVR.PowerupType.Regen:
                        owoSkin.Feel("Heal");
                        break;
                    default:
                        owoSkin.Feel("Power Up");
                        break;
                }
            }
        }

        [HarmonyPatch(typeof(FistVR.ZosigGameManager), "VomitObject", new Type[] { typeof(FistVR.FVRObject) })]
        public class OnVomitObject
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                if (!owoSkin.CanFeel()) return;

                owoSkin.Feel("Vomit");
            }
        }

        [HarmonyPatch(typeof(FistVR.ZosigGameManager), "EatBangerJunk")]
        public class OnEatBangerJunk
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                if (!owoSkin.CanFeel()) return;

                owoSkin.Feel("Eating");
            }
        }

        [HarmonyPatch(typeof(FistVR.ZosigGameManager), "EatHerb")]
        public class OnEatHerb
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                if (!owoSkin.CanFeel()) return;

                owoSkin.Feel("Eating");
            }
        }

        [HarmonyPatch(typeof(FistVR.ZosigGameManager), "EatMeatCore")]
        public class OnEatMeatCore
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                if (!owoSkin.CanFeel()) return;

                owoSkin.Feel("Eating");
            }
        }

        #endregion

        #region Player damage

        [HarmonyPatch(typeof(FVRPlayerHitbox), "Damage", new Type[] { typeof(Damage) })]
        public class OnDamageDealtHitbox
        {
            [HarmonyPostfix]
            public static void Postfix(FVRPlayerHitbox __instance, Damage d)
            {
                if (!owoSkin.CanFeel()) return;

                float actualHealth = __instance.Body.GetPlayerHealth();

                if (actualHealth == lastHealth) return;
                lastHealth = actualHealth;

                if (actualHealth <= 0)
                {
                    owoSkin.StopAllHapticFeedback();
                    owoSkin.Feel("Death", 4);
                    owoSkin.playerIsAlive = false;
                    return;
                }


                FVRPlayerBody myBody = __instance.Body;
                owoSkin.FeelDynamicDamage(myBody, d);

            }
        }

        [HarmonyPatch(typeof(FVRPlayerBody), "ResetHealth")]
        public class OnPlayerKilled
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                owoSkin.LOG("Reset health");
                owoSkin.playerIsAlive = true;
            }
        }

        #endregion


        [HarmonyPatch(typeof(GrappleGun), "AttemptRetract")]
        public class OnAttemptRetract
        {
            [HarmonyPostfix]
            public static void Postfix(GrappleGun __instance, bool Pull)
            {
                if (!owoSkin.CanFeel()) return;
                if (Pull && __instance.IsHeld)
                {
                    owoSkin.LOG($"Grapple me lleva");
                }
            }
        }
    }
}
