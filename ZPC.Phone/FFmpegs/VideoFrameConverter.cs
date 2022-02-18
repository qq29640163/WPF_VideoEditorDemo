using FFmpeg.AutoGen;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ZPC.Phone.FFmpegs
{
    public sealed unsafe class VideoFrameConverter : IDisposable
    {
        private readonly IntPtr _convertedFrameBufferPtr;
        private readonly Size _destinationSize;
        private readonly byte_ptrArray4 _dstData;
        private readonly int_array4 _dstLinesize;
        private readonly SwsContext* _pConvertContext;
        /// <summary>
        /// 帧格式转换
        /// </summary>
        /// <param name="sourceSize"></param>
        /// <param name="sourcePixelFormat"></param>
        /// <param name="destinationSize"></param>
        /// <param name="destinationPixelFormat"></param>
        public VideoFrameConverter(Size sourceSize, AVPixelFormat sourcePixelFormat,
            Size destinationSize, AVPixelFormat destinationPixelFormat)
        {
            _destinationSize = destinationSize;
            //分配并返回一个SwsContext。您需要它使用sws_scale()执行伸缩/转换操作
            //主要就是使用SwsContext进行转换！！！
            _pConvertContext = ffmpeg.sws_getContext(sourceSize.Width,
                sourceSize.Height,
                sourcePixelFormat,
                destinationSize.Width,
                destinationSize.Height,
                destinationPixelFormat,
                ffmpeg.SWS_FAST_BILINEAR,//默认算法 还有其他算法
                null,
                null,
                null//额外参数 在flasgs指定的算法，而使用的参数。如果  SWS_BICUBIC  SWS_GAUSS  SWS_LANCZOS这些算法。  这里没有使用
                );
            if (_pConvertContext == null)
                throw new ApplicationException("Could not initialize the conversion context.");
            //获取媒体帧所需要的大小
            var convertedFrameBufferSize = ffmpeg.av_image_get_buffer_size(destinationPixelFormat,
                destinationSize.Width,
                destinationSize.Height,
                1);
            //申请非托管内存,unsafe代码
            _convertedFrameBufferPtr = Marshal.AllocHGlobal(convertedFrameBufferSize);
            //转换帧的内存指针
            _dstData = new byte_ptrArray4();
            _dstLinesize = new int_array4();

            //挂在帧数据的内存区把_dstData里存的的指针指向_convertedFrameBufferPtr
            ffmpeg.av_image_fill_arrays(ref _dstData,
                ref _dstLinesize,
                (byte*)_convertedFrameBufferPtr,
                destinationPixelFormat,
                destinationSize.Width,
                destinationSize.Height,
                1);
        }

        public void Dispose()
        {
            Marshal.FreeHGlobal(_convertedFrameBufferPtr);
            ffmpeg.sws_freeContext(_pConvertContext);
        }

        public AVFrame Convert(AVFrame sourceFrame)
        {
            //转换格式
            ffmpeg.sws_scale(_pConvertContext,
                sourceFrame.data,
                sourceFrame.linesize,
                0,
                sourceFrame.height,
                _dstData,
                _dstLinesize);

            var data = new byte_ptrArray8();
            data.UpdateFrom(_dstData);
            var linesize = new int_array8();
            linesize.UpdateFrom(_dstLinesize);

            return new AVFrame
            {
                data = data,
                linesize = linesize,
                width = _destinationSize.Width,
                height = _destinationSize.Height
            };
        }
    }
}
