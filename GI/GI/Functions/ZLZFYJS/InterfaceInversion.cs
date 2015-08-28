﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GI.Functions
{
    class InterfaceInversion
    {
        #region 属性 各种路径
        /// <summary>
        /// exe路径
        /// </summary>
        public static string exePath = @"InterfaceInversion_F.exe";
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
        /// 重力界面反演
        /// </summary>
        /// <param name="input">输入文件</param>
        /// <param name="referenceDepth">参考深度</param>
        /// <param name="densityContrast">密度差</param>
        /// <param name="filterParameterWh">最小截断频率wh</param>
        /// <param name="filterParameterSh">最大截断频率sh</param>
        /// <param name="maxIteration">最大迭代次数</param>
        /// <param name="tolerance">收敛准则</param>
        /// <param name="coordinateUnit">坐标单位 度(°)=0 千米(km)=1</param>
        /// <returns></returns>
        public static Task<string> Start(string input, double referenceDepth, double densityContrast, double filterParameterWh, double filterParameterSh, int maxIteration, double tolerance, int coordinateUnit)
        {
            // 输入文件全部存入临时文件夹
            File.Copy(input, inPath, true);
            //如果输出文件不存在则自动创建输出文件
            if (!File.Exists(outPath))
                File.Create(outPath).Dispose();
            // 构造parameters.inp内容
            string tc = String.Format("{0}\n{1}\n{2} {3}\n{4} {5}\n{6} {7}\n{8}", inPath, outPath, referenceDepth, densityContrast, filterParameterWh, filterParameterSh, maxIteration, tolerance, coordinateUnit);
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