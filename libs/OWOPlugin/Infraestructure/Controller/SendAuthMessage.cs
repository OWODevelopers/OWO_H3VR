namespace OWOGame
{
    internal class SendAuthMessage
    {
        readonly SendMessage send;
        readonly CandidatesVault keys;

        public SendAuthMessage(SendMessage send, CandidatesVault keys)
        {
            this.send = send;
            this.keys = keys;
        }

        public void Execute(string auth, Address addressee)
        {
            if (addressee.Equals(Address.Any))
            {
                foreach(var address in keys.StoredServers)
                    send.Execute(auth, address);
            }
            else if (keys.ContainsCandidate(addressee))
            {
                send.Execute(auth, addressee);
            }
        }
    }
}