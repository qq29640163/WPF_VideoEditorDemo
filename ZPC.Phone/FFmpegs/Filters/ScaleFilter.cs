namespace ZPC.Phone
{
    class ScaleFilter : IFilter
    {
        public Resolution OutputResolution { get; set; }
        public string FilterName { get => "Resolution"; }
        public string InputFlag { get; set; }
        public string OutputFlag { get; set; }

        public ScaleFilter(Resolution outputResolution)
        {
            OutputResolution = outputResolution;
        }

        public string GetFilter()
        {
            return OutputResolution.HasValue() ? ((string.IsNullOrEmpty(InputFlag) ? "" : $"[{ InputFlag}]") + $"scale={OutputResolution.Width}:{OutputResolution.Height}" + (OutputFlag == "" ? "" : $"[{ OutputFlag}]")) : "";
        }

        public override string ToString()
        {
            return OutputResolution.ToString();
        }
    }
}
