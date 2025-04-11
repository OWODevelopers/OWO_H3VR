using System.Linq;

namespace OWOGame
{
    internal static class BakedSensationsParser
    {
        const char SEPARATOR = '~';

        public static bool CanParse(string message)
        {
            return (!message.Contains(MicrosensationsParser.SEPARATOR) && 
                    !message.Contains(SensationWithMusclesParser.SEPARATOR)) ||
                    message.Contains(SEPARATOR) ;
        }

        public static BakedSensation From(string message)
        {
            if (!message.Contains(SEPARATOR))
            {
                return SensationsFactory.Create().Bake(int.Parse(message), "");
            }

            var parameters = message.Split(SEPARATOR);
            return new BakedSensation(int.Parse(parameters[0]), parameters[1], parameters[2], parameters[3], FamilyFrom(parameters));
        }

        static Family FamilyFrom(string[] parameters) => parameters.Length < 5 ? Family.None : (Family)parameters[4];
    }
}