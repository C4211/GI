using GI.Tools;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace GI.Functions
{
    class GeoBackward
    {
        #region 属性 各种路径
        /// <summary>
        /// exe路径
        /// </summary>
        public static string exePath = @"bin\DZTFY.exe";
        /// <summary>
        /// 重力异常数据文件(*.txt)路径
        /// </summary>
        public static string inPath1 = @"in1.txt";
        /// <summary>
        /// 重力梯度异常数据文件(*.txt)路径
        /// </summary>
        public static string inPath2 = @"in2.txt";
        /// <summary>
        /// 输出文件(*.txt)路径
        /// </summary>
        public static string outPath = @"out.txt";
        /// <summary>
        /// 任务控制
        /// </summary>
        public static Process p = null;
        #endregion

        public static Task<string> Start(int choice, string inFile1, string inFile2 = "", double arg1 = 0.0, double arg2 = 0.0)
        {
            File.Copy(inFile1, inPath1, true);
            if (inFile2 != "")
                File.Copy(inFile2, inPath2, true);
            //如果输出文件不存在则自动创建输出文件
            if (!File.Exists(outPath))
                File.Create(outPath).Dispose();
            // 构造参数内容
            string tc;
            if (choice == 0)
                tc = string.Format("{0} {1} {2} {3}", choice, inFile1, inPath1, outPath);
            else if (choice == 1)
                tc = string.Format("{0} {1} {2} {3}", choice, inFile1, inPath1, outPath);
            else if (choice == 2)
                tc = string.Format("{0} {1} {2} {3} {4} {5}", choice, inFile1, inPath1, inFile2, inPath2, outPath);
            else
                tc = string.Format("{0} {1} {2} {3} {4} {5} {6} {7}", choice, inFile1, inPath1, inFile2, inPath2, outPath, arg1, arg2);

            // 执行exe
            return Task.Factory.StartNew<string>(() =>
            {
                string msg = ""; 
                try
                {
                    p = new Process();
                    ProcessStartInfo startInfo = new ProcessStartInfo(exePath, tc);
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
                        if (File.Exists(inPath1))
                            File.Delete(inPath1);
                        if (File.Exists(inPath2))
                            File.Delete(inPath2);
                    }
                    catch { };
                }
                return msg;
            });
        }
    }
}
