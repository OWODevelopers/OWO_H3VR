namespace OWOGame
{
    public struct Multiplier
    {
        public readonly int value;

        Multiplier(int value)
        {
            this.value = Math.Clamp(value, 0, value);
        }
        
        public static Multiplier operator *(Multiplier theFirst, int theSecond)
        {
            return new Multiplier((theFirst.value * theSecond) / 100);
        }

        public static implicit operator Multiplier(int howmuch)
        {
            return new Multiplier(howmuch);
        }

        public static implicit operator int(Multiplier howmuch)
        {
            return howmuch.value;
        }
    }
}