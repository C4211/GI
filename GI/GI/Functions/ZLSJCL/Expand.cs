using GI.Tools;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GI.Functions
{
    class Expand
    {
        #region 属性 各种路径
        /// <summary>
        /// exe路径
        /// </summary>
        public static string exePath = @"bin\Expand_F.exe";
        /// <summary>
        /// parameter.inp临时文件路径
        /// </summary>
        public static string tcPath = @"parameters.inp";

        private static string inputOrigin;
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
        /// 校验输入文件
        /// </summary>
        /// <param name="input">输入文件路径</param>
        /// <returns>文件存在且合法:new int[]{Nx_input, Ny_input, Nx_output, Ny_output}</returns>
        public static int[] Init(string input)
        {
            inputOrigin = input;
            if (!File.Exists(input))
                throw new Exception("输入文件不存在！");
            int[] data = FileNameFilter.CheckGRDFileFormat(input);
            if (data == null)
                throw new Exception("输入文件不是GRD数据格式！");
            return data;
        }

        public static Task<string> Start(int Nx_output, int Ny_output)
        {
            //如果输出文件不存在则自动创建输出文件
            if (!File.Exists(outPath))
                File.Create(outPath).Dispose();
            // 构造parameter.inp内容
            string tc = string.Format("{0}\n{1}\n{2} {3}", inPath, outPath, Nx_output, Ny_output);
            // 写入parameter.inp
            using (var writer = new StreamWriter(tcPath, false, Encoding.GetEncoding("GB2312")))
            {
                writer.Write(tc);
            }
            // 执行exe
            return Task.Factory.StartNew<string>(() =>
            {
                // 输入文件存入临时文件夹
                File.Copy(inputOrigin, inPath, true);
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
