namespace OWOGame
{
    internal class SendMessage
    {
        readonly Network network;

        public SendMessage(Network network)
        {
            this.network = network;
        }

        public void Execute(string message, Address addressee)
        {
            if (!addressee.IsValid) return;

            network.SendTo(message, addressee);
        }
    }
}