using GI.Tools;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GI.Functions
{
    class MatchingFilter
    {
        #region 属性 各种路径
        /// <summary>
        /// exe路径
        /// </summary>
        public static string exePath = @"bin\MatchingFilter_F.exe";
        /// <summary>
        /// parameters.inp临时文件路径
        /// </summary>
        public static string tcPath = @"parameters.inp";
        /// <summary>
        /// 输入文件(*.grd)路径
        /// </summary>
        public static string inPath = @"in.grd";
        /// <summary>
        /// 输出文件(*.grd)路径
        /// </summary>
        public static string outPath = @"out.grd";
        /// <summary>
        /// 任务控制
        /// </summary>
        public static Process p = null;
        #endregion

        /// <summary>
        /// 匹配滤波
        /// </summary>
        /// <param name="input">输入文件路径</param>
        /// <param name="arg0">深源埋深</param>
        /// <param name="arg1">浅源埋深</param>
        /// <param name="arg2">纵轴截距</param>
        /// <param name="choice">区域场=0 局域场=1 </param>
        /// <returns></returns>
        public static Task<string> Start(string input, double arg0, double arg1, double arg2, int choice)
        {
            // 输入文件全部存入临时文件夹
            File.Copy(input, inPath, true);
            //如果输出文件不存在则自动创建输出文件
            if (!File.Exists(outPath))
                File.Create(outPath).Dispose();
            // 构造parameters.inp内容
            string tc = String.Format("{0}\n{1}\n{2} {3} {4} {5}", inPath, outPath, arg0, arg1, arg2, choice);
            // 写入parameters.inp
            using (var writer = new StreamWriter(tcPath, false, Encoding.GetEncoding("GB2312")))
            {
                writer.Write(tc);
            }
            // 执行exe
            return Task.Factory.StartNew<string>(() =>
            {
                string msg = "";
                try
                {
                    p = new Process();
                    ProcessStartInfo startInfo = new ProcessStartInfo(exePath, tcPath);
                    p.StartInfo = startInfo;
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.RedirectStandardOutput = true;
                    p.StartInfo.CreateNoWindow = true;
                    p.Start();
                    msg = p.StandardOutput.ReadToEnd();
                }
                catch
                {
                    MessageWindow.Show("找不到EXE！");
                }
                finally
                {
                    try
                    {
                        if (File.Exists(tcPath))
                            File.Delete(tcPath);
                        if (File.Exists(inPath))
                            File.Delete(inPath);
                    }
                    catch { };
                }
                return msg;
            });
        }
    }
}
