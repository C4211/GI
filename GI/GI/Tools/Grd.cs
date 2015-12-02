using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GI.Tools
{
    public class Grd
    {
        #region 属性
        /// <summary>
        /// 横向像素数量
        /// </summary>
        public int width { get; set; }
        /// <summary>
        /// 纵向像素数量
        /// </summary>
        public int height { get; set; }

        /// <summary>
        /// 最高海拔
        /// </summary>
        public double max { get; set; }
        /// <summary>
        /// 最低海拔
        /// </summary>
        public double min { get; set; }

        /// <summary>
        /// 经度最小值
        /// </summary>
        public double miny { get; set; }
        /// <summary>
        /// 经度最大值
        /// </summary>
        public double maxy { get; set; }

        /// <summary>
        /// 纬度最小值
        /// </summary>
        public double minx { get; set; }
        /// <summary>
        /// 纬度最大值
        /// </summary>
        public double maxx { get; set; }

        /// <summary>
        /// 海拔矩阵
        /// </summary>
        public double[,] matrix { get; set; }


        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="filepath">grd文件路径</param>
        public Grd(string filepath)
        {
            using (StreamReader sr = new StreamReader(filepath))
            {
                string[] strs;
                //第一行
                string str = sr.ReadLine();

                //第二行
                str = sr.ReadLine();
                strs = str.Split(new char[] { ' ', '\t', ',' }, StringSplitOptions.RemoveEmptyEntries);
                try
                {
                    width = int.Parse(strs[0]);
                    height = int.Parse(strs[1]);
                }
                catch { }

                //第三行
                str = sr.ReadLine();
                strs = str.Split(new char[] { ' ', '\t', ',' }, StringSplitOptions.RemoveEmptyEntries);
                try
                {
                    minx = double.Parse(strs[0]);
                    maxx = double.Parse(strs[1]);
                }
                catch { }

                //第四行
                str = sr.ReadLine();
                strs = str.Split(new char[] { ' ', '\t', ',' }, StringSplitOptions.RemoveEmptyEntries);
                try
                {
                    miny = double.Parse(strs[0]);
                    maxy = double.Parse(strs[1]);
                }
                catch { }

                //第五行
                str = sr.ReadLine();
                strs = str.Split(new char[] { ' ', '\t', ',' }, StringSplitOptions.RemoveEmptyEntries);
                try
                {
                    min = double.Parse(strs[0]);
                    max = double.Parse(strs[1]);
                }
                catch { }

                //第六行以后
                matrix = new double[height, width];
                str = sr.ReadToEnd();
                strs = str.Split(new char[] { ' ', '\t', ',', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                try
                {
                    int k = 0;
                    for (int i = 0; i < height; i++)
                        for (int j = 0; j < width; j++, k++)
                        {
                            matrix[i, j] = double.Parse(strs[k]);
                        }
                }
                catch { }
            }
        }
        #endregion

        #region 函数
        /// <summary>
        /// Grd颜色矩阵
        /// </summary>
        /// <returns>颜色矩阵</returns>
        public Color[,] ColorMatrix()
        {
            Color[,] colorMatrix = new Color[height, width];
            ColorTransformer ct = new ColorTransformer(max, min);
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                {
                    colorMatrix[i, j] = ct.ColorTransform(matrix[i, j]);
                }
            return colorMatrix;
        }

        /// <summary>
        /// 绘制Grd图片
        /// </summary>
        /// <returns>绘制好的图片</returns>
        public WriteableBitmap GrdImage()
        {
            WriteableBitmap wb = new WriteableBitmap(width, height, 72, 72, PixelFormats.Bgr24, null);
            Color[,] matrix = ColorMatrix();
            for (int i = 0; i < wb.PixelHeight; i++)
                for (int j = 0; j < wb.PixelWidth; j++)
                {
                    Int32Rect sourceRect = new Int32Rect(j, wb.PixelHeight - i - 1, 1, 1);
                    byte[] colorData = { matrix[i, j].B, matrix[i, j].G, matrix[i, j].R };
                    wb.WritePixels(sourceRect, colorData, wb.BackBufferStride, 0);
                }
            return wb;
        }

        #endregion

    }

    class ColorTransformer
    {
        #region 变量
        /// <summary>
        /// 连续区间的最大值
        /// </summary>
        private double _max { get; set; }
        /// <summary>
        /// 连续区间的最小值
        /// </summary>
        private double _min { get; set; }

        /// <summary>
        /// 连续区间的间隔
        /// </summary>
        private double interval
        {
            get { return (_max - _min) / 1280; }
        }

        /// <summary>
        /// 最小颜色值
        /// </summary>
        private Color colorMin = new Color() { R = 255, G = 0, B = 255 };
        #endregion

        #region 构造函数
        /// <summary>
        /// ColorTransform类构造函数
        /// </summary>
        /// <param name="max">连续区间的最大值</param>
        /// <param name="min">连续区间的最小值</param>
        public ColorTransformer(double max, double min)
        {
            _max = max;
            _min = min;
        }
        #endregion

        #region 方法
        /// <summary>
        /// 传入区间内数值转换为Color
        /// </summary>
        /// <param name="value">传入的区间内数值</param>
        /// <returns>输出的Color</returns>
        public Color ColorTransform(double value)
        {
            int convert = ValueToInt(value);
            return IntToColor(convert);
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// 传入区间内数值转换为Int
        /// </summary>
        /// <param name="value">传入的区间内数值</param>
        /// <returns>输出的整型数值</returns>
        private int ValueToInt(double value)
        {
            try
            {
                int result = (int)((value - _min) / interval);
                return result;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 整形数值转换为Color
        /// </summary>
        /// <param name="value">传入的整形数值</param>
        /// <returns>输出的Color</returns>
        private Color IntToColor(int value)
        {
            Color c = colorMin;
            if (value <= 255)
            {
                c.R -= (byte)value;
                return c;
            }
            if (value <= 511)
            {
                c.R = 0;
                c.G = (byte)(value - 256);
                return c;
            }
            if (value <= 767)
            {
                c.R = 0;
                c.G = 255;
                c.B = (byte)(767 - value);
                return c;
            }
            if (value <= 1023)
            {
                c.R = (byte)(value - 768);
                c.G = 255;
                c.B = 0;
                return c;
            }
            if (value <= 1279)
            {
                c.R = 255;
                c.G = (byte)(1279 - value);
                c.B = 0;
                return c;
            }
            return c;
        }
        #endregion
    }
}
