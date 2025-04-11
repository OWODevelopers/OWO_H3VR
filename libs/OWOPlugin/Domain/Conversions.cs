namespace OWOGame
{
    public static class Conversions
    {
        public static Multiplier ToPercentage(this float howMuch) => (int)(howMuch * 100);
    }
}