using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZPC.Phone.Draw.Tool;

namespace ZPC.Phone.Draw.Serialize
{
    public sealed class DrawPenSerializer : DrawGeometrySerializerBase
    {
        public override DrawGeometryBase Deserialize(DrawingCanvas drawingCanvas)
        {
            var draw = new PenDrawTool(drawingCanvas);
            draw.DeserializeFrom(this);
            return draw;
        }

        #region 属性



        #endregion
    }
}
