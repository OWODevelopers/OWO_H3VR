namespace OWOGame
{
    internal static class BakedSensationsBuilder
    {
        const string SEPARATOR = "~";

        public static string From(BakedSensation sensation) => sensation.id.ToString();
        public static string Stringify(BakedSensation sensation)
        {
            return sensation.id + SEPARATOR +
                   sensation.name + SEPARATOR +
                   sensation.reference + SEPARATOR +
                   sensation.icon + SEPARATOR +
                   sensation.Family;
        }
    }
}