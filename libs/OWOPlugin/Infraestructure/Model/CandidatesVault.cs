using System.Collections.Generic;
using System.Linq;

namespace OWOGame
{
    internal class CandidatesVault
    {
        public Address LastApp => candidateServers.FirstOrDefault();
        public List<string> StoredServers => candidateServers.Select(candidate => candidate.value).ToList();

        HashSet<Address> candidateServers = new HashSet<Address>();

        public void Store(Message message)
        {
            if (message.HasAddressee)
                candidateServers.Add(message.addressee);
        }

        public void Clean() => candidateServers.Clear();
        public bool ContainsCandidate(Address address) => candidateServers.Contains(address);
    }
}