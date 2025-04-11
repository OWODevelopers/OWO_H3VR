using System;
using System.Linq;

namespace OWOGame
{
    public static class MusclesExtensions
    {
        public static Muscle[] WithIntensity(this Muscle[] muscles, int intensity) 
            => muscles.Select(m => m.WithIntensity(intensity)).ToArray();

        public static Muscle Mirror(this Muscle of) => new Muscle(MirrorOf(of.id), of.intensity);

        public static Muscle MultiplyIntensityBy(this Muscle of, Multiplier howMuch) =>
            new Muscle(of.id, howMuch * of.intensity);
        
        public static Muscle[] MultiplyIntensityBy(this Muscle[] of, Multiplier howMuch) =>
            of.Select(muscle => muscle.MultiplyIntensityBy(howMuch)).ToArray();
        public static Muscle[] Mirror(this Muscle[] of) => of.Select(Mirror).ToArray();
        static int MirrorOf(int aPosition) => aPosition % 2 == 0 ? aPosition + 1 : aPosition - 1;
        public static string Stringify(this Muscle muscle) => MusclesBuilder.From(muscle);
        public static string Stringify(this Muscle[] muscles) => MusclesBuilder.From(muscles);
    }
}