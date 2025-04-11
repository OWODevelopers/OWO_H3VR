using System.Linq;

namespace OWOGame
{
    internal static class SequenceParser
    {
        public const char SEPARATOR = '&';

        public static bool CanParse(string message) => message.Contains(SEPARATOR);

        public static Sensation From(string message)
        {
            var messages = message.Split(SEPARATOR);
            var sensations = messages.Select(s => (Sensation)s).ToArray();

            return new SensationsSequence(sensations);
        }
    }
}