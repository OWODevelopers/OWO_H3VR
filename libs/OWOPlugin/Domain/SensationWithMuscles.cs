namespace OWOGame
{
    public class SensationWithMuscles : Sensation
    {
        public readonly Sensation reference;
        public readonly Muscle[] muscles;

        public override float Duration => reference.Duration;

        public SensationWithMuscles(Sensation reference, Muscle[] muscles)
        {
            this.reference = reference;
            this.muscles = muscles;
        }

        public override Sensation MultiplyIntensityBy(Multiplier howMuch)
        {
            return new SensationWithMuscles(this.reference, muscles.MultiplyIntensityBy(howMuch));
        }
    }
}