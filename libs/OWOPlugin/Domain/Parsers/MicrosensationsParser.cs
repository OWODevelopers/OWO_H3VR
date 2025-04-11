namespace OWOGame
{
    internal static class MicrosensationsParser
    {
        public const char SEPARATOR = ',';

        public static MicroSensation From(string message)
        {
            var parameters = message.Split(SEPARATOR);

            return new MicroSensation(int.Parse(parameters[0]),
                                      float.Parse(parameters[1]) / 10,
                                      int.Parse(parameters[2]),
                                      float.Parse(parameters[3]) / 1000,
                                      float.Parse(parameters[4]) / 1000,
                                      float.Parse(parameters[5]) / 10,
                                      NameFrom(parameters));
        }

        static string NameFrom(string[] parameters) => parameters.Length >= 7 ? parameters[6] : "";
    }
}