using System.IO;

namespace OWOGame
{
    public class MicroSensation : Sensation
    {
        public readonly int frequency;
        public readonly float duration;
        public readonly int intensity;
        public readonly float rampUp;
        public readonly float rampDown;
        public readonly float exitDelay;
        public readonly string name;

        public override float Duration => duration + exitDelay;

        internal MicroSensation(int frequency, float duration, int intensity,
                                float rampUp, float rampDown, float exitDelay, string name = "")
        {
            this.frequency = Math.Clamp(frequency, 1, 100);
            this.duration = Math.Round(Math.Clamp(duration, 0.1f, 20f));
            this.intensity = Math.Clamp(intensity, 0, 100);
            this.rampUp = Math.Round(Math.Clamp(rampUp, 0, 2));
            this.rampDown = Math.Round(Math.Clamp(rampDown, 0, 2));
            this.exitDelay = Math.Round(Math.Clamp(exitDelay, 0, 20f));
            this.name = name;
        }
        public override Sensation MultiplyIntensityBy(Multiplier howMuch)
        {
            return new MicroSensation(frequency, duration, intensity * howMuch, rampUp, rampDown, exitDelay, name);
        }

        public MicroSensation WithName(string name)
        {
            return new MicroSensation(frequency, duration, intensity, rampUp, rampDown, exitDelay, name).WithPriority(Priority) as MicroSensation;
        }


    }
}