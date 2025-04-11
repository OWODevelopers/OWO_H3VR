using System.Collections.Generic;
using System.Linq;

namespace OWOGame
{
    public class SensationsSequence : Sensation
    {
        public readonly List<Sensation> sensations;

        public override float Duration => sensations.Sum(s => s.Duration);

        public SensationsSequence(params Sensation[] sensations)
        {
            this.sensations = new List<Sensation>(sensations);
        }
        
        public override Sensation MultiplyIntensityBy(Multiplier howMuch)
        {
            return new SensationsSequence(sensations.Select(s => s.MultiplyIntensityBy(howMuch)).ToArray());
        }
    }
}