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
    class FLYBHF
    {
        #region 属性 各种路径
        /// <summary>
        /// exe路径
        /// </summary>
        public static string exePath = @"Utils\TC_FFT_F.exe";
        /// <summary>
        /// TC.inp临时文件路径
        /// </summary>
        public static string tcPath = @"parameter.inp";
        /// <summary>
        /// 内区地形数据文件(*.grd)路径
        /// </summary>
        public static string srtm30GrdPath = @"srtm30.grd";
        /// <summary>
        /// 外区地形数据文件(*.grd)路径
        /// </summary>
        public static string srtm60GrdPath = @"srtm60.grd";
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
        /// 执行地形改正傅里叶变换法exe
        /// </summary>
        /// <param name="srtm30">内区地形数据文件(*.grd)</param>
        /// <param name="srtm60">外区地形数据文件(*.grd)</param>
        /// <param name="density">密度</param>
        /// <param name="innerRadius">内区半径</param>
        /// <param name="outterRadius">外区半径</param>
        /// <returns>异步执行Task，返回值为结果</returns>
        /// 
        public static Task<string> Start(string srtm30, string srtm60, double density, double innerRadius, double outterRadius)
        {
            //判断传入文件是否存在
            if (!File.Exists(srtm30))
                throw new Exception("内区地形数据文件不存在！");
            if (!File.Exists(srtm60))
                throw new Exception("外区地形数据文件不存在！");
            //如果输出文件不存在则自动创建输出文件
            if (!File.Exists(outPath))
                File.Create(outPath).Dispose();
            // 读取内外区地形数据文件，获取参数
            string NxNy_inner, NxNy_outter;
            using (var reader = new StreamReader(srtm30))
            {
                reader.ReadLine();
                NxNy_inner = reader.ReadLine();
            }
            using (var reader = new StreamReader(srtm60))
            {
                reader.ReadLine();
                NxNy_outter = reader.ReadLine();
            }
            // 构造parameter.inp内容
            string tc = String.Format("{0}\n{1}\n{2}\n2 f {3} {4}\n0 0 {5}\n0 0 {6}\n{7}", srtm30GrdPath, srtm60GrdPath, outPath, innerRadius, outterRadius, NxNy_inner, NxNy_outter, density);
            // MessageBox.Show(tc);
            // 写入TC.inp
            using (var writer = new StreamWriter(tcPath, false, Encoding.GetEncoding("GB2312")))
            {
                writer.Write(tc);
            }
            // 执行exe
            return Task.Factory.StartNew<string>(() =>
            {
                // 输入文件全部存入临时文件夹
                File.Copy(srtm30, srtm30GrdPath, true);
                File.Copy(srtm60, srtm60GrdPath, true);
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
                    if (File.Exists(tcPath))
                        File.Delete(tcPath);
                    if (File.Exists(srtm30GrdPath))
                        File.Delete(srtm30GrdPath);
                    if (File.Exists(srtm60GrdPath))
                        File.Delete(srtm60GrdPath);
                }
                return msg;
            });
        }
    }
}
