namespace spectator.Sources
{
    public class Sample
    {
        public Sample(string instance, double value)
        {
            Instance = instance;
            Value = value;
        }

        public string Instance { get; private set; }

        public double Value { get; private set; }
    }
}
