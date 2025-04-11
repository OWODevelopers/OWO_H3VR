namespace OWOGame
{
    internal static class GamesBuilder
    {
        const string SEPARATOR = "#";

        public static string Build(GameAuth theGame)
        {
            if (theGame.sensations.Length <= 0) return string.Empty;

            var result = theGame.sensations[0].Stringify();

            for (int i = 1; i < theGame.sensations.Length; i++)
            {
                result += SEPARATOR + "\n"+ theGame.sensations[i].Stringify();
            }

            return result;
        }
    }
}