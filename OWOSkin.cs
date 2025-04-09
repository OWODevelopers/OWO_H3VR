using OWOGame;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;

namespace OWO_H3VR
{
    public class OWOSkin
    {
        public bool suitEnabled = false;
        private static bool heartBeatIsActive = false;
        private int heartbeatCount = 0;

        public Dictionary<String, Sensation> FeedbackMap = new Dictionary<String, Sensation>();

        public OWOSkin()
        {
            RegisterAllSensationsFiles();
            InitializeOWO();
        }

        #region Skin Configuration

        private void RegisterAllSensationsFiles()
        {
            string configPath = Directory.GetCurrentDirectory() + "\\BepinEx\\Plugins\\OWO";
            DirectoryInfo d = new DirectoryInfo(configPath);
            FileInfo[] Files = d.GetFiles("*.owo", SearchOption.AllDirectories);
            for (int i = 0; i < Files.Length; i++)
            {
                string filename = Files[i].Name;
                string fullName = Files[i].FullName;
                string prefix = Path.GetFileNameWithoutExtension(filename);
                if (filename == "." || filename == "..")
                    continue;
                string tactFileStr = File.ReadAllText(fullName);
                try
                {
                    Sensation test = Sensation.Parse(tactFileStr);
                    FeedbackMap.Add(prefix, test);
                }
                catch (Exception e) { LOG(e.Message); }

            }
        }

        private async void InitializeOWO()
        {
            LOG("Initializing OWO skin");

            var gameAuth = GameAuth.Create(AllBakedSensations()).WithId("0");

            OWO.Configure(gameAuth);
            string[] myIPs = GetIPsFromFile("OWO_Manual_IP.txt");
            if (myIPs.Length == 0) await OWO.AutoConnect();
            else
            {
                await OWO.Connect(myIPs);
            }

            if (OWO.ConnectionState == OWOGame.ConnectionState.Connected)
            {
                suitEnabled = true;
                LOG("OWO suit connected.");
                Feel("Heart Beat");
            }
            if (!suitEnabled) LOG("OWO is not enabled?!?!");
        }

        public BakedSensation[] AllBakedSensations()
        {
            var result = new List<BakedSensation>();

            foreach (var sensation in FeedbackMap.Values)
            {
                if (sensation is BakedSensation baked)
                {
                    LOG("Registered baked sensation: " + baked.name);
                    result.Add(baked);
                }
                else
                {
                    LOG("Sensation not baked? " + sensation);
                    continue;
                }
            }
            return result.ToArray();
        }

        public string[] GetIPsFromFile(string filename)
        {
            List<string> ips = new List<string>();
            string filePath = Directory.GetCurrentDirectory() + "\\BepinEx\\Plugins\\OWO" + filename;
            if (File.Exists(filePath))
            {
                LOG("Manual IP file found: " + filePath);
                var lines = File.ReadLines(filePath);
                foreach (var line in lines)
                {
                    if (IPAddress.TryParse(line, out _)) ips.Add(line);
                    else LOG("IP not valid? ---" + line + "---");
                }
            }
            return ips.ToArray();
        }

        ~OWOSkin()
        {
            LOG("Destructor called");
            DisconnectOWO();
        }

        public void DisconnectOWO()
        {
            LOG("Disconnecting OWO skin.");
            OWO.Disconnect();
        }
        #endregion

        public void LOG(String msg)
        {
            Plugin.Log.LogInfo(msg);
        }

        #region Feels
        public void Feel(String key, int Priority = 0, int intensity = 0)
        {
            if (FeedbackMap.ContainsKey(key))
            {
                Sensation toSend = FeedbackMap[key];

                if (intensity != 0)
                {
                    toSend = toSend.WithMuscles(Muscle.All.WithIntensity(intensity));
                }

                OWO.Send(toSend.WithPriority(Priority));
            }

            else LOG("Feedback not registered: " + key);
        }
        public void FeelWithBothHand(String key, int priority = 0, bool isRightHand = true, int intensity = 0)
        {

            if (isRightHand)
            {
                key += " RL";
            }
            else
            {
                key += " LR";
            }

            Feel(key, priority, intensity);
        }

        public void FeelWithHand(String key, int priority = 0, bool isRightHand = true, int intensity = 0)
        {

            if (isRightHand)
            {
                key += " R";
            }
            else
            {
                key += " L";
            }

            Feel(key, priority, intensity);
        }

        public void Feel360(String key, float myRotation)
        {
            Sensation toSend = FeedbackMap[key];

            if (myRotation >= 0 && myRotation <= 180)
            {
                if (myRotation >= 0 && myRotation <= 90) toSend = toSend.WithMuscles(Muscle.Dorsal_L, Muscle.Lumbar_L);
                else toSend = toSend.WithMuscles(Muscle.Dorsal_R, Muscle.Lumbar_R);
            }
            else
            {
                if (myRotation >= 270 && myRotation <= 359) toSend = toSend.WithMuscles(Muscle.Pectoral_L, Muscle.Abdominal_L);
                else toSend.WithMuscles(Muscle.Pectoral_R, Muscle.Abdominal_R);
            }

            if (!suitEnabled) { return; }
            OWO.Send(toSend.WithPriority(3));
        }

        #endregion

        public string ConfigureRecoilBulletName(string bulletName)
        {
            SensationsDictionary.RecoilSensations.TryGetValue(bulletName, out string sensation);

            if (sensation == null)
            {
                return "Default";
            }

            return sensation;
        }
        /*
        //NEED TO TEST
        public float GetHitAngle(FistVR.FVRPlayerBody player, Vector3 hit)
        {

            Vector3 patternOrigin = new Vector3(0f, 0f, 1f);
            Vector3 hitPosition = hit - player.TorsoTransform.position;
            Quaternion PlayerRotation = player.TorsoTransform.rotation;
            Vector3 playerDir = PlayerRotation.eulerAngles;
            // get rid of the up/down component to analyze xz-rotation
            Vector3 flattenedHit = new Vector3(hitPosition.x, 0f, hitPosition.z);

            // get angle. .Net < 4.0 does not have a "SignedAngle" function...
            float hitAngle = Vector3.SignedAngle(flattenedHit, patternOrigin,Vector3.up);

            float myRotation = hitAngle - playerDir.y;
            myRotation *= -1f;

            return myRotation;

        }
        */

        #region heart beat loop
        public void StartHeartBeat()
        {
            if (heartBeatIsActive) return;

            heartBeatIsActive = true;
            HeartBeatFuncAsync();
        }

        public void StopHeartBeat()
        {
            heartbeatCount = 0;
            heartBeatIsActive = false;
        }

        public async Task HeartBeatFuncAsync()
        {
            while (heartBeatIsActive && heartbeatCount <= 15)
            {
                heartbeatCount++;
                Feel("Heart Beat", 0);
                await Task.Delay(1000);
            }

            StopHeartBeat();
        }
        #endregion


        public void StopAllHapticFeedback()
        {


            OWO.Stop();
        }

        public bool CanFeel()
        {
            return suitEnabled;
        }
    }
}
