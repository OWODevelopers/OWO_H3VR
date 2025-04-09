using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

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

    }
}
