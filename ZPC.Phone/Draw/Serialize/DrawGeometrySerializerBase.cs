using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ZPC.Phone.Draw.Serialize
{
    /// <summary>
    /// 画图几何图形序列化基类
    /// </summary>
    public abstract class DrawGeometrySerializerBase
    {
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <returns></returns>
        public abstract DrawGeometryBase Deserialize(DrawingCanvas drawingCanvas);

        #region 属性

        public Double StrokeThickness { get; set; }

        public Color Color { get; set; }

        public String Geometry { get; set; }

        public Matrix Matrix { get; set; }

        #endregion
    }
}
