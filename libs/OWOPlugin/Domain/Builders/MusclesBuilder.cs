namespace OWOGame
{
    internal static class MusclesBuilder
    {
        public static string From(params Muscle[] muscles)
        {
            var result = From(muscles[0]);

            for (int i = 1; i < muscles.Length; i++)
            {
                result += $",{From(muscles[i])}";
            }

            return result;
        }

        private static string From(Muscle muscle)
        {
            return $"{muscle.id}%{muscle.intensity}";
        }
    }
}