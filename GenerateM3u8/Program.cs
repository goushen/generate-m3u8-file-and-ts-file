using System;

namespace GenerateM3u8
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string _videoInPut = "C:\\Users\\Administrator\\Desktop\\Video\\input\\30425_20230601062239.mp4";
            // FFmpeg .exe文件路径 下载：https://github.com/BtbN/FFmpeg-Builds/releases/download/latest/ffmpeg-master-latest-win64-gpl.zip
            string _ffmpegPath = "D:\\项目\\生成m3u8\\generate-m3u8-file-and-ts-file\\ffmpeg\\bin\\ffmpeg.exe";
            //m3u8 文件和ts文件
            string _m3u8OutPut = "C:\\Users\\Administrator\\Desktop\\Video\\m3u8\\demo.m3u8";

            #region 队列方式 因为m3u8文件生成比较耗性能和时间

            //M3u8Helper.Run();
            //M3u8Helper.Queue.Enqueue(
            //        new M3u8Helper(
            //            _videoInPut,
            //            _ffmpegPath,
            //            _m3u8OutPut,
            //                 () =>
            //                {
            //                    Console.WriteLine("Complete");
            //                },
            //                (ex) =>
            //                {
            //                    Console.WriteLine("Error");
            //                }));
            #endregion


            #region 直接使用

            new M3u8Helper(_videoInPut,
                        _ffmpegPath,
                        _m3u8OutPut).Generate();
            #endregion
        }
    }
}
