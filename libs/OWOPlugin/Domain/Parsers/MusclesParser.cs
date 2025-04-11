using System.Linq;

namespace OWOGame
{
    internal static class MusclesParser
    {
        public static Muscle[] Parse(string message)
        {
            var muscles = message.Split(',');

            return muscles.Select(m => ParseSingle(m)).ToArray();
        }

        public static Muscle ParseSingle(string message)
        {
            var parameters = message.Split('%');

            return new Muscle(int.Parse(parameters[0]),
                              int.Parse(parameters[1]));
        }
    }
}