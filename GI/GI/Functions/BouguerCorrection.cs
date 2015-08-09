using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GI.Tools
{
    class BouguerCorrection
    {
        #region 属性 各种路径
        /// <summary>
        /// exe路径
        /// </summary>
        public static string exePath = @"FA2BA_F.exe";
        /// <summary>
        /// TC.inp临时文件路径
        /// </summary>
        public static string tcPath = @"TC.inp";
        /// <summary>
        /// 站点文件(*.dat)路径
        /// </summary>
        public static string datPath = @"obs1_FA.dat";
        /// <summary>
        /// 内区地形数据文件(*.grd)路径
        /// </summary>
        public static string srtm30GrdPath = @"srtm30.grd";
        /// <summary>
        /// 外区地形数据文件(*.grd)路径
        /// </summary>
        public static string srtm60GrdPath = @"srtm60.grd";
        /// <summary>
        /// 输出文件(*.dat)路径
        /// </summary>
        public static string outPath = @"out.DAT";
        /// <summary>
        /// 任务控制
        /// </summary>
        public static Process p = null;
        #endregion

        /// <summary>
        /// 执行布格改正exe
        /// </summary>
        /// <param name="dat">站点文件(*.dat)</param>
        /// <param name="srtm30">内区地形数据文件(*.grd)</param>
        /// <param name="srtm60">外区地形数据文件(*.grd)</param>
        /// <param name="density">密度</param>
        /// <param name="innerRadius">内区半径</param>
        /// <param name="outterRadius">外区半径</param>
        /// <returns>异步执行Task，返回值为结果</returns>
        /// 
        public static Task<string> Start(string dat, string srtm30, string srtm60, double density, double innerRadius, double outterRadius)
        {
            //判断传入文件是否存在
            if (!File.Exists(dat))
                throw new Exception("站点文件不存在！");
            if (!File.Exists(srtm30))
                throw new Exception("内区地形数据文件不存在！");
            if (!File.Exists(srtm60))
                throw new Exception("外区地形数据文件不存在！");
            //如果输出文件不存在则自动创建输出文件
            if (!File.Exists(outPath))
                File.Create(outPath).Dispose();
            ////如果临时文件夹不存在则自动创建临时文件夹
            //if (!Directory.Exists(tmpPath))
            //    Directory.CreateDirectory(tmpPath);
            // 输入文件全部存入临时文件夹
            File.Copy(dat, datPath, true);
            File.Copy(srtm30, srtm30GrdPath, true);
            File.Copy(srtm60, srtm60GrdPath, true);
            // 构造TC.inp内容
            string tc = String.Format("obs1_FA.dat\nsrtm30.grd\nsrtm60.grd\nout.DAT\n5 3 0 2 {0}\n20.00416666 24.99583334 97.00416666 101.9958334\n{1} {2}", density, innerRadius, outterRadius);
            // 写入TC.inp
            using (var writer = new StreamWriter(tcPath, false))
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
                    throw new Exception("执行EXE失败！");
                }
            });
        }
    }

}

