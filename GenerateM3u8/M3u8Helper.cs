using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace GenerateM3u8
{
    public class M3u8Helper
    {
        private string _videoInPut = "C:\\Users\\Administrator\\Desktop\\Video\\input\\30425_20230601062239.mp4";
        // FFmpe 文件路径_
        private string _ffmpegPath = "D:\\图片\\ffmpeg-master-latest-win64-gpl\\ffmpeg-master-latest-win64-gpl\\bin\\ffmpeg.exe";
        //m3u8 文件和ts文件
        private string _m3u8OutPut = "C:\\Users\\Administrator\\Desktop\\Video\\m3u8\\demo.m3u8";

        private int _segmentDuration = 3;

        private Action _noComplete;

        private Action<string> _noError;

        public static Queue<M3u8Helper> Queue = new Queue<M3u8Helper>();

        /// <summary>
        /// 使用队列
        /// </summary> 
        /// <param name="videoInPut">选择视频路径</param>
        /// <param name="ffmpegPath">FFmpe 文件路径(绝对路径)</param>
        /// <param name="m3u8OutPut"> m3u8保存路径</param>
        /// <param name="noComplete"> </param>
        /// <param name="noError"> </param>
        /// <param name="segmentDuration"> 视频分片时长（秒）</param>
        public M3u8Helper(string videoInPut, string ffmpegPath, string m3u8OutPut, Action noComplete, Action<string> noError = null, int segmentDuration = 3)
        {
            _videoInPut = videoInPut;

            _ffmpegPath = ffmpegPath;
            _m3u8OutPut = m3u8OutPut;
            _segmentDuration = segmentDuration;
            _noComplete = noComplete;
            _noError = noError;
        }

        /// <summary>
        ///  不使用队列
        /// </summary> 
        /// <param name="videoInPut">选择视频路径</param>
        /// <param name="ffmpegPath">FFmpe 文件路径(绝对路径)</param>
        /// <param name="m3u8OutPut"> m3u8保存路径</param>
        /// <param name="noComplete"> </param>
        /// <param name="noError"> </param>
        /// <param name="segmentDuration"> 视频分片时长（秒）</param>
        public M3u8Helper(string videoInPut, string ffmpegPath, string m3u8OutPut, int segmentDuration = 3)
        {
            _videoInPut = videoInPut;

            _ffmpegPath = ffmpegPath;
            _m3u8OutPut = m3u8OutPut;
            _segmentDuration = segmentDuration;
            _noComplete = () => Console.WriteLine("Complete");
            _noError = (info) => Console.WriteLine("Error:" + info);

        }

        /// <summary>
        /// 启用队列
        /// </summary>
        /// <returns></returns>
        public static Task Run()
        {
            return Task.Run(() =>
            {
                while (true)
                {
                    if (Queue.Count > 0)
                        Queue.Dequeue().Generate();
                }
            });
        }

        /// <summary>
        /// 生成
        /// </summary>
        public void Generate()
        {
            try
            {
                // 降低压缩比提升速度：-preset ultrafast
                // 降低画质： -vf "scale=640:480" -b:v 1000k
                string arguments = $"-i {_videoInPut} -profile:v baseline -level 2.0 -threads 5  -vf \"scale=640:480\" -b:v 1000k  -start_number 0 -hls_time {_segmentDuration} -hls_list_size 0 -f hls {_m3u8OutPut}";
                Process process = new Process();
                process.StartInfo.FileName = _ffmpegPath;
                process.StartInfo.Arguments = arguments;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;

                // 为进程的输出和错误流附加事件处理程序
                process.OutputDataReceived += Process_OutputDataReceived;
                process.ErrorDataReceived += Process_ErrorDataReceived;

                // 启动进程
                process.Start();

                // 异步读取输出和错误流
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                // 等待进程完成
                process.WaitForExit();
                if (!System.IO.File.Exists(_m3u8OutPut))
                    throw new Exception("m3u8文件生成失败");
                _noComplete();
            }
            catch (Exception ex)
            {
                _noError(ex.Message);
            }
        }

        private static void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                Console.WriteLine(e.Data);
            }
        }

        private static void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                Console.WriteLine(e.Data);
            }
        }
    }
}