namespace ZPC.Phone
{
    class VolumeFilter : IFilter
    {
        public int Percentage { get; set; }
        public string FilterName { get => "Volume"; }
        public string InputFlag { get; set; }
        public string OutputFlag { get; set; }

        public VolumeFilter(int percentage)
        {
            Percentage = percentage;
        }

        public string GetFilter()
        {
            return $"volume={(Percentage / 100f):0.##}";
        }

        public override string ToString()
        {
            return $"{Percentage}%";
        }
    }
}
