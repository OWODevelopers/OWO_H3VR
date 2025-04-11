namespace OWOGame
{
    public abstract partial class Sensation
    {
        public int Priority { get; set; } = 0;
        public abstract float Duration { get; }

        public static Sensation Parse(string message) => message;
        public static implicit operator Sensation(string message) => SensationsParser.From(message);
        public static implicit operator string(Sensation sensation) => SensationsBuilder.From(sensation);
        public override string ToString() => this;
        public abstract Sensation MultiplyIntensityBy(Multiplier howMuch);
    }
}