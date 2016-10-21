using GI.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GI.Functions
{
    /// <summary>
    /// 功率谱分析
    /// </summary>
    class PSD
    {
        #region 属性 各种路径
        /// <summary>
        /// exe路径
        /// </summary>
        public static string exePath = @"Utils\PSD_F.exe";
        /// <summary>
        /// parameters.inp临时文件路径
        /// </summary>
        public static string tcPath = @"parameters.inp";
        /// <summary>
        /// 输入文件(*.grd)路径
        /// </summary>
        public static string inPath = @"in.grd";
        /// <summary>
        /// 输出文件(*.dat|*.txt)路径
        /// </summary>
        public static string outPath = @"out.dat";
        /// <summary>
        /// 任务控制
        /// </summary>
        public static Process p = null;
        #endregion

        /// <summary>
        /// 功率谱分析
        /// </summary>
        /// <param name="input">输入文件路径</param>
        /// <returns></returns>
        public static Task<string> Start(string input)
        {
            //如果输出文件不存在则自动创建输出文件
            if (!File.Exists(outPath))
                File.Create(outPath).Dispose();
            // 构造parameters.inp内容
            string tc = String.Format("{0}\n{1}", inPath, outPath);
            // 写入parameters.inp
            using (var writer = new StreamWriter(tcPath, false, Encoding.GetEncoding("GB2312")))
            {
                writer.Write(tc);
            }
            // 执行exe
            return Task.Factory.StartNew<string>(() =>
            {
                // 输入文件全部存入临时文件夹
                File.Copy(input, inPath, true);
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
