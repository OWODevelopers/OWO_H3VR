namespace OWOGame
{
    internal class NotifyAbscense
    {
        readonly Network network;
        readonly CandidatesVault candidates;

        public NotifyAbscense(Network network, CandidatesVault candidates)
        {
            this.network = network;
            this.candidates = candidates;
        }

        public void Execute(string authCommand)
        {
            var gameId = authCommand.Split('*')[0];

            foreach (var server in candidates.StoredServers)
            {
                var message = gameId + "*GAMEUNAVAILABLE";

                network.SendTo(message, server);
            }
        }
    }
}