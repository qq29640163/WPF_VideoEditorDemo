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
    public sealed unsafe class VideoStreamDecoder : IDisposable
    {
        //视频编码上下文结构体
        private readonly AVCodecContext* _pCodecContext;
        //视频封装上下文结构体
        private readonly AVFormatContext* _pFormatContext;
        private readonly AVFrame* _pFrame;
        private readonly AVPacket* _pPacket;
        private readonly AVFrame* _receivedFrame;
        private readonly int _streamIndex;

        public string CodecName { get; }
        public Size FrameSize { get; }
        public AVPixelFormat PixelFormat { get; }

        /// <summary>
        /// 视频解码器
        /// </summary>
        /// <param name="url">视频流URL</param>
        /// <param name="HWDeviceType">硬件解码器类型(默认AVHWDeviceType.AV_HWDEVICE_TYPE_NONE)</param>
        public VideoStreamDecoder(string url, AVHWDeviceType HWDeviceType = AVHWDeviceType.AV_HWDEVICE_TYPE_NONE)
        {
            //分配一个AVFormatContext
            _pFormatContext = ffmpeg.avformat_alloc_context();
            //分配一个AVFrame
            _receivedFrame = ffmpeg.av_frame_alloc();
            var pFormatContext = _pFormatContext;
            //将源音视频流传递给ffmpeg即ffmpeg打开源视频
            ffmpeg.avformat_open_input(&pFormatContext, url, null, null).ThrowExceptionIfError();
            //获取音视频流信息
            ffmpeg.avformat_find_stream_info(_pFormatContext, null).ThrowExceptionIfError();
            AVCodec* codec = null;
            //在源里找到最佳的流，如果指定了解码器，则根据解码器寻找流，将解码器传递给codec
            _streamIndex = ffmpeg
                .av_find_best_stream(_pFormatContext, AVMediaType.AVMEDIA_TYPE_VIDEO, -1, -1, &codec, 0)
                .ThrowExceptionIfError();
            //根据解码器分配一个AVCodecContext ，仅仅分配工具，还没有初始化。
            _pCodecContext = ffmpeg.avcodec_alloc_context3(codec);
            //如果硬解码
            if (HWDeviceType != AVHWDeviceType.AV_HWDEVICE_TYPE_NONE)
            {
                //根据硬件编码类型创建AVHWDeviceContext，存在AVFormatContext.hw_device_ctx (_pCodecContext->hw_device_ctx)
                ffmpeg.av_hwdevice_ctx_create(&_pCodecContext->hw_device_ctx, HWDeviceType, null, null, 0)
                    .ThrowExceptionIfError();
            }
            //将最佳流的格式参数传递给codecContext
            ffmpeg.avcodec_parameters_to_context(_pCodecContext, _pFormatContext->streams[_streamIndex]->codecpar)
                .ThrowExceptionIfError();
            //根据codec初始化pCodecContext 。与_pCodecContext = ffmpeg.avcodec_alloc_context3(codec);对应
            ffmpeg.avcodec_open2(_pCodecContext, codec, null).ThrowExceptionIfError();

            CodecName = ffmpeg.avcodec_get_name(codec->id);
            FrameSize = new Size(_pCodecContext->width, _pCodecContext->height);
            PixelFormat = _pCodecContext->pix_fmt;
            //分配AVPacket
            /* AVPacket用于存储压缩的数据，分别包括有音频压缩数据，视频压缩数据和字幕压缩数据。
             它通常在解复用操作后存储压缩数据，然后作为输入传给解码器。或者由编码器输出然后传递给复用器。
             对于视频压缩数据，一个AVPacket通常包括一个视频帧。对于音频压缩数据，可能包括几个压缩的音频帧。
             */
            _pPacket = ffmpeg.av_packet_alloc();
            _pFrame = ffmpeg.av_frame_alloc();
        }

        public void Dispose()
        {
            var pFrame = _pFrame;
            ffmpeg.av_frame_free(&pFrame);

            var pPacket = _pPacket;
            ffmpeg.av_packet_free(&pPacket);

            ffmpeg.avcodec_close(_pCodecContext);
            var pFormatContext = _pFormatContext;
            ffmpeg.avformat_close_input(&pFormatContext);
        }


        /// <summary>
        /// 解码下一帧帧
        /// </summary>
        /// <param name="frame">参数返回解码后的帧</param>
        /// <returns></returns>
        public bool TryDecodeNextFrame(out AVFrame frame)
        {
            //取消帧的引用。帧将不会被任何资源引用
            ffmpeg.av_frame_unref(_pFrame);
            ffmpeg.av_frame_unref(_receivedFrame);
            int error;

            do
            {
                try
                {
                    #region 读取帧忽略无效帧
                    do
                    {
                        
                        ffmpeg.av_packet_unref(_pPacket);
                        //读取无效帧,根据pFormatContext读取帧，返回到Packet中
                        error = ffmpeg.av_read_frame(_pFormatContext, _pPacket);

                        if (error == ffmpeg.AVERROR_EOF)//根据pFormatContext读取帧，返回到Packet中
                        {
                            frame = *_pFrame;
                            return false;
                        }
                        //数值是负数是错误信息
                        error.ThrowExceptionIfError();
                    } while (_pPacket->stream_index != _streamIndex);//忽略掉音视频流里面与有效流(初始化(构造函数)时标记的_streamIndex)不一致的流
                    #endregion
                    //将帧数据放入解码器
                    ffmpeg.avcodec_send_packet(_pCodecContext, _pPacket).ThrowExceptionIfError();
                }
                finally
                {
                    //消除对_pPacket的引用
                    ffmpeg.av_packet_unref(_pPacket);
                }
                //读取解码器里解码(_pCodecContext)后的帧通过参数返回(_pFrame)
                error = ffmpeg.avcodec_receive_frame(_pCodecContext, _pFrame);
            } while (error == ffmpeg.AVERROR(ffmpeg.EAGAIN));//当返回值等于 EAGAIN(再试一次)，认为读取帧结束

            error.ThrowExceptionIfError();
            //如果配置了硬件解码则调用硬件解码器解码
            if (_pCodecContext->hw_device_ctx != null)
            {
                //将_pFrame通过硬件解码后放入_receivedFrame
                ffmpeg.av_hwframe_transfer_data(_receivedFrame, _pFrame, 0).ThrowExceptionIfError();
                frame = *_receivedFrame;
            }
            else
                frame = *_pFrame;

            return true;
        }

        /// <summary>
        /// 获取媒体TAG信息
        /// </summary>
        /// <returns></returns>
        public IReadOnlyDictionary<string, string> GetContextInfo()
        {
            AVDictionaryEntry* tag = null;
            var result = new Dictionary<string, string>();

            while ((tag = ffmpeg.av_dict_get(_pFormatContext->metadata, "", tag, ffmpeg.AV_DICT_IGNORE_SUFFIX)) != null)
            {
                var key = Marshal.PtrToStringAnsi((IntPtr)tag->key);
                var value = Marshal.PtrToStringAnsi((IntPtr)tag->value);
                result.Add(key, value);
            }

            return result;
        }
    }
}
