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
    class VerticalDerivativeSpace
    {
        #region 属性 各种路径
        /// <summary>
        /// exe路径
        /// </summary>
        public static string exePath = @"Utils\VerticalDerivative_Space_F.exe";
        /// <summary>
        /// parameters.inp临时文件路径
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
        /// <returns>文件存在且合法:{dx, dy}</returns>
        public static double[] Init(string input)
        {
            inputOrigin = input;
            if (!File.Exists(input))
                throw new Exception("输入文件不存在！");
            using (var reader = new StreamReader(input))
            {
                string firstLine = reader.ReadLine();
                if (firstLine.Trim() != "DSAA")
                    throw new Exception("打开文件错误，不是GRD数据格式！\n请检查数据文件格式！");
                string[] strData, strData1, strData2;
                int Nx_input = 0, Ny_input = 0;
                double xmin, xmax, ymin, ymax;
                strData = reader.ReadLine().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                strData1 = reader.ReadLine().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                strData2 = reader.ReadLine().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (!int.TryParse(strData[0], out Nx_input) || !int.TryParse(strData[1], out Ny_input))
                    throw new Exception("GRD数据第2行参数格式错误！");
                if (!double.TryParse(strData1[0], out xmin) || !double.TryParse(strData1[1], out xmax))
                    throw new Exception("GRD数据第3行参数格式错误！");
                if (!double.TryParse(strData2[0], out ymin) || !double.TryParse(strData2[1], out ymax))
                    throw new Exception("GRD数据第3行参数格式错误！");
                double dx = (xmax - xmin) / (Nx_input - 1);
                double dy = (ymax - ymin) / (Ny_input - 1);
                return new double[] { dx, dy };
            }
        }

        /// <summary>
        /// 空间域垂向导数
        /// </summary>
        /// <param name="input">输入文件路径</param>
        /// <param name="order">一阶导数=1 二阶导数=2</param>
        /// <param name="choice">算法选择 Healck=0 ElkinsI=1 ElkinsII=2 ElkinsIII=3 Rosenbach=4</param>
        /// <returns></returns>
        public static Task<string> Start(int order, int choice)
        {
            //如果输出文件不存在则自动创建输出文件
            if (!File.Exists(outPath))
                File.Create(outPath).Dispose();
            // 构造parameters.inp内容
            string tc = String.Format("{0}\n{1}\n{2} {3}", inPath, outPath, order, choice);
            // 写入parameters.inp
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
