using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ZPC.Phone.Draw.Serialize
{
    [XmlRoot("Geometries")]
    public sealed class DrawGeometrySerializer
    {
        #region 属性

        [XmlArrayItem(typeof(DrawPenSerializer)),
         //XmlArrayItem(typeof(DrawRangingSerializer)),
         //XmlArrayItem(typeof(DrawLineSerializer)),
         //XmlArrayItem(typeof(DrawArrowSerializer)),
         //XmlArrayItem(typeof(DrawRectangleSerializer)),
         //XmlArrayItem(typeof(DrawEllipseSerializer)),
         //XmlArrayItem(typeof(DrawAngleSerializer)),
         //XmlArrayItem(typeof(DrawPolylineSerializer)),
         //XmlArrayItem(typeof(DrawCurveSerializer)),
         //XmlArrayItem(typeof(DrawPolygonSerializer)),
         //XmlArrayItem(typeof(DrawClosedCurveSerializer)),
         //XmlArrayItem(typeof(DrawAreaSerializer)),
         XmlArrayItem(typeof(DrawTextSerializer))]
        public DrawGeometrySerializerBase[] Geometries { get; set; }

        #endregion
    }
}
