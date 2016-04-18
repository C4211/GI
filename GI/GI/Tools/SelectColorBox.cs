using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GI.Tools
{
    public class SelectColorBox : ComboBox
    {
        public SelectColorBox()
            : base()
        {
            this.Loaded += SelectColorBox_Loaded;
        }

        void SelectColorBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(ColorFilePath))
            {
                Directory.CreateDirectory(ColorFilePath);
            }
            DirectoryInfo dir = new DirectoryInfo(ColorFilePath);
            List<SelectColorItem> colors = new List<SelectColorItem>();
            SelectColorItem sci;
            foreach (var file in dir.GetFiles("*.clr"))
            {
                sci = new SelectColorItem();
                sci.ColorFilePath = file.FullName;
                colors.Add(sci);
            }
            this.ItemsSource = colors;
            this.SelectedIndex = 0;
        }

        #region 获取设置依赖属性
        public string ColorFilePath
        {
            get { return (string)GetValue(ColorFilePathProperty); }
            set { SetValue(ColorFilePathProperty, value); }
        }
        #endregion

        #region 注册依赖属性
        public static readonly DependencyProperty ColorFilePathProperty =
            DependencyProperty.Register("ColorFilePath", typeof(string), typeof(SelectColorBox));
        #endregion



    }

    public class SelectColorItem : Grid
    {
        public SelectColorItem() : base()
        {
            this.Loaded += SelectColorItem_Loaded;
        }

        private void SelectColorItem_Loaded(object sender, RoutedEventArgs e)
        {
            SetColorBrush();
        }

        #region 设置依赖属性 
        public string ColorFilePath
        {
            get { return (string)GetValue(ColorFilePathProperty); }
            set { SetValue(ColorFilePathProperty, value); }
        }

        #endregion

        #region 注册依赖属性
        public static readonly DependencyProperty ColorFilePathProperty =
            DependencyProperty.Register("ColorFilePath", typeof(string), typeof(SelectColorItem));
        #endregion

        private void SetColorBrush()
        {
            LinearGradientBrush brush = new LinearGradientBrush();
            brush.StartPoint = new Point(0, 0.5);
            brush.EndPoint = new Point(1, 0.5);
            using (StreamReader sr = new StreamReader(ColorFilePath))
            {
                sr.ReadLine();
                string str;
                string[] strs;
                double index;
                byte r, g, b;

                while ((str = sr.ReadLine()) != null)
                {
                    //MessageBox.Show(str);
                    strs = str.Split(new char[] { ' ', '\t', ',' }, StringSplitOptions.RemoveEmptyEntries);
                    try
                    {
                        index = double.Parse(strs[0]);
                        r = (byte)int.Parse(strs[1]);
                        g = (byte)int.Parse(strs[2]);
                        b = (byte)int.Parse(strs[3]);
                        brush.GradientStops.Add(new GradientStop(Color.FromRgb(r, g, b), index / 100));
                    }
                    catch
                    {
                        throw new Exception("读取颜色文件失败！");
                    }
                }

                this.Background = brush;
            }
        }

        public static Brush ColorBrush(string ColorFilePath)
        {
            LinearGradientBrush brush = new LinearGradientBrush();
            brush.StartPoint = new Point(0.5, 0);
            brush.EndPoint = new Point(0.5, 1);
            using (StreamReader sr = new StreamReader(ColorFilePath))
            {
                sr.ReadLine();
                string str;
                string[] strs;
                double index;
                byte r, g, b;

                while ((str = sr.ReadLine()) != null)
                {
                    //MessageBox.Show(str);
                    strs = str.Split(new char[] { ' ', '\t', ',' }, StringSplitOptions.RemoveEmptyEntries);
                    try
                    {
                        index = double.Parse(strs[0]);
                        r = (byte)int.Parse(strs[1]);
                        g = (byte)int.Parse(strs[2]);
                        b = (byte)int.Parse(strs[3]);
                        brush.GradientStops.Add(new GradientStop(Color.FromRgb(r, g, b), index / 100));
                    }
                    catch
                    {
                        throw new Exception("读取颜色文件失败！");
                    }
                }

                return brush;
            }
        }
    }
}