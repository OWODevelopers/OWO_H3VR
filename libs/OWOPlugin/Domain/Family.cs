namespace OWOGame
{
    public struct Family
    {
        readonly string name;

        Family(string name)
        {
            this.name = name;
        }
        
        public static implicit operator string (Family family) => family.name;
        public static implicit operator Family (string name) => new Family(name);
        public static Family None { get; } = "";

        public override string ToString() => name;
    }
}