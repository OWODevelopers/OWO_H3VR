namespace OWOGame
{
    internal struct Message
    {
        public readonly string value;
        public readonly string addressee;

        public bool IsEmpty => string.IsNullOrEmpty(value);
        public bool HasAddressee => !string.IsNullOrEmpty(addressee);

        public Message(string value, string addresseeIP)
        {
            this.value = value;
            this.addressee = addresseeIP;
        }

        public static Message Invalid => new Message(string.Empty, Address.Empty);
    }
}