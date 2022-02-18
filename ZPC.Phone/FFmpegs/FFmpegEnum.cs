using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZPC.Phone.FFmpegs
{
    public enum FFmpegEnum
    {
        /// <summary>
        /// 空解获取解码视频时长和其他信息
        /// </summary>
        Get_FullDecodeTime,
        /// <summary>
        /// 截取从视频文件的哪一秒开始截取内容，参数 int ss(从哪秒开始)
        /// </summary>
        Cut_FromSecond,
        /// <summary>
        /// 截取从视频文件的开始截取到哪一秒结束的内容，参数 int t(到哪秒截止)
        /// </summary>
        Cut_ToSecond,
        /// <summary>
        /// 截取从视频文件的某一秒开始到某一秒结束的内容，参数 int ss (从哪秒开始) 参数 int t(到哪秒截止)
        /// </summary>
        Cut_FromToSecond,
        /// <summary>
        /// 合并视频,添加视频路径，格式：路径1|路径2|路径3
        /// </summary>
        Merge_Video,
        /// <summary>
        /// 合并不同分辨率的视频，格式路径1|路径2|路径3，输出分辨率
        /// </summary>
        Merge_Video_FromDiffScale,
        /// <summary>
        /// 添加图片水印,参数1：水印文件url,参数2:图片宽，参数3：图片高，参数4：图片X坐标，参数5:图片Y坐标
        /// </summary>
        AddImageWatermark,
        /// <summary>
        /// 添加文字水印,参数1：字体大小,参数2:字体类型，参数3：文字内容，参数4：字体颜色，参数5:X坐标 参数6:Y坐标
        /// </summary>
        AddTextWatermark,
        /// <summary>
        /// 视频转gif,参数1:开始时间,参数2:结束时间,参数3:分辨率,参数4:帧率:
        /// </summary>
        ConvertGif,
        /// <summary>
        /// 模糊视频指定区域
        /// 参数1：裁剪区域的高
        /// 参数2：裁剪区域的宽
        /// 参数3：裁剪区域左上角X轴坐标
        /// 参数4：裁剪区域左上角Y轴坐标
        /// 参数5：模糊类型(avgblur,boxblur)，
        /// 参数6：模糊度，越高越模糊，
        /// 参数7：模糊区域覆盖原视频的X坐标位置(保持和参数3一致)
        /// 参数8：模糊区域覆盖原视频的Y坐标位置(保持和参数4一致)
        /// </summary>
        BlurDesignatedArea,
    }
}
