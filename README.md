# WPF_VideoEditorDemo
该项目是基于FFmpeg实现的简单的视频剪辑功能和Prism的MVVM模式应用，
仅基于学习和研究ffmpeg和Prism使用，
项目主要实现了Prism的导航跳转，MVVM前后分离,页面注册，
使用ffmpeg实现了视频的合并，拆分，水印添加，模糊，转GIF，以及部分滤镜特效的预览，
关于ffmpeg命令语法和API使用请去官网学习，代码中有部分基于官网的编解码的API示例可以参考，
在FFmpegHelper.cs文件的头部有部分滤镜的原理和命令教程链接，
API编解码的代码在VideoEditViewModel.cs 底部使用FFmpeg.AutoGen实现，
网上大部分关于ffmpeg的资料少之又少，希望我的代码能帮助部分想了解ffmpeg的，我也是个新手，共勉

The project is a simple video editing function based on ffmpeg and the MVVM mode application of prism

Based only on learning and research, ffmpeg and prism are used

The project mainly realizes the navigation jump of prism, the separation of MVVM from front to back, and page registration,

Ffmpeg is used to realize video merging, splitting, watermark addition, blur, GIF conversion, and preview of some filter effects,

For ffmpeg command syntax and API usage, please go to the official website to learn. Some API examples of encoding and decoding based on the official website can be referred to in the code

In ffmpeghelper The head of the CS file has links to the principles and commands of some filters

The API codec code is in videoeditviewmodel CS use ffmpeg at the bottom Autogen implementation

There is little information about ffmpeg on the Internet. I hope my code can help some people who want to understand ffmpeg. I'm also a novice. Let's encourage each other
