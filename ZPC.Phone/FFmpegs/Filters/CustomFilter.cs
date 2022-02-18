namespace ZPC.Phone
{
    class CustomFilter : IFilter
    {
        public string FilterName { get; }
        public string InputFlag { get; set; }
        public string OutputFlag { get; set; }

        private string filter;

        public CustomFilter(string name, string filter)
        {
            FilterName = name;
            this.filter = filter;
        }

        public string GetFilter()
        {
            return filter;
        }
    }
}
