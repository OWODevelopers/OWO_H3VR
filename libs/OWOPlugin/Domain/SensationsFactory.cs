namespace OWOGame
{
    public static class SensationsFactory
    {
        public static MicroSensation Create(int frequency = 100, float durationSeconds = 0.1f, int intensityPercentage = 100,
                                            float rampUpMillis = 0, float rampDownMillis = 0, float exitDelaySeconds = 0)
        {
            return new MicroSensation(frequency, durationSeconds, intensityPercentage, rampUpMillis, rampDownMillis, exitDelaySeconds);
        }
    }
}