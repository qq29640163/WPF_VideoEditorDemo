using System.Collections.Generic;
using System.Text;
using System.Linq;


namespace ZPC.Phone
{
    public interface IFilter
    {
        string FilterName { get; }

        /// <summary>
        /// 输入标记别名
        /// </summary>
        public string InputFlag { get; set; }

        /// <summary>
        /// 输出标记别名
        /// </summary>
        public string OutputFlag { get; set; }

        string GetFilter();
    }


    class Filtergraph
    {
        /// <summary>
        /// 返回添加的滤镜filter的总数
        /// </summary>
        public int Count { get => filters.Values.Sum(x => x.Count); }

        readonly Dictionary<(int inputIndex, int streamIndex), List<IFilter>> filters;


        public Filtergraph()
        {
            filters = new Dictionary<(int inputIndex, int streamIndex), List<IFilter>>();
        }

        public void AddFilter(IFilter filter, int inputIndex, int streamIndex)
        {
            if (filters.ContainsKey((inputIndex, streamIndex)))
            {
                filters[(inputIndex, streamIndex)].Add(filter);
            }
            else
            {
                filters.Add((inputIndex, streamIndex), new List<IFilter>());
                filters[(inputIndex, streamIndex)].Add(filter);
            }
        }

        public void AddFilters(IEnumerable<IFilter> filters, int inputIndex, int streamIndex)
        {
            if (this.filters.ContainsKey((0, streamIndex)))
            {
                this.filters[(0, streamIndex)].AddRange(filters);
            }
            else
            {
                this.filters.Add((inputIndex, streamIndex), new List<IFilter>());
                this.filters[(inputIndex, streamIndex)].AddRange(filters);
            }
        }

        public void ClearFilters()
        {
            this.filters.Clear();
        }

        public string GenerateFiltergraph()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var filterGroup in filters)
            {
                sb.Append($"[{filterGroup.Key.inputIndex}:{filterGroup.Key.streamIndex}]");
                sb.Append(GenerateFilterchain(filterGroup.Value));
                sb.Append(';');
            }
            sb.Remove(sb.Length - 1, 1);

            return sb.ToString();
        }

        private string GenerateFilterchain(List<IFilter> filters)
        {
            return string.Join(",", filters.Select(f => f.GetFilter()));
        }
    }
}