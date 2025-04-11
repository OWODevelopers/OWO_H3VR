namespace OWOGame
{
    public partial struct Icon
    {
        readonly string name;
        internal Icon(string name) => this.name = name;

        public static implicit operator Icon(string message) => new Icon(message);
        public static implicit operator string(Icon icon) => icon.name;
        public override string ToString() => this;
    }
}