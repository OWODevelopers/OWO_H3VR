using FistVR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace OWO_H3VR
{
    public class OWOSkin
    {
        public OWOSDK owoSDK;
        public bool suitEnabled = false;
        public bool playerIsAlive = true;

        //        private static bool heartBeatIsActive = false;
        //        private int heartbeatCount = 0;

        public Dictionary<String, String> FeedbackMap = new Dictionary<String, String>();

        public OWOSkin(OWOSDK owoSdkRef)
        {
            owoSDK = owoSdkRef;
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
                string owoString = File.ReadAllText(fullName);
                try
                {
                    //Sensation test = Sensation.Parse(owoString);
                    FeedbackMap.Add(prefix, owoString);
                }
                catch (Exception e) { LOG(e.Message); }

            }
        }

        private void InitializeOWO()
        {
            LOG("Initializing OWO skin");

            owoSDK.CreateAuth(AllBakedSensations());
            //StartCoroutine(owoSDK.StartConnection());

            //suitEnabled = true;
            //LOG("OWO suit connected.");

            //string[] myIPs = GetIPsFromFile("OWO_Manual_IP.txt");
            //if (myIPs.Length == 0) await OWO.AutoConnect();
            //else
            //{
            //    await OWO.Connect(myIPs);
            //}

            //if (OWO.ConnectionState == OWOGame.ConnectionState.Connected)
            //{
            //    suitEnabled = true;
            //    LOG("OWO suit connected.");
            //    Feel("Heart Beat");
            //}
            //if (!suitEnabled) LOG("OWO is not enabled?!?!");
        }

        public IEnumerator FinalizeOWOConnection()
        {
            while (!owoSDK.isConnected) {
                yield return new WaitForSeconds(.2f);
            }

            suitEnabled = true;
            LOG("OWO suit connected.");
            Feel("Heart Beat");
            yield return null;
        }

        public String[] AllBakedSensations()
        {
            var result = new List<String>();

            foreach (var sensation in FeedbackMap)
            {
                LOG("Registered baked sensation: " + sensation.Key);
                result.Add(sensation.Value);
            }
            return result.ToArray();
        }

        //public string[] GetIPsFromFile(string filename)
        //{
        //    List<string> ips = new List<string>();
        //    string filePath = Directory.GetCurrentDirectory() + "\\BepinEx\\Plugins\\OWO" + filename;
        //    if (File.Exists(filePath))
        //    {
        //        LOG("Manual IP file found: " + filePath);
        //        var lines = File.ReadLines(filePath);
        //        foreach (var line in lines)
        //        {
        //            if (IPAddress.TryParse(line, out _)) ips.Add(line);
        //            else LOG("IP not valid? ---" + line + "---");
        //        }
        //    }
        //    return ips.ToArray();
        //}

        ~OWOSkin()
        {
            LOG("Destructor called");
            DisconnectOWO();
        }

        public void DisconnectOWO()
        {
            LOG("Disconnecting OWO skin.");
            owoSDK.Disconnect();
        }
#endregion

        public void LOG(String msg)
        {
            Plugin.Log.LogInfo(msg);
        }

        #region Feels
        public void Feel(String key, int priority = 0, int intensity = 0)
        {
            if (FeedbackMap.ContainsKey(key))
            {
                String toSend = FeedbackMap[key].Split('~')[0]; //enviamos solo el ID al ser baked

                if (intensity != 0)
                {
                    toSend += "|"; //separamos los musculos

                    for (int i = 0; i < 10; i++)
                    {
                        toSend += $"{i}%{intensity},"; //asignamos la intensidad a cada musculo
                    }                    
                }

                owoSDK.Send(toSend, FeedbackMap[key], priority);
            }

            else LOG("Feedback not registered: " + key);
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

        #endregion


        //        public void Feel360(String key, float myRotation)
        //        {
        //            Sensation toSend = FeedbackMap[key];

        //            if (myRotation >= 0 && myRotation <= 180)
        //            {
        //                if (myRotation >= 0 && myRotation <= 90) toSend = toSend.WithMuscles(Muscle.Dorsal_L, Muscle.Lumbar_L);
        //                else toSend = toSend.WithMuscles(Muscle.Dorsal_R, Muscle.Lumbar_R);
        //            }
        //            else
        //            {
        //                if (myRotation >= 270 && myRotation <= 359) toSend = toSend.WithMuscles(Muscle.Pectoral_L, Muscle.Abdominal_L);
        //                else toSend.WithMuscles(Muscle.Pectoral_R, Muscle.Abdominal_R);
        //            }

        //            if (!suitEnabled) { return; }
        //            OWO.Send(toSend.WithPriority(3));
        //        }

        //        #endregion

        //        public string ConfigureRecoilBulletName(string bulletName)
        //        {
        //            SensationsDictionary.RecoilSensations.TryGetValue(bulletName, out string sensation);

        //            if (sensation == null)
        //            {
        //                return "Default";
        //            }

        //            return sensation;
        //        }
        //        /*
        //        //NEED TO TEST
        public float GetHitAngle(FistVR.FVRPlayerBody player, Damage d)
        {
            float num = Vector3.Angle(d.strikeDir, GM.CurrentPlayerBody.Head.forward);
            float num2 = Vector3.Angle(d.strikeDir, GM.CurrentPlayerBody.Head.right);


            if (num > 90f)
            {
                LOG("### ESTOY MIRANDO");
                if (num2 > 90f)
                {
                    LOG("### DERECHA");
                }
                else {
                    LOG("### IZQUIERDA");
                }
            }
            else 
            { 
               LOG("### ESTOY DE ESPALDAS");

                if (num2 > 90f)
                {
                    LOG("### DERECHA");
                }
                else
                {
                    LOG("### IZQUIERDA");
                }
            }
            
            LOG("### ANGLE" + num);

            return 0.0f;
        }         

        //        #region heart beat loop
        //        public void StartHeartBeat()
        //        {
        //            if (heartBeatIsActive) return;

        //            heartBeatIsActive = true;
        //            HeartBeatFuncAsync();
        //        }

        //        public void StopHeartBeat()
        //        {
        //            heartbeatCount = 0;
        //            heartBeatIsActive = false;
        //        }

        //        public async Task HeartBeatFuncAsync()
        //        {
        //            while (heartBeatIsActive && heartbeatCount <= 15)
        //            {
        //                heartbeatCount++;
        //                Feel("Heart Beat", 0);
        //                await Task.Delay(1000);
        //            }

        //            StopHeartBeat();
        //        }
        //        #endregion


        public void StopAllHapticFeedback()
        {
            owoSDK.Stop();
        }

        public bool CanFeel()
        {
            return suitEnabled && playerIsAlive;
        }
    }
    }
