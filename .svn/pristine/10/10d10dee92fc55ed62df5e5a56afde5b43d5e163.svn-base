using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ZPC.Phone.Draw.Tool;

namespace ZPC.Phone.Draw.Serialize
{
    public sealed class DrawTextSerializer : DrawGeometrySerializerBase
    {
        public override DrawGeometryBase Deserialize(DrawingCanvas drawingCanvas)
        {
            var draw = new TextDrawTool(drawingCanvas);
            draw.DeserializeFrom(this);
            return draw;
        }

        #region 属性

        public Point StartPoint { get; set; }

        public String Text { get; set; }

        public Double Width { get; set; }

        public Double Height { get; set; }

        #endregion
    }
}
