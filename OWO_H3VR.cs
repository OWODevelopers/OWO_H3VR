using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;

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

        /// <summary>
        /// Commented because missing game libraries
        /// </summary>
         /*
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
         */
    }
}
