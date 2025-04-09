using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace OWO_H3VR
{

    public class Plugin : BaseUnityPlugin //CAMBIAR LAS REFERENCIAS DE UNITY POR LAS DEL JUEGO
    {

        #pragma warning disable CS0109
        internal static new ManualLogSource Log;
        #pragma warning restore CS0109

        public static OWOSkin owoSkin;

        private void Awake()
        {
            Log = Logger;
            Logger.LogMessage("OWO_REPO plugin is loaded!");
            owoSkin = new OWOSkin();

            var harmony = new Harmony("owo.patch.repo");
            harmony.PatchAll();
        }

    }
}
