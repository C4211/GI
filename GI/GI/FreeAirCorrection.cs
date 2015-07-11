using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GI
{
    /// <summary>
    /// 自空校正工具类
    /// </summary>
    class FreeAirCorrectionStart
    {
        /// <summary>
        /// 开始自空校正
        /// </summary>
        /// <param name="inputPath">输入文件路径</param>
        /// <param name="outputPath">输出文件路径</param>
        /// <param name="choice">计算方法</param>
        /// <returns>异步执行Task</returns>
        public static Task Start(string inputPath, string outputPath, int choice)
        {
            List<FreeAirCorrection> list;
            return Task.Run(() =>
            {
                list = ReadAndCheckInputFormat(inputPath);
                CalculateFreeAirAnomaly(list, choice);
                WriteOutput(list, outputPath);
            });
        }

        /// <summary>
        /// 读取输入文件并检查输入格式方法
        /// </summary>
        /// <param name="inputPath">输入文件路径</param>
        /// <returns>输入内容</returns>
        static List<FreeAirCorrection> ReadAndCheckInputFormat(string inputPath)
        {
            List<FreeAirCorrection> result = new List<FreeAirCorrection>();
            using (StreamReader sr = new StreamReader(inputPath, Encoding.Default))
            {
                string str = sr.ReadLine();
                string[] group;
                double longitude, latitude, height, observed;
                for (int line = 1; !string.IsNullOrEmpty(str); line++)
                {
                    // 以空格、Tab、逗号分隔
                    group = str.Split(new char[] { ' ', '\t', ',' },
                        StringSplitOptions.RemoveEmptyEntries);
                    // 检查参数数量
                    if (group.Length != 4)
                        throw new Exception(string.Format("第{0}行参数数量错误！", line));
                    // 格式检查
                    if (!(double.TryParse(group[0], out longitude) && longitude >= -180.0 && longitude <= 180.0))
                        throw new Exception("经度格式错误！");
                    if (!(double.TryParse(group[1], out latitude) && latitude >= -90.0 && latitude <= 90.0))
                        throw new Exception("纬度格式错误！");
                    if (!double.TryParse(group[2], out height))
                        throw new Exception("高度格式错误！");
                    if (!double.TryParse(group[3], out observed))
                        throw new Exception("测量值格式错误！");
                    // 保存
                    result.Add(new FreeAirCorrection(longitude, latitude, height, observed));
                    str = sr.ReadLine();
                }
                return result;
            }
        }

        /// <summary>
        /// 输出到文件
        /// </summary>
        /// <param name="list">自空校正对象列表</param>
        /// <param name="outputPath">输出路径</param>
        static void WriteOutput(List<FreeAirCorrection> list, string outputPath)
        {
            using (StreamWriter sw = new StreamWriter(outputPath, false, Encoding.Default))
            {
                foreach (var fac in list)
                {
                    sw.WriteLine(string.Format("{0} {1} {2} {3} {4}", fac.Longitude, fac.Latitude, fac.Height, fac.ObservedGravity, fac.FreeAirAnomaly));
                }
            }
        }

        /// <summary>
        /// 计算自空异常
        /// </summary>
        /// <param name="list">自空校正对象列表</param>
        /// <param name="choice">校正方法</param>
        static void CalculateFreeAirAnomaly(List<FreeAirCorrection> list, int choice)
        {
            FreeAirCorrection fac;
            for (int i = 0; i < list.Count; i++)
            {
                fac = list[i];
                // 计算正常重力
                CalculateNormalGravity(fac, choice);
                // 高度改正
                DoHeightCorrection(fac);
                // 计算自空异常
                if (fac.ObservedGravity > 9999999.0)
                {
                    //fac.ObservedGravity微伽，fac.FreeAirAnomaly毫伽
                    fac.FreeAirAnomaly = fac.ObservedGravity / 1000 + fac.HeightCorrection - fac.NormalGravity;
                }
                else
                {
                    //fac.ObservedGravity毫伽，fac.FreeAirAnomaly毫伽
                    fac.FreeAirAnomaly = fac.ObservedGravity + fac.HeightCorrection - fac.NormalGravity;
                }
            }
        }

        /// <summary>
        /// 计算正常重力
        /// </summary>
        /// <param name="fac">自空校正对象</param>
        /// <param name="choice">校正方法</param>
        static void CalculateNormalGravity(FreeAirCorrection fac, int choice)
        {
            double a = 0.0, b = 0.0, c = 0.0;
            switch (choice)
            {
                case 1://(1)1901-1909年的赫尔默特(R.Helment)公式
                    a = 978030.00; b = 0.005302; c = 0.000007;
                    break;
                case 2://(2)1930年卡西尼(Cassinis)国际正常重力公式
                    a = 978049.00; b = 0.0052884; c = 0.0000059;
                    break;
                case 3://(3)1971年国际正常重力公式
                    a = 978031.80; b = 0.0053024; c = 0.0000059;
                    break;
                case 4://(4)1979年IUGG确定的正常重力公式
                    a = 978032.70; b = 0.0053024; c = 0.000005;
                    break;
                default:
                    throw new Exception("公式编号不正确！");
            }
            fac.NormalGravity = a * (1 + b * Math.Sin(Radian(fac.Latitude)) * Math.Sin(Radian(fac.Latitude)) - c * Math.Sin(2 * Radian(fac.Latitude)));
        }

        /// <summary>
        /// 高度校正
        /// </summary>
        /// <param name="fac">自空校正对象</param>
        static void DoHeightCorrection(FreeAirCorrection fac)
        {
            fac.HeightCorrection = 0.3086 * (1 + 0.0007 * Math.Cos(2 * Radian(fac.Latitude))) * fac.Height - 7.2e-8 * fac.Height * fac.Height;
        }

        /// <summary>
        /// 角度转弧度
        /// </summary>
        /// <param name="degree">角度</param>
        /// <returns>弧度</returns>
        static double Radian(double degree)
        {
            return degree * Math.PI / 180;
        }

    }

    /// <summary>
    /// 自空类
    /// </summary>
    class FreeAirCorrection
    {
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public double Height { get; set; }
        public double ObservedGravity { get; set; }
        public double NormalGravity { get; set; }
        public double HeightCorrection { get; set; }
        public double FreeAirAnomaly { get; set; }
        public FreeAirCorrection(double longitude, double latitude, double height, double observed)
        {
            this.Longitude = longitude;
            this.Latitude = latitude;
            this.Height = height;
            this.ObservedGravity = observed;
        }
    }


}
