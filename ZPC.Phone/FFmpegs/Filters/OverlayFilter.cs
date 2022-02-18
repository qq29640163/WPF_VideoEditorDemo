using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZPC.Phone.FFmpegs.Filters
{
    public class OverlayFilter : IFilter
    {
        public string FilterName { get => "Overlay"; }
        
        /// <summary>
        /// 覆盖标记别名
        /// </summary>
        public string OverlayFlag { get; set; }

        /// <summary>
        /// 距主视频X轴位置
        /// </summary>
        public string X { get; set; }

        public string StartTime { get; set; }
        public string EndTime { get; set; }
        /// <summary>
        /// 距主视频Y轴位置
        /// </summary>
        public string Y { get; set; }

        public string InputFlag { get; set; }

        public string OutputFlag { get; set; }

        public string GetFilter()
        {
            string enble = string.Empty;
            if (!string.IsNullOrEmpty(StartTime) && !string.IsNullOrEmpty(EndTime))
            {
                enble = $":enable=\'between(t,{StartTime},{EndTime})\'";
            }
            string overlay = "overlay";
            overlay = (string.IsNullOrEmpty(InputFlag) ? "" : $"[{InputFlag}]") +
                (string.IsNullOrEmpty(OverlayFlag) ? "" : $"[{OverlayFlag}]") +
                overlay + $"={X}:{Y}" + enble +
                (string.IsNullOrEmpty(OutputFlag) ? "" : $"[{OutputFlag}]");
            return overlay;
        }
    }
}
