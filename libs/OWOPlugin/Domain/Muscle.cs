using System.Linq;

namespace OWOGame
{
    public readonly struct Muscle
    {
        public readonly int id;
        public readonly int intensity;

        internal Muscle(int id, int intensity = 100)
        {
            this.id = id;
            this.intensity = Math.Clamp(intensity,0, 100);
        }

        public Muscle WithIntensity(int intensity) => new Muscle(id, intensity);

        public static Muscle Pectoral_R = new Muscle(0);
        public static Muscle Pectoral_L = new Muscle(1);
        public static Muscle Abdominal_R = new Muscle(2);
        public static Muscle Abdominal_L = new Muscle(3);
        public static Muscle Arm_R = new Muscle(4);
        public static Muscle Arm_L = new Muscle(5);
        public static Muscle Dorsal_R = new Muscle(6);
        public static Muscle Dorsal_L = new Muscle(7);
        public static Muscle Lumbar_R = new Muscle(8);
        public static Muscle Lumbar_L = new Muscle(9);

        public static Muscle[] All => Front.Concat(Back).ToArray();

        public static Muscle[] Front => new[]{Pectoral_R, Pectoral_L,
                                              Abdominal_R, Abdominal_L,
                                              Arm_R, Arm_L};

        public static Muscle[] Back => new[] { Dorsal_R, Dorsal_L, Lumbar_R, Lumbar_L };

        public static Muscle[] Parse(string muscles) => MusclesParser.Parse(muscles);
    }
}