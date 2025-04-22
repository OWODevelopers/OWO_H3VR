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

        public void FeelDynamicDamage(FistVR.FVRPlayerBody player, Damage d)
        {
            string sensation = "70,1,90,0,0,0,Hurt";
            float num = Vector3.Angle(d.strikeDir, GM.CurrentPlayerBody.Head.forward);
            float num2 = Vector3.Angle(d.strikeDir, GM.CurrentPlayerBody.Head.right);


            if (num > 90f) //Facing
            {                
                if (num2 > 90f) //Right
                {
                    sensation += "|0%100, 1%50, 2%100, 3%50";
                }
                else { //Left
                    sensation += "|0%50, 1%100, 2%50, 3%100";
                }
            }
            else //Back
            { 
                if (num2 > 90f) //Right
                {
                    sensation += "|6%100, 7%50, 8%100, 9%50";
                }
                else //Left
                {
                    sensation += "|6%50, 7%100, 8%50, 9%100";
                }
            }

            owoSDK.SendDynamic(sensation, 3);
        }         


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
