namespace OWOGame
{
    internal static class SensationsParser
    {
        public static Sensation From(string message)
        {
            if (BakedSensationsParser.CanParse(message)) return BakedSensationsParser.From(message);
            if (SequenceParser.CanParse(message)) return SequenceParser.From(message);
            if (SensationWithMusclesParser.CanParse(message)) return SensationWithMusclesParser.From(message);

            return MicrosensationsParser.From(message);
        }
    }
}