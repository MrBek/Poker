namespace Poker.Client.Audio
{
    public sealed class AudioInputDeviceDescription
    {
        private readonly string name;
        private readonly int    minFrequency;
        private readonly int    maxFrequency;

        public string Name
        {
            get { return name; }
        }

        public int MinFrequency
        {
            get { return minFrequency; }
        }

        public int MaxFrequency
        {
            get { return maxFrequency; }
        }

        public override string ToString()
        {
            return string.Format("{0}, MinFreq: {1}, MaxFreq: {2}", name, minFrequency, maxFrequency);
        }

        internal AudioInputDeviceDescription(string name,int minFrequency,int maxFrequency)
        {
            this.name           = name;
            this.minFrequency   = minFrequency;
            this.maxFrequency   = maxFrequency;
        }
    }
}