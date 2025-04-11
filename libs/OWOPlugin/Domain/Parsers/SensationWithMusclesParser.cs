using System.Linq;

namespace OWOGame
{
    internal static class SensationWithMusclesParser
    {
        public const char SEPARATOR = '|';

        public static bool CanParse(string message) => message.Contains(SEPARATOR);

        public static SensationWithMuscles From(string message)
        {
            var parameters = message.Split(SEPARATOR);

            return new SensationWithMuscles(parameters[0],
                                            MusclesParser.Parse(parameters[1]));
        }
    }
}