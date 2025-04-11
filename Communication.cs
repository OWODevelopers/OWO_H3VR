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
        int gameID = 0;
        int PORT = 54020;
        IPEndPoint remoteEndPoint;
        EndPoint connectedTo = new IPEndPoint(0, 0);

        public Communication() 
        {
            buffer = new byte[1024];
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.EnableBroadcast = true;
            socket.ReceiveTimeout = 2500;
            socket.Blocking = false;

            remoteEndPoint = new IPEndPoint(IPAddress.Broadcast, PORT);
        }


        public IEnumerator StartConnection()
        {
            yield return new WaitForSeconds(.2f);

            Plugin.Log.LogInfo("Start...");
            bool connection = false;
            string toSend = "ping";
            var auth = System.Text.Encoding.UTF8.GetBytes($"{gameID}AUTH*");

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
                Plugin.Log.LogInfo("WHAT WAS THAT " + result);

                if (result == "pong")
                {
                    connection = true;
                }

                if (result == "okay") 
                {                    
                    socket.SendTo(auth, remoteEndPoint);
                    Plugin.Log.LogInfo("HOLY MOLY ");
                }

                yield return new WaitForSeconds(.1f);
            }
        }

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

    }
}
