using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace OWOGame
{
    internal class FindServer
    {
        readonly Network network;
        readonly NotifyAbscense notifyAbscense;
        readonly ReceiveAvailableApp interpretAppMessage;
        readonly SendAuthMessage sendAuth;
        readonly CandidatesVault candidates;

        public int DelayTime = 500;
        Task TimeBetweenAttempts => Task.Delay(DelayTime);

        public FindServer(Network network, NotifyAbscense notifyAbscense, ReceiveAvailableApp receiveSecretKey,
                          SendAuthMessage sendAuth, CandidatesVault keys)
        {
            this.network = network;
            this.notifyAbscense = notifyAbscense;
            this.interpretAppMessage = receiveSecretKey;
            this.sendAuth = sendAuth;
            this.candidates = keys;
        }

        public async Task ExecuteWithAbscense(Address addressee, string auth)
        {
            await Execute(new string[] { addressee }, auth);
            notifyAbscense.Execute(auth);
        }

        public async Task Execute(string[] addressees, string auth)
        {
            network.State = ConnectionState.Connecting;

            foreach (var address in addressees)
                if (candidates.ContainsCandidate(address)) sendAuth.Execute(auth, address);

            do
            {
                string lastMessage = ReceiveMessage(out var sender);

                if (string.IsNullOrEmpty(lastMessage))
                {
                    foreach (var address in addressees)
                    {
                        NotifyPresence(address);
                    }
                }

                if (lastMessage.Equals("okay"))
                {
                    candidates.Store(new Message("", sender));
                    sendAuth.Execute(auth, sender);
                }
                else if (IsConnectionVerification(addressees, sender, lastMessage))
                {
                    network.Connect(sender);
                }

                await TimeBetweenAttempts;
            }
            while (network.ConnectedServers.Count != addressees.Count() && network.State != ConnectionState.Disconnected);
        }

        bool IsConnectionVerification(string[] addresse, string sender, string lastMessage)
        {
            return lastMessage.Equals("pong") && (addresse.Contains(sender) || addresse[0] == "255.255.255.255");
        }

        string ReceiveMessage(out Address sender) => network.Listen(out sender);

        public async Task Scan()
        {
            while (network.State == ConnectionState.Disconnected)
            {
                NotifyPresence("255.255.255.255");
                await interpretAppMessage.Execute();
                await TimeBetweenAttempts;
            }
        }

        void NotifyPresence(string addressee) => network.SendTo("ping", addressee);
    }
}