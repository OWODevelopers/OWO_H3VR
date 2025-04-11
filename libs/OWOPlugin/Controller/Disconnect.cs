namespace OWOGame.Controller
{
    internal class Disconnect
    {
        readonly Client client;
        public Disconnect(Client client) => this.client = client;

        public void Execute()
        {
            if (client.State == ConnectionState.Disconnected) return;

            client.Disconnect();
        }
    }
}