namespace OWOGame.Controller
{
    internal class SendMessage
    {
        readonly Client client;
        public SendMessage(Client client) => this.client = client;

        public void Execute(string message)
        {
            if (!client.IsConnected) return;

            client.Send(message);
        }
    }
}