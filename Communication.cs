using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace OWO_H3VR
{
    public class Communication
    {
        readonly Socket socket;
        readonly byte[] buffer;
        int PORT = 54020;
        IPEndPoint remoteEndPoint;
        EndPoint connectedTo = new IPEndPoint(0, 0);

        int gameID = 0;
        string auth;

        public Communication() 
        {
            buffer = new byte[1024];
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.EnableBroadcast = true;
            socket.ReceiveTimeout = 2500;
            socket.Blocking = false;

            remoteEndPoint = new IPEndPoint(IPAddress.Broadcast, PORT);
            auth = $"{gameID}*AUTH*";
        }

        #region Connection State

        public IEnumerator StartConnection()
        {
            yield return new WaitForSeconds(.2f);

            Plugin.Log.LogInfo("Start...");
            bool connection = false;
            string toSend = "ping";
            var authEncoded = System.Text.Encoding.UTF8.GetBytes(auth);

            while (!connection)
            {
                Plugin.Log.LogInfo("Sending ping !");

                byte[] sendBytes = Encoding.UTF8.GetBytes(toSend);

                socket.SendTo(sendBytes, remoteEndPoint);

                yield return new WaitForSeconds(.2f);
                //socket.Send(sendBytes, sendBytes.Length, remoteEndPoint);

                EndPoint endPoint = new IPEndPoint(0, 0);
                var bytes = socket.ReceiveFrom(buffer, ref connectedTo);

                //byte[] bytes = udpClient.Receive(ref remoteEndPoint);
                string result = System.Text.Encoding.ASCII.GetString(buffer,0,bytes);
                //Plugin.Log.LogInfo("WHAT WAS THAT " + result);

                if (result == "pong")
                {
                    connection = true;
                }

                if (result == "okay") 
                {                    
                    socket.SendTo(authEncoded, remoteEndPoint);                    
                }

                yield return new WaitForSeconds(.1f);
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
        public void Send(int sensationId) 
        {
            Plugin.Log.LogInfo("Sending sensation");

            byte[] sendBytes = Encoding.UTF8.GetBytes($"{gameID}*SENSATION*{sensationId}");

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
                auth += $"{i}~{allSensations[i]}#";
            }

            auth = auth.Substring(0, auth.Length - 1);
        }

        #endregion

    }
}
