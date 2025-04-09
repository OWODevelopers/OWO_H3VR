using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
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

        private void Awake()
        {
            Log = Logger;
            Logger.LogMessage("OWO_H3VR plugin is loaded!");
            owoSkin = new OWOSkin();

            var harmony = new Harmony("owo.patch.h3vr");
            harmony.PatchAll();
        }
        /*

        #region Weapon Recoil
        /// <summary>
        /// Commented because missing game libraries
        /// </summary>
       [HarmonyPatch(typeof(FistVR.FVRFireArm),"Recoil")]
       public class OnRecoilGun
       {
           [HarmonyPostfix]
           public static void Postfix(FistVR.FVRFireArm __instance, bool twoHandStabilized, bool foregripStabilized, bool shoulderStabilized)
           {
               string gunName = "";
               string recoilPrefix;
               bool fatalError = false;
               bool hasStock = false;
               bool twoHanded = false;
               bool isRightHand = true;
               float intensity;
               FistVR.FVRFireArmRecoilProfile myRecoil;
               FistVR.FireArmRoundType myBulletType;

               try { gunName = __instance.name; }
               catch { owoSkin.LOG("Gun name not found."); }
               try { hasStock = __instance.HasActiveShoulderStock; }
               catch { owoSkin.LOG("Gun stock info not found."); }
               try { twoHanded = __instance.Foregrip.activeSelf; }
               catch { owoSkin.LOG("Gun foregrip info not found."); }
               try { myRecoil = __instance.RecoilProfile; }
               catch { owoSkin.LOG("Recoil profile not found."); fatalError = true; myRecoil = new FistVR.FVRFireArmRecoilProfile(); }
               try { myBulletType = __instance.RoundType; }
               catch { owoSkin.LOG("Round type not found."); fatalError = true; myBulletType = new FistVR.FireArmRoundType(); }

               if (fatalError)
               {
                   //owoSkin.GunRecoil(isRightHand, "Pistol", 1.0f, (foregripStabilized | twoHandStabilized), shoulderStabilized);
                   return;
               }

               recoilPrefix = owoSkin.ConfigureRecoilBulletName(myBulletType);

               float scaledRecoil = (float)Math.Sqrt((double)myRecoil.XYLinearPerShot) + 0.55f;

               intensity = Math.Min(scaledRecoil, 1.0f);

               if (recoilPrefix == "Default")
               {
                   if ((hasStock) | (twoHanded)) { recoilPrefix = "Rifle"; }
                   else { recoilPrefix = "Pistol"; }
               }

               // Special case for "The OG" shotgun
               if (gunName.Contains("BreakActionShotgunTheOG")) { recoilPrefix = "HolyMoly"; intensity = 1.0f; }

               //owoSkin.GunRecoil(isRightHand, recoilPrefix, intensity, (foregripStabilized | twoHandStabilized), shoulderStabilized);

               //On both feels need to identify with hand, intensity and if the other hand is holding.

           }
       }


       [HarmonyPatch(typeof(FistVR.FVRPhysicalObject))]
       [HarmonyPatch("OnCollisionEnter")]
       [HarmonyPatch(new Type[] { typeof(Collision) })]
       public class OnMeleeCollider
       {
           [HarmonyPostfix]
           public static void Postfix(FistVR.FVRPhysicalObject __instance, Collision col)
           {
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
               int intensity = (int)Mathf.Clamp((Math.Min(0.2f + speed / 5.0f, 1.0f))*100, 50, 100);



               owoSkin.FeelWithHand("Melee Attack", intensity, isRightHand);
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
                        owoSkin.Feel("Heal");
                        break;
                    case FistVR.PowerupType.Explosive:
                        owoSkin.Feel("ExplosionFace");
                        break;
                    case FistVR.PowerupType.InfiniteAmmo:
                        owoSkin.Feel("InfiniteAmmo");
                        break;
                    case FistVR.PowerupType.Invincibility:
                        owoSkin.Feel("Invincibility");
                        break;
                    case FistVR.PowerupType.QuadDamage:
                        owoSkin.Feel("QuadDamage");
                        break;
                    case FistVR.PowerupType.SpeedUp:
                        owoSkin.Feel("HeartBeatFast");
                        break;
                    case FistVR.PowerupType.Regen:
                        owoSkin.Feel("Heal");
                        break;
                    case FistVR.PowerupType.MuscleMeat:
                        owoSkin.Feel("MuscleMeat");
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
        #endregion

        [HarmonyPatch(typeof(FistVR.FVRPlayerBody), "KillPlayer", new Type[] { typeof(bool) })]
        public class OnPlayerKilled
        {
            [HarmonyPostfix]
            public static void Postfix()
            {

                //maxHealth = 0; //??

                owoSkin.StopAllHapticFeedback();
                owoSkin.Feel("Death");
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


        [HarmonyPatch(typeof(FistVR.FVRMovementManager), "RocketJump")]
        public class OnRocketJump
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                owoSkin.Feel("Jump");
            }
        }

        [HarmonyPatch(typeof(FistVR.ZosigGameManager), "VomitObject", new Type[] { typeof(FistVR.FVRObject) })]
        public class OnVomitObject
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                owoSkin.Feel("Vomit");
            }
        }

        [HarmonyPatch(typeof(FistVR.ZosigGameManager), "EatBangerJunk")]
        public class OnEatBangerJunk
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                owoSkin.Feel("Eating");
            }
        }

        [HarmonyPatch(typeof(FistVR.ZosigGameManager), "EatHerb")]
        public class OnEatHerb
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                owoSkin.Feel("Eating");
            }
        }

        [HarmonyPatch(typeof(FistVR.ZosigGameManager), "EatMeatCore")]
        public class OnEatMeatCore
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                owoSkin.Feel("Eating");
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
        }

        #region Player damage

        [HarmonyPatch(typeof(FistVR.FVRPlayerHitbox))]
        [HarmonyPatch("Damage")]
        [HarmonyPatch(new Type[] { typeof(FistVR.Damage) })]
        public class OnDamageDealtHitbox
        {
            [HarmonyPostfix]
            public static void Postfix(FistVR.FVRPlayerHitbox __instance, FistVR.Damage d)
            {
                // Get XZ-angle and y-shift of hit
                FistVR.FVRPlayerBody myBody = __instance.Body;
                var angleShift = getAngleAndShift(myBody, d.point);

                // Different hit patterns for different damage classes
                string feedbackKey = "BulletHit";
                switch (d.Class)
                {
                    case FistVR.Damage.DamageClass.Projectile:
                        feedbackKey = "BulletHit";
                        break;
                    case FistVR.Damage.DamageClass.Melee:
                        feedbackKey = "BladeHit";
                        break;
                    case FistVR.Damage.DamageClass.Explosive:
                        feedbackKey = "Impact";
                        break;
                    case FistVR.Damage.DamageClass.Environment:
                        feedbackKey = "Impact";
                        break;
                    case FistVR.Damage.DamageClass.Abstract:
                        feedbackKey = "BulletHit";
                        break;
                    default:
                        break;
                }

                // If it's at the very top, play back a headshot
                if (angleShift.Value == 0.5f) { tactsuitVr.HeadShot(angleShift.Key); }
                else { tactsuitVr.PlayBackHit(feedbackKey, angleShift.Key, angleShift.Value); }

                // Logging from when I tried to figure things out
                //tactsuitVr.LOG("Dealt Body position: " + myBody.TorsoTransform.position.x.ToString() + " " + myBody.TorsoTransform.position.y.ToString() + " " + myBody.TorsoTransform.position.z.ToString());
                //tactsuitVr.LOG("Dealt Hitpoint: " + d.point.x.ToString() + " " + d.point.y.ToString() + " " + d.point.z.ToString());
                //tactsuitVr.LOG("Dealt StrikeDir: " + d.strikeDir.x.ToString() + " " + d.strikeDir.y.ToString() + " " + d.strikeDir.z.ToString());
            }
        }

        #endregion
         */

    }
}
