using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZPC.Phone.FFmpegs.Filters
{
    public class DrawtextFilter : IFilter
    {
        public string FilterName { get => "Drawtext"; }

        /// <summary>
        /// 字体文件
        /// </summary>
        public string Fontfile { get; set; }

        /// <summary>
        /// 文字
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 文字文件
        /// </summary>
        public string Textfile { get; set; }

        /// <summary>
        /// 用于使用背景颜色在文本周围绘制一个框。该值必须是 1（启用）或 0（禁用）。框的默认值为0。
        /// </summary>
        public string Box { get; set; }

        /// <summary>
        /// 用于在文本周围绘制框的颜色,默认值为“white”
        /// </summary>
        public string Boxcolor { get; set; }

        /// <summary>
        /// 字体颜色，格式：#556677FF,FF为a通道，前面的55 66 77 分别为rgb
        /// </summary>
        public string Fontcolor{ get; set; }

        /// <summary>
        /// 字体大小，整数
        /// </summary>
        public string Fontsize { get; set; }

        /// <summary>
        /// 字体名称，默认为Sans字体
        /// </summary>
        public string Font { get; set; }

        /// <summary>
        /// 距视频X轴的距离
        /// </summary>
        public string X { get; set; }

        /// <summary>
        /// 距视频Y轴的距离
        /// </summary>
        public string Y { get; set; }

        /// <summary>
        /// 滤镜输出标识别名
        /// </summary>
        public string OutputFlag { get; set; }
        public string InputFlag { get; set; }

        public string StartTime { get; set; }
        public string EndTime { get; set; }

        public string GetFilter()
        {
            string enble = string.Empty;
            if (!string.IsNullOrEmpty(StartTime) && !string.IsNullOrEmpty(EndTime))
            {
                enble = $":enable=\'between(t,{StartTime},{EndTime})\'";
            }
            string drawtext = (string.IsNullOrEmpty(InputFlag) ? "" : $"[{InputFlag}]") + "drawtext";
            drawtext = drawtext + $"=fontsize={Fontsize}:fontfile={Fontfile}:text='{Text}':fontcolor={Fontcolor}:x={X}:y={Y}" + enble
                + (string.IsNullOrEmpty(OutputFlag) ? "" : $"[{OutputFlag}]");
            return drawtext;
        }
    }
}
