using FistVR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static FistVR.RemoteGun;

namespace OWO_H3VR
{
    public static class SensationsDictionary
    {
        static string[] shotgunRounds = { "a12g_Shotgun", "a20g_Shotgun", "a3gauge", "a12gaugeShort", "a12GaugeBelted", "a40_46_Grenade"};
        static string[] rocketRounds = { "aRPG7Rocket", "aM1A1Rocket", "aPanzerSchreckRocket" };
        static string[] rifleRounds = { "a50_BMG", "a20x82mm", "a13_2mmTuF", "a408Cheytac", "a50_Remington_BP", "a50mmPotato" };
        static string[] battleRifleRounds = { "a762_51_Nato", "a762_54_Mosin", "a3006_Springfield", "a75x54mmFrench", "a762_54_Mosin", "a300_Winchester_Magnum", "a338Lapua" };
        static string[] assaultRifleRounds = { "a556_45_Nato", "a545_39_Soviet", "a762_39_Soviet", "a280British", "a58x42mm", "a792x33mmKurz", "a762_39_Soviet" };


        public static string SensationByWeaponType(FireArmRoundType roundType)
        {
            string prefix = "Pistol";            

            if (rocketRounds.Any(roundType.ToString().Contains)) { prefix = "Rocket"; }
            if (shotgunRounds.Any(roundType.ToString().Contains)) { prefix = "Shotgun"; }
            if (rifleRounds.Any(roundType.ToString().Contains)) { prefix = "BigRifle"; }
            if (battleRifleRounds.Any(roundType.ToString().Contains)) { prefix = "Rifle"; }
            if (assaultRifleRounds.Any(roundType.ToString().Contains)) { prefix = "Rifle"; }

            return prefix;
        }
    };
}
