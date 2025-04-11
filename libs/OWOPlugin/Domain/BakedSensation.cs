namespace OWOGame
{
    public class BakedSensation : Sensation
    {
        public readonly int id;
        public readonly string name;
        public readonly Family Family;
        public readonly Sensation reference;
        public readonly Icon icon;

        public override float Duration => reference.Duration;

        internal BakedSensation(int id, string name, Sensation reference, Icon icon, Family family)
        {
            this.id = id;
            this.name = name;
            this.reference = reference;
            this.icon = icon;
            this.Family = family;
        }

        public static new BakedSensation Parse(string message) => (Sensation)message as BakedSensation;
        
        public override Sensation MultiplyIntensityBy(Multiplier howMuch)
        {
            return new BakedSensation(id, name, reference, icon, Family);
        }

        public BakedSensation WithIcon(Icon icon) => new BakedSensation(id, name, reference, icon, Family).WithPriority(Priority) as BakedSensation;
        public BakedSensation BelongsTo(Family family) => new BakedSensation(id, name, reference, icon, family).WithPriority(Priority) as BakedSensation;
        public string Stringify() => BakedSensationsBuilder.Stringify(this);
    }
}