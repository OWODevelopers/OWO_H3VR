namespace OWOGame
{
    public abstract partial class Sensation
    {
        /// <summary>
        /// SensationsFactory.Create(100, .1f)
        /// </summary>
        public static Sensation Ball => SensationsFactory.Create(100, .1f);

        /// <summary>
        /// SensationsFactory.Create(10, .1f)
        /// </summary>
        public static Sensation Dart => SensationsFactory.Create(10, .1f);

        /// <summary>
        /// Sensation.DaggerEntry.Append(Sensation.DaggerMovement)
        /// </summary>
        public static Sensation Dagger => Sensation.DaggerEntry.Append(Sensation.DaggerMovement);

        /// <summary>
        /// SensationsFactory.Create(60, .2f)
        /// </summary>
        public static Sensation DaggerEntry => SensationsFactory.Create(60, .2f);

        /// <summary>
        /// SensationsFactory.Create(100, 2, 100, .3f, .1f)
        /// </summary>
        public static Sensation DaggerMovement
            => SensationsFactory.Create(100, 2, 100, .3f, .1f);

        /// <summary>
        /// Sensation.ShotEntry.Append(Sensation.ShotExit).Append(Sensation.ShotBleeding);
        /// </summary>
        public static Sensation ShotWithExit
            => Sensation.ShotEntry.Append(Sensation.ShotExit).Append(Sensation.ShotBleeding);

        /// <summary>
        /// SensationsFactory.Create(30, .1f).WithMuscles(Muscle.Pectoral_R)
        /// </summary>
        public static Sensation ShotEntry => SensationsFactory.Create(30, .1f).WithMuscles(Muscle.Pectoral_R);

        /// <summary>
        /// SensationsFactory.Create(20, .1f).WithMuscles(Muscle.Dorsal_R)
        /// </summary>
        public static Sensation ShotExit => SensationsFactory.Create(20, .1f).WithMuscles(Muscle.Dorsal_R);

        /// <summary>
        /// SensationsFactory.Create(50, .5f, 80, 0, .3f).WithMuscles(Muscle.Pectoral_R, Muscle.Pectoral_L)
        /// </summary>
        public static Sensation ShotBleeding
            => SensationsFactory.Create(50, .5f, 80, 0, .3f)
                .WithMuscles(Muscle.Pectoral_R, Muscle.Pectoral_L);
    }
}