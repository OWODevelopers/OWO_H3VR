using System.Linq;

namespace OWOGame
{
    internal static class GamesParser
    {
        public static GameAuth From(string auth)
        {
            var sensations = auth.Split('#');

            if (int.TryParse(auth, out var id)) return new GameAuth().WithId(auth);
            if (string.IsNullOrEmpty(sensations[0])) return new GameAuth();

            return new GameAuth("0", sensations.Select(s => BakedSensationsParser.From(s)).ToArray());
        }
    }
}