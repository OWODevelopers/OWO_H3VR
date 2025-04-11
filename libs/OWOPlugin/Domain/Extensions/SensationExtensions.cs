using System.Linq;

namespace OWOGame
{
    public static class SensationExtensions
    {
        public static Sensation WithPriority(this Sensation source, int priority)
        {
            source.Priority = priority;
            return source;
        }
        
        public static Sensation Append(this Sensation source, Sensation addend)
        {
            return new SensationsSequence(source, addend).WithPriority(source.Priority);
        }
 
        public static BakedSensation Bake(this Sensation source, int id, string name)
        {
            if (source is BakedSensation bake) return bake;

            return new BakedSensation(id, name, source, Icon.Empty, Family.None).WithPriority(source.Priority) as BakedSensation;
        }

        public static Sensation WithMuscles(this Sensation source, params Muscle[] muscles)
        {
            if (muscles.Length <= 0 || source is SensationWithMuscles) return source;
            if (source is SensationsSequence sequence)
            {
                return new SensationsSequence(sequence.sensations.Select(s => s.WithMuscles(muscles)).ToArray()).WithPriority(source.Priority);
            }

            return new SensationWithMuscles(source, muscles).WithPriority(source.Priority);
        }
    }
}