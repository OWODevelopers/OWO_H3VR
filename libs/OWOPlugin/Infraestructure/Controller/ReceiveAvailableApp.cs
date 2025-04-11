using System.Threading.Tasks;

namespace OWOGame
{
    internal class ReceiveAvailableApp
    {
        readonly Network network;
        readonly CandidatesVault keys;

        public ReceiveAvailableApp(CandidatesVault secretKeys, Network network)
        {
            this.keys = secretKeys;
            this.network = network;
        }

        public Task Execute()
        {
            var message = network.Listen(out Address sender);

            if (!message.Equals("okay"))
                return Task.CompletedTask;

            keys.Store(new Message("", sender));

            return Task.CompletedTask;
        }
    }
}