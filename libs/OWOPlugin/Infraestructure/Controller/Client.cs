using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace OWOGame
{
    public class Client
    {
        readonly Network network;
        readonly SendMessage sendMessage;
        readonly FindServer findServer;
        readonly ListenForDisconnection disconnection;
        readonly CandidatesVault candidates;

        public ConnectionState State => network.State;

        public bool IsConnected => network.ConnectedServers != null && network.ConnectedServers.Count != 0;
        bool CanScan => network.State == ConnectionState.Disconnected;
        public List<string> DiscoveredServers => candidates.StoredServers;

        internal Client(Network network, SendMessage sendMessage, FindServer findServer,
                        ListenForDisconnection disconnection, CandidatesVault keys)
        {
            this.network = network;
            this.sendMessage = sendMessage;
            this.findServer = findServer;
            this.disconnection = disconnection;
            this.candidates = keys;
        }

        ~Client() => Close();

        internal Task ScanServer()
        {
            if(!CanScan)
                return Task.CompletedTask;

            candidates.Clean();
            return findServer.Scan();
        }

        internal Task FindServer(string auth, params string[] addresses)
        {
            if (!CanScan) return Task.CompletedTask;

            if (addresses[0] == "255.255.255.255") candidates.Clean();
            if (addresses.Length == 1) return FindServer(addresses[0], auth);

            return Task.Run(() => findServer.Execute(addresses, auth));
        }

        async Task FindServer(string address, string auth)
        {
            await findServer.ExecuteWithAbscense(address, auth);

            _ = ListenForDisconnection(address, auth);
        }

        async Task ListenForDisconnection(string addressee, string auth)
        {
            if (await disconnection.Listen())
            {
                Disconnect();
                await FindServer(addressee, auth);
            }
        }

        public void Send(string message)
        {
            network.ConnectedServers.ForEach(server => sendMessage.Execute(message, server));
        }

        public void Disconnect()
        {
            network.Disconnect();
        }

        public void Close() => network.Close();
        public void ChangeConnectionAttemptRate(int newRate)
        {
            findServer.DelayTime = newRate;
            disconnection.delayTime = newRate;
        }

        public void PortTo(int newPort) => network.PortTo(newPort);
    }
}