using System.Collections.Generic;

namespace OWOGame
{
    public enum ConnectionState { Connected, Disconnected, Connecting }

    public interface Network
    {
        List<Address> ConnectedServers { get; }
        ConnectionState State { get; set; }
        bool IsConnecting { get; }
        bool IsConnected { get; }

        void SendTo(string message, string addressee);
        string Listen(out Address sender);
        void Connect(Address server);
        void Disconnect();
        void Close();
        void PortTo(int newPort);
    }
}