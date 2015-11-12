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
    class GeoForward
    {
        #region 属性 各种路径
        /// <summary>
        /// exe路径
        /// </summary>
        public static string exePath = @"bin\DZTZY.exe";
        /// <summary>
        /// 输出文件(*.txt)路径
        /// </summary>
        public static string outPath = @"out.txt";
        /// <summary>
        /// 任务控制
        /// </summary>
        public static Process p = null;
        #endregion

        /// <summary>
        /// 地质体正演
        /// </summary>
        /// <returns></returns>
        public static Task<string> Start(double[] modelArgs, double[] planeArgs, int choice)
        {
            //如果输出文件不存在则自动创建输出文件
            if (!File.Exists(outPath))
                File.Create(outPath).Dispose();
            // 构造参数内容
            StringBuilder sb = new StringBuilder();
            int n = 8;
            if (modelArgs[0] == 1)
                n = 6;
            for (int i = 0; i < n; i++)
            {
                sb.Append(modelArgs[i]);
                sb.Append(' ');
            }
            for (int i = 1; i < 8; i++)
            {
                sb.Append(planeArgs[i]);
                sb.Append(' ');
            }
            sb.AppendFormat("{0} {1}", choice, outPath);
            string tc = sb.ToString();
            // 执行exe
            return Task.Factory.StartNew<string>(() =>
            {
                try
                {
                    p = new Process();
                    ProcessStartInfo startInfo = new ProcessStartInfo(exePath, tc);
                    p.StartInfo = startInfo;
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.RedirectStandardOutput = true;
                    p.StartInfo.CreateNoWindow = true;
                    p.Start();
                    return p.StandardOutput.ReadToEnd();
                }
                catch
                {
                    MessageWindow.Show("找不到EXE！");
                    return "";
                }
            });
        }
    }
}
