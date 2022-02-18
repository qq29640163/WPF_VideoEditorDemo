using Newtonsoft.Json.Linq;
using System;

namespace ZPC.Phone
{
    public class ColorInfo
    {
        public string PixelFormat { get; private set; }
        public string ColorSpace { get; private set; }
        public string ColorPrimaries { get; private set; }
        public string ColorTransfer { get; private set; }
        public MasteringDisplayMetadata DisplayMetadata { get; private set; }
        public (int maxContent, int maxAverage) LightLevelMetadata { get; private set; }

        public int ColorDepth => PixelFormat.Contains("10") ? 10 : 8;

        /*
        {
            "pix_fmt": "yuv420p10le",
            "color_space": "bt2020nc",
            "color_primaries": "bt2020",
            "color_transfer": "smpte2084",
            "side_data_list": [
                {
                    "side_data_type": "Mastering display metadata",
                    "red_x": "34000/50000",
                    "red_y": "16000/50000",
                    "green_x": "13250/50000",
                    "green_y": "34500/50000",
                    "blue_x": "7500/50000",
                    "blue_y": "3000/50000",
                    "white_point_x": "15635/50000",
                    "white_point_y": "16450/50000",
                    "min_luminance": "50/10000",
                    "max_luminance": "10000000/10000"
                },
                {
                    "side_data_type": "Content light level metadata",
                    "max_content": 0,
                    "max_average": 0
                }
            ]
        }
        */
        public static ColorInfo FromJson(JToken element)
        {
            ColorInfo colorInfo = new ColorInfo();
            string pix_fmt = element.Value<string>("pix_fmt");
            if (string.IsNullOrEmpty(pix_fmt))
                colorInfo.PixelFormat = pix_fmt;
            string color_space = element.Value<string>("color_space");
            if (string.IsNullOrEmpty(color_space))
                colorInfo.ColorSpace = color_space;

            string color_primaries = element.Value<string>("color_primaries");
            if (string.IsNullOrEmpty(color_primaries))
                colorInfo.ColorPrimaries = color_primaries;

            string color_transfer = element.Value<string>("color_transfer");
            if (string.IsNullOrEmpty(color_transfer))
                colorInfo.ColorTransfer = color_transfer;

            JToken side_data_list = element.Value<JToken>("side_data_list");
            if (string.IsNullOrEmpty(color_transfer))
            {
                foreach (var item in side_data_list.Children())
                {
                    string sideDataType = item.Value<string>("side_data_type");
                    if (sideDataType == "Mastering display metadata")
                    {
                        colorInfo.DisplayMetadata = MasteringDisplayMetadata.FromJson(item);
                    }
                    else if (sideDataType == "Content light level metadata")
                    {
                        colorInfo.LightLevelMetadata = (item.Value<int>("max_content"), item.Value<int>("max_average"));
                    }
                }
            }

            return colorInfo;
        }

        public override string ToString()
        {
            return $"Color space: {ColorSpace}\nColor primaries: {ColorPrimaries}\nColor transfer: {ColorTransfer}\nDisplay metadata: {DisplayMetadata}\nLight level metadata: (max content: {LightLevelMetadata.maxContent}, max average: {LightLevelMetadata.maxAverage})";
        }


        public class MasteringDisplayMetadata
        {
            public (int x, int y) Red;
            public (int x, int y) Green;
            public (int x, int y) Blue;
            public (int x, int y) WhitePoint;
            public (int min, int max) Luminance;

            /*
            {
                "side_data_type": "Mastering display metadata",
                "red_x": "34000/50000",
                "red_y": "16000/50000",
                "green_x": "13250/50000",
                "green_y": "34500/50000",
                "blue_x": "7500/50000",
                "blue_y": "3000/50000",
                "white_point_x": "15635/50000",
                "white_point_y": "16450/50000",
                "min_luminance": "50/10000",
                "max_luminance": "10000000/10000"
            }
            */
            public static MasteringDisplayMetadata FromJson(JToken jsonElement)
            {
                return new MasteringDisplayMetadata()
                {
                    Red = (GetValue(jsonElement.Value<string>("red_x")), GetValue(jsonElement.Value<string>("red_y"))),
                    Green = (GetValue(jsonElement.Value<string>("green_x")), GetValue(jsonElement.Value<string>("green_y"))),
                    Blue = (GetValue(jsonElement.Value<string>("blue_x")), GetValue(jsonElement.Value<string>("blue_y"))),
                    WhitePoint = (GetValue(jsonElement.Value<string>("white_point_x")), GetValue(jsonElement.Value<string>("white_point_y"))),
                    Luminance = (GetValue(jsonElement.Value<string>("min_luminance")), GetValue(jsonElement.Value<string>("max_luminance"))),
                };
            }

            static int GetValue(string value)
            {
                value = value.Substring(0, value.IndexOf('/'));
                return Convert.ToInt32(value);
            }
            public override string ToString()
            {
                // G(green_x,green_y)B(blue_x,blue_y)R(red_x,red_y)WP(white_x,white_y)L(max,min)
                return $"G({Green.x},{Green.y})B({Blue.x},{Blue.y})R({Red.x},{Red.y})WP({WhitePoint.x},{WhitePoint.y})L({Luminance.max},{Luminance.min})";
            }
        }
    }
}