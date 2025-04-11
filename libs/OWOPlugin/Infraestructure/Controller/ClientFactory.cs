namespace OWOGame
{
    internal static class ClientFactory
    {
        public static Client Create(Network network, Message secretKey = default, CandidatesVault keysVault = null, int scanDelayMs = 500)
        {
            if (keysVault == null)
                keysVault = new CandidatesVault();

            if (secretKey.addressee != null)
            {
                keysVault.Store(secretKey);
            }

            var sendMessage = new SendMessage(network);
            var notifyAbscense = new NotifyAbscense(network, keysVault);
            var sendAuth = new SendAuthMessage(sendMessage, keysVault);
            var receiveSecretKey = new ReceiveAvailableApp(keysVault, network);
            var findServer = new FindServer(network, notifyAbscense, receiveSecretKey, sendAuth, keysVault)
            {
                DelayTime = scanDelayMs
            };
            
            var reconnect = new ListenForDisconnection(network);

            return new Client(network, sendMessage, findServer, reconnect, keysVault);
        }
    }
}