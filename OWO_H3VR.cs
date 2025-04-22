using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using FistVR;
using System;
using UnityEngine;

namespace OWO_H3VR
{
    [BepInPlugin("org.bepinex.plugins.OWO_H3VR", "OWO_H3VR", "1.0.0")]
    public class Plugin : BaseUnityPlugin //CAMBIAR LAS REFERENCIAS DE UNITY POR LAS DEL JUEGO
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
            public static void postfix(FVRFireArm __instance, bool twoHandStabilized)
            {
                owoSkin.LOG($"FVRFireArm - Recoil Name:{__instance.name} TwoHAnds:{twoHandStabilized}");
                
                if (!owoSkin.suitEnabled) return;

                string sensation = SensationsDictionary.SensationByWeaponType(__instance.RoundType);

                if (twoHandStabilized || __instance.Foregrip.activeSelf) //esto está mal
                {
                    owoSkin.Feel($"{sensation} LR");
                }
                else
                {
                    owoSkin.FeelWithHand($"{sensation}", 0, __instance.m_hand.IsThisTheRightHand);
                }

            }
        }

        #region Movement

        [HarmonyPatch(typeof(FVRMovementManager), "Jump")]
        public class OnJump
        {
            [HarmonyPrefix]
            public static void Prefix(FVRMovementManager __instance)
            {                
                if (!owoSkin.suitEnabled) return;

                if (Traverse.Create(__instance).Field("m_isGrounded").GetValue<bool>())
                {
                    owoSkin.Feel("Jump");
                    jumping = true;
                }
            }
        }

        [HarmonyPatch(typeof(FVRMovementManager), "RocketJump")]
        public class OnRocketJump
        {
            [HarmonyPrefix]
            public static void Prefix(FVRMovementManager __instance)
            {
                if (!owoSkin.suitEnabled) return;

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
                if (!owoSkin.suitEnabled || jumping == false) return;

                if (Traverse.Create(__instance).Field("m_isGrounded").GetValue<bool>())
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
                if (!owoSkin.suitEnabled) return;


                if (!__instance.IsHeld) { return; }
                if (!__instance.MP.IsMeleeWeapon) { return; }
                string collideWith = col.collider.name;
                // Collision with shells or mags shouldn't trigger feedback. Guns are "melee" as well.
                if (collideWith.Contains("Capsule") | collideWith.Contains("Mag")) { return; }
                bool twohanded = __instance.IsAltHeld;
                bool isRightHand = __instance.m_hand.IsThisTheRightHand;
                float speed = col.relativeVelocity.magnitude;
                // Also ignore very light bumps 
                if (speed <= 1.0f) { return; }
                // Scale feedback with the speed of the collision
                int intensity = (int)Mathf.Clamp((Math.Min(0.2f + speed / 5.0f, 1.0f)) * 100, 50, 100);



                owoSkin.FeelWithHand("Melee Attack", intensity, isRightHand);
            }
        }

        #endregion

        #region World interaction

        [HarmonyPatch(typeof(FistVR.FVRSceneSettings), "OnPowerupUse", new Type[] { typeof(FistVR.PowerupType) })]
        public class OnPowerupUse
        {
            [HarmonyPostfix]
            public static void Postfix(FistVR.PowerupType type)
            {
                // Powerup special effects in Take & Hold mode
                switch (type)
                {
                    case FistVR.PowerupType.Health:
                    case FistVR.PowerupType.Regen:
                        owoSkin.Feel("Heal");
                        break;
                    case FistVR.PowerupType.Explosive:
                        owoSkin.Feel("Explosion Face");
                        break;
                    case FistVR.PowerupType.InfiniteAmmo:
                        owoSkin.Feel("Infinite Ammo");
                        break;
                    case FistVR.PowerupType.Invincibility:
                        owoSkin.Feel("Invincibility");
                        break;
                    case FistVR.PowerupType.QuadDamage:
                        owoSkin.Feel("QuadDamage");
                        break;
                    case FistVR.PowerupType.SpeedUp:
                        owoSkin.Feel("Heart Beat Fast");
                        break;
                    case FistVR.PowerupType.MuscleMeat:
                        owoSkin.Feel("Muscle Meat");
                        break;
                    case FistVR.PowerupType.Ghosted:
                        owoSkin.Feel("Ghosted");
                        break;
                    case FistVR.PowerupType.Cyclops:
                        owoSkin.Feel("Cyclops");
                        break;
                    default:
                        owoSkin.LOG($"PowerupType - {type}");
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
                if (!owoSkin.suitEnabled) return;

                owoSkin.Feel("Vomit");
            }
        }

        [HarmonyPatch(typeof(FistVR.ZosigGameManager), "EatBangerJunk")]
        public class OnEatBangerJunk
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                if (!owoSkin.suitEnabled) return;

                owoSkin.Feel("Eating");
            }
        }

        [HarmonyPatch(typeof(FistVR.ZosigGameManager), "EatHerb")]
        public class OnEatHerb
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                if (!owoSkin.suitEnabled) return;

                owoSkin.Feel("Eating");
            }
        }

        [HarmonyPatch(typeof(FistVR.ZosigGameManager), "EatMeatCore")]
        public class OnEatMeatCore
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                if (!owoSkin.suitEnabled) return;

                owoSkin.Feel("Eating");
            }
        }

        #endregion

        #region Player damage

        [HarmonyPatch(typeof(FVRPlayerHitbox), "Damage", new Type[] { typeof(Damage)})]
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


                // Different hit patterns for different damage classes
                //string feedbackKey = "BulletHit";
                //switch (d.Class)
                //{
                //    case FistVR.Damage.DamageClass.Projectile:
                //        feedbackKey = "Bullet Hit";
                //        break;
                //    case FistVR.Damage.DamageClass.Melee:
                //        feedbackKey = "Blade Hit";
                //        break;
                //    case FistVR.Damage.DamageClass.Explosive:
                //        feedbackKey = "Impact";
                //        break;
                //    case FistVR.Damage.DamageClass.Environment:
                //        feedbackKey = "Impact";
                //        break;
                //    case FistVR.Damage.DamageClass.Abstract:
                //        feedbackKey = "Impact";
                //        break;
                //    default:
                //        break;
                //}

                //owoSkin.LOG($"Damage by: {feedbackKey} - life:{__instance.Body.GetPlayerHealth()}");
                
                //if (!owoSkin.suitEnabled) return;
                //owoSkin.Feel(feedbackKey);
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

        /*

        #region Weapon Recoil
        /// <summary>
        /// Commented because missing game libraries
        /// </summary>
       [harmonypatch(typeof(fistvr.fvrfirearm),"recoil")]
       public class onrecoilgun
       {
           [harmonypostfix]
           public static void postfix(fistvr.fvrfirearm __instance, bool twohandstabilized, bool foregripstabilized, bool shoulderstabilized)
           {
               string gunname = "";
               string recoilprefix;
               bool fatalerror = false;
               bool hasstock = false;
               bool twohanded = false;
               bool isrighthand = true;
               float intensity;
               fistvr.fvrfirearmrecoilprofile myrecoil;
               fistvr.firearmroundtype mybullettype;

               try { gunname = __instance.name; }
               catch { owoskin.log("gun name not found."); }
               try { hasstock = __instance.hasactiveshoulderstock; }
               catch { owoskin.log("gun stock info not found."); }
               try { twohanded = __instance.foregrip.activeself; }
               catch { owoskin.log("gun foregrip info not found."); }
               try { myrecoil = __instance.recoilprofile; }
               catch { owoskin.log("recoil profile not found."); fatalerror = true; myrecoil = new fistvr.fvrfirearmrecoilprofile(); }
               try { mybullettype = __instance.roundtype; }
               catch { owoskin.log("round type not found."); fatalerror = true; mybullettype = new fistvr.firearmroundtype(); }

               if (fatalerror)
               {
                   owoskin.gunrecoil(isrighthand, "pistol", 1.0f, (foregripstabilized | twohandstabilized), shoulderstabilized);
                   return;
               }

               recoilprefix = owoskin.configurerecoilbulletname(mybullettype);

               float scaledrecoil = (float)math.sqrt((double)myrecoil.xylinearpershot) + 0.55f;

               intensity = math.min(scaledrecoil, 1.0f);

               if (recoilprefix == "default")
               {
                   if ((hasstock) | (twohanded)) { recoilprefix = "rifle"; }
                   else { recoilprefix = "pistol"; }
               }

                special case for "the og" shotgun
               if (gunname.contains("breakactionshotguntheog")) { recoilprefix = "holymoly"; intensity = 1.0f; }

               owoskin.gunrecoil(isrighthand, recoilprefix, intensity, (foregripstabilized | twohandstabilized), shoulderstabilized);

               on both feels need to identify with hand, intensity and if the other hand is holding.

           }
       }

        #endregion

        // Tried to find the holster function. Did not yet succeed.
        [HarmonyPatch(typeof(FistVR.FVRFireArmBeltSegment), "EndInteraction", new Type[] { typeof(FistVR.FVRViveHand) })]
        public class OnHolsterEndInteraction
        {
            [HarmonyPostfix]
            public static void Postfix(FistVR.FVRViveHand hand)
            {
                owoSkin.FeelWithHand("Holster",isRightHand: hand.IsThisTheRightHand);
            }
        }

     

        [HarmonyPatch(typeof(FistVR.MainMenuScreen), "Start", new Type[] { })]
        public class OnLoadMenuScreen
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                //maxHealth = 0; //??
                
                owoSkin.StopAllHapticFeedback();
            }
        }

        /// <summary>
        /// CHECK IF HEALTH AND PLAYER POSITION CAN BE READ BETTER
        /// </summary>
        
        float maxHealth = 0;

        [HarmonyPatch(typeof(FistVR.FVRPlayerBody), "Update")]
        public class OnPlayerBodyUpdate
        {
            [HarmonyPostfix]
            public static void Postfix(FistVR.FVRPlayerBody __instance)
            {
                // I can't get to the world player position in the grenade explosion
                // function, so just store it globally on update
                playerPosition = __instance.transform.position;

                float health = __instance.Health;
                
                if (health > maxHealth) { maxHealth = health; }
                
                if (health < maxHealth / 3.0f) { owoSkin.StartHeartBeat(); }
                else { owoSkin.StopHeartBeat(); }
            }
        }

        //CHECK BETTER PLAYER POSITION
        [HarmonyPatch(typeof(FistVR.GrenadeExplosion), "Explode")]
        public class bhaptics_GrenadeExplosion
        {
            [HarmonyPostfix]
            public static void Postfix(FistVR.GrenadeExplosion __instance)
            {
                Vector3 grenadePosition = __instance.transform.position;
                
                //float distance = (grenadePosition - playerPosition).magnitude;
                
                // if grenade is more than 40 meters away, ignore explosion.
                // otherwise scale feedback. If close enough, this is in *addition*
                // to the explosion damage feedback
                //int intensity = (int) Math.Max(((40.0f - distance) / 40.0f), 0.0f);
                //owoSkin.Feel("ExplosionBelly", intensity);
            }
        }*/
    }
}
