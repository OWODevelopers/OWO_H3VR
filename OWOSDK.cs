using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace OWO_H3VR
{
    public class OWOSDK
    {
        readonly Socket socket;
        readonly byte[] buffer;
        int PORT = 54020;
        IPEndPoint remoteEndPoint;
        EndPoint connectedTo = new IPEndPoint(0, 0);
        public bool isConnected = false;

        int gameID = 50891978;
        string auth;

        private DateTime lastSendEndTime;
        private int lastSendPriority = 0;

        public OWOSDK() 
        {
            buffer = new byte[1024];
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.EnableBroadcast = true;
            socket.ReceiveTimeout = 2500;
            socket.Blocking = false;

            remoteEndPoint = new IPEndPoint(IPAddress.Broadcast, PORT);
            //remoteEndPoint = new IPEndPoint(IPAddress.Parse("192.168.217.94"), PORT);
            auth = $"{gameID}*AUTH*";
        }

        #region Connection State

        public IEnumerator StartConnection()
        {
            yield return new WaitForSeconds(.1f);

            Plugin.Log.LogInfo("Start...");            
            string toSend = "ping";
            var authEncoded = System.Text.Encoding.UTF8.GetBytes(auth);

            while (!isConnected)
            {
                Plugin.Log.LogInfo("Sending ping !");

                byte[] sendBytes = Encoding.UTF8.GetBytes(toSend);
                socket.SendTo(sendBytes, remoteEndPoint);

                yield return new WaitForSeconds(.2f);

                try
                {
                    socket.Blocking = false; // No bloquear la recepción inmediatamente
                    var bytes = socket.ReceiveFrom(buffer, ref connectedTo);
                    socket.Blocking = true;

                    if (bytes > 0)
                    {
                        string result = System.Text.Encoding.ASCII.GetString(buffer, 0, bytes);

                        //Plugin.Log.LogInfo("WHAT WAS THAT " + result);

                        if (result == "pong")
                        {
                            Plugin.Log.LogInfo("PONG");

                            isConnected = true;
                        }

                        if (result == "okay")
                        {
                            socket.SendTo(authEncoded, connectedTo);
                        }
                    }
                }
                catch (SocketException ex)
                {
                    if (ex.SocketErrorCode != SocketError.WouldBlock) Plugin.Log.LogError($"SocketEX: {ex.Message}");

                    else Plugin.Log.LogInfo("SocketEX: No data received");            
                }                    

                yield return new WaitForSeconds(.2f);
            }
        }

        public void Disconnect()
        {
            Plugin.Log.LogInfo("Sending sensation");

            byte[] sendBytes = Encoding.UTF8.GetBytes($"OWO_Close");
            socket.SendTo(sendBytes, connectedTo);

            socket.Close();
        }

        #endregion

        #region Communication
        public void Send(string sensation, string sensationOCL = "", int priority = 0) 
        {
            if (!CanSendNextSensation(sensationOCL, priority)) return;

            Plugin.Log.LogInfo($"SENDING SENSATION: {gameID}*SENSATION*{sensation}");

            byte[] sendBytes = Encoding.UTF8.GetBytes($"{gameID}*SENSATION*{sensation}");

            socket.SendTo(sendBytes, connectedTo);
        }

        public void Stop() 
        {
            byte[] sendBytes = Encoding.UTF8.GetBytes($"{gameID}*STOP");

            socket.SendTo(sendBytes, connectedTo);
        }

        public void CreateAuth(String[] allSensations) 
        {            
            for (int i = 0; i < allSensations.Length; i++) 
            {
                auth += $"{allSensations[i]}#";
            }

            auth = auth.Substring(0, auth.Length - 1);
            Plugin.Log.LogInfo($"### AUTH: {auth}");
        }

        #endregion

        #region priority management

        private bool CanSendNextSensation(string sensationOCL, int priority)
        {
            if (priority >= lastSendPriority || lastSendEndTime <= DateTime.Now) 
            {
                lastSendPriority = priority;
                lastSendEndTime = DateTime.Now.AddMilliseconds(GetSensationMilliseconds(sensationOCL));
                return true;
            }

            return false;
        }

        private int GetSensationMilliseconds(string sensationOCL)
        {

            Plugin.owoSkin.LOG($"### SENSATION OCL:{sensationOCL}");            

            var micros = sensationOCL.Split('~')[2].Split('&');
            var result = 0;

            foreach (var micro in micros)
            {
                result += Int32.Parse(micro.Split(',')[1]) + Int32.Parse(micro.Split(',')[5]);
            }

            result = result * 100;
            return result;
        }

        #endregion

    }
}
