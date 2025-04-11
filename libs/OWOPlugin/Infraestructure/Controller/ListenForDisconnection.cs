using System.Threading.Tasks;

namespace OWOGame
{
    internal class ListenForDisconnection
    {
        readonly Network network;

        public int delayTime = 50;
        Task ListenDelay => Task.Delay(delayTime);

        public ListenForDisconnection(Network network)
        {
            this.network = network;
        }

        public async Task<bool> Listen()
        {
            while (network.State == ConnectionState.Connected)
            {
                var message = network.Listen(out var sender);

                if (message.Equals("OWO_Close") && network.ConnectedServers.Contains(sender))
                {
                    return true;
                }

                await ListenDelay;
            }

            return false;
        }
    }
}