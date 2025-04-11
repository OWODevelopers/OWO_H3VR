using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace OWOGame
{
    internal class UDPNetwork : Network
    {
        readonly byte[] buffer;
        readonly Socket socket;
        readonly ASCIIEncoder encoding;

        public List<Address> ConnectedServers { get; private set; } = new List<Address>();
        public ConnectionState State { get; set; } = ConnectionState.Disconnected;
        public bool IsConnecting => State == ConnectionState.Connecting;
        public bool IsConnected => State == ConnectionState.Connected;
        int PORT = 54020;

        public UDPNetwork()
        {
            buffer = new byte[1024];
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.EnableBroadcast = true;
            socket.ReceiveTimeout = 2500;
            socket.Blocking = false;

            this.encoding = new ASCIIEncoder();
        }

        public string Listen(out Address address)
        {
            try
            {
                EndPoint endPoint = new IPEndPoint(0, 0);
                var result = encoding.Decode(buffer, socket.ReceiveFrom(buffer, ref endPoint));

                address = new Address((endPoint as IPEndPoint).Address.ToString());
                return result;
            }
            catch
            {
                address = Address.Empty;
                return string.Empty;
            }
        }

        public void SendTo(string message, string addressee)
        {
            socket.SendTo(encoding.Encode(message), new IPEndPoint(IPAddress.Parse(addressee), PORT));
        }

        public void Connect(Address address)
        {
            if (!ConnectedServers.Contains(address))
                ConnectedServers.Add(address);

            State = ConnectionState.Connected;
        }

        public void Disconnect()
        {
            ConnectedServers.Clear();
            State = ConnectionState.Disconnected;
        }

        public void Close() => socket.Close();
        public void PortTo(int newPort) => PORT = newPort;
    }
}