using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GI.Functions
{
    class RegularizationFilter
    {
        #region 属性 各种路径
        /// <summary>
        /// exe路径
        /// </summary>
        public static string exePath = @"RegularizationFilter_F.exe";
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
        /// 正则化滤波
        /// </summary>
        /// <param name="input">输入文件路径</param>
        /// <param name="choice">低通滤波=0 高通滤波=1 带通滤波=2</param>
        /// <returns></returns>
        public static Task<string> Start(string input, double a1, double b1, double f01, double a2, double b2, double f02, int choice)
        {
            //判断传入文件是否存在
            if (!File.Exists(input))
                throw new Exception("输入文件不存在！");
            //如果输出文件不存在则自动创建输出文件
            if (!File.Exists(outPath))
                File.Create(outPath).Dispose();
            // 输入文件全部存入临时文件夹
            File.Copy(input, inPath, true);
            // 构造parameters.inp内容
            string tc = String.Format("{0}\n{1}\n{2} {3} {4}\n{5} {6} {7}\n{8}", inPath, outPath, a1, b1, f01, a2, b2, f02, choice);
            // 写入parameters.inp
            using (var writer = new StreamWriter(tcPath, false, Encoding.GetEncoding("GB2312")))
            {
                writer.Write(tc);
            }
            // 执行exe
            return Task.Factory.StartNew<string>(() =>
            {
                try
                {
                    p = new Process();
                    ProcessStartInfo startInfo = new ProcessStartInfo(exePath, tcPath);
                    p.StartInfo = startInfo;
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.RedirectStandardOutput = true;
                    p.StartInfo.CreateNoWindow = true;
                    p.Start();
                    return p.StandardOutput.ReadToEnd();
                }
                catch
                {
                    MessageBox.Show("找不到EXE！");
                    return "";
                }
            });
        }
    }
}
