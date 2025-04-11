namespace OWOGame
{
    public struct Address
    {
        public readonly string value;
        public bool IsValid => !string.IsNullOrEmpty(value);

        public Address(string value) => this.value = value;

        public static implicit operator string(Address addressee) => addressee.value;
        public static implicit operator Address(string value) => new Address(value);

        public static Address Create(string ip) => new Address(ip);
        public static Address Any => new Address("255.255.255.255");
        public static Address Empty => new Address(string.Empty);
        public static Address Null => new Address(null);
    }
}