namespace OWOGame
{
    internal static class SensationsBuilder
    {
        public static string From(Sensation sensation)
        {
            if (sensation is MicroSensation micro) return From(micro);
            if (sensation is SensationWithMuscles withMuscles) return From(withMuscles);
            if (sensation is SensationsSequence sequence) return From(sequence);

            return BakedSensationsBuilder.From(sensation as BakedSensation);
        }

        private static string From(SensationsSequence sequence)
        {
            var result = From(sequence.sensations[0]);

            for (int i = 1; i < sequence.sensations.Count; i++)
            {
                result += $"&{From(sequence.sensations[i])}";
            }

            return result;
        }

        private static string From(SensationWithMuscles sensation)
        {
            return From(sensation.reference) + "|" + sensation.muscles.Stringify();
        }

        private static string From(MicroSensation microsensation)
        {
            return $"{microsensation.frequency},{(int)Math.Round(microsensation.duration * 10)},{microsensation.intensity}," +
                   $"{(int)Math.Round(microsensation.rampUp * 1000)},{(int)Math.Round(microsensation.rampDown * 1000)},{(int)Math.Round(microsensation.exitDelay * 10)},{microsensation.name}";
        }
    }
}