using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace GI.Tools
{
    /// <summary>
    /// FilePreviewWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PSDPreviewWindow : Window
    {
        private PSDPreviewWindow()
        {
            InitializeComponent();
        }

        public static void PreviwShow(Window owner, FileInfo fileInfo)
        {
            Application.Current.MainWindow.Cursor = Cursors.Wait;
            PSDPreviewWindow ppw = new PSDPreviewWindow();
            ppw.Owner = owner;
            ppw.PSDDrawing(fileInfo.FullName);
            ppw.ShowDialog();
            Application.Current.MainWindow.Cursor = Cursors.Arrow; 
        }

        private void PSDDrawing(string filepath)
        {
                List<List<double>> points = LoadPoint(filepath);
                List<double> pointsX = points[0];
                List<double> pointsY = points[1];
                double pointsX_Max = pointsX.Max();
                double pointsY_Max = pointsY.Max();
                double pointsX_Min = pointsX.Min();
                double pointsY_Min = pointsY.Min();
                double ExpandX = 500 / (pointsX_Max - pointsX_Min);
                double ExpandY = 350 / (pointsY_Max - pointsY_Min);
                double UnitX = (pointsX_Max - pointsX_Min) / 5;
                double UnitY = (pointsY_Max - pointsY_Min) / 10;
                if (UnitY == 0)
                {
                    (unity.Children[5] as TextBlock).Text = (pointsY_Min).ToString("0.000");
                    #region 画刻度
                    PathGeometry pgunit = new PathGeometry();
                    for (int i = 0; i <= 10; i++)
                    {
                        
                        PathFigure Unit = new PathFigure();
                        Unit.IsClosed = false;
                        Unit.StartPoint = new Point(0, 35 * i);
                        Unit.Segments.Add(new LineSegment(new Point(500, 35 * i), true));
                        pgunit.Figures.Add(Unit);

                    }
                    for (int i = 0; i <= 5; i++)
                    {
                        PathFigure Unit = new PathFigure();
                        Unit.IsClosed = false;
                        Unit.StartPoint = new Point(100 * i, 0);
                        Unit.Segments.Add(new LineSegment(new Point(100 * i, 350), true));
                        pgunit.Figures.Add(Unit);
                        (unitx.Children[i] as TextBlock).Text = (pointsX_Min + UnitX * i).ToString("0.00");
                    }
                    unit.Data = pgunit;
                    #endregion
                    #region 画线
                    PathGeometry pgPSD = new PathGeometry();
                    PathFigure PSD = new PathFigure();
                    PSD.IsClosed = false;
                    PSD.StartPoint = new Point(0, 35 * 5);
                    PSD.Segments.Add(new LineSegment(new Point(500, 35 * 5), true));
                    pgPSD.Figures.Add(PSD);
                    path.Data = pgPSD;
                    path.Stretch = Stretch.None;
                    #endregion
                }
                else
                {
                    #region 画刻度
                    PathGeometry pgunit = new PathGeometry();
                    for (int i = 0; i <= 10; i++)
                    {
                        PathFigure Unit = new PathFigure();
                        Unit.IsClosed = false;
                        Unit.StartPoint = new Point(0, 35 * i);
                        Unit.Segments.Add(new LineSegment(new Point(500, 35 * i), true));
                        pgunit.Figures.Add(Unit);
                        (unity.Children[unity.Children.Count - i - 1] as TextBlock).Text = (pointsY_Min + UnitY * i).ToString("0.000");
                    }
                    for (int i = 0; i <= 5; i++)
                    {
                        PathFigure Unit = new PathFigure();
                        Unit.IsClosed = false;
                        Unit.StartPoint = new Point(100 * i, 0);
                        Unit.Segments.Add(new LineSegment(new Point(100 * i, 350), true));
                        pgunit.Figures.Add(Unit);
                        (unitx.Children[i] as TextBlock).Text = (pointsX_Min + UnitX * i).ToString("0.00");
                    }
                    unit.Data = pgunit;
                    #endregion
                    #region 画线
                    PathGeometry pgPSD = new PathGeometry();
                    PathFigure PSD = new PathFigure();
                    PSD.IsClosed = false;
                    bool PSDn = true;
                    for (int i = 0; i < pointsX.Count; i++)
                    {
                        if (PSDn)
                        {
                            PSD.StartPoint = new Point(pointsX[i] * ExpandX, -pointsY[i] * ExpandY);
                            PSDn = false;
                        }
                        else
                        {
                            LineSegment ls = new LineSegment(new Point(pointsX[i] * ExpandX, -pointsY[i] * ExpandY), true);
                            PSD.Segments.Add(ls);
                        }
                    }
                    pgPSD.Figures.Add(PSD);
                    path.Data = pgPSD;
                    path.Stretch = Stretch.Fill;
                    #endregion
                }
        }

        private List<List<double>> LoadPoint(string filepath)
        {
            List<double> pointsX = new List<double>();
            List<double> pointsY = new List<double>();
            List<List<double>> points = new List<List<double>>();
            using (StreamReader sr = new StreamReader(filepath, Encoding.Default))
            {
                string str = sr.ReadLine();
                string[] p;
                for (int line = 1; !string.IsNullOrEmpty(str); line++)
                {
                    if (line == 1)
                        continue;
                    p = str.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    try
                    {
                        pointsX.Add(double.Parse(p[0]));
                        pointsY.Add(double.Parse(p[1]));
                    }
                    catch{ }
                    str = sr.ReadLine();
                }
            }
            points.Add(pointsX);
            points.Add(pointsY);
            return points;
        }
        

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private bool isClosed = false;
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (isClosed == false)
            {
                e.Cancel = true;
                Storyboard sb = (this.FindResource("GI.Window.closeStoryboard") as Storyboard).Clone();
                sb.Completed += delegate { isClosed = true; this.Close(); };
                content.BeginStoryboard(sb);
            }
            else
            {
                e.Cancel = false;
            }
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            Storyboard sb = (this.FindResource("GI.Window.Border.Focus") as Storyboard).Clone();
            content.BeginStoryboard(sb);
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            Storyboard sb = (this.FindResource("GI.Window.Border.Default") as Storyboard).Clone();
            content.BeginStoryboard(sb);
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
            {
                Storyboard sb = (this.FindResource("GI.Window.openStoryboard") as Storyboard).Clone();
                content.BeginStoryboard(sb);
            }
        }


        private void Head_Min_Click(object sender, RoutedEventArgs e)
        {
            Storyboard sb = (this.FindResource("GI.Window.closeStoryboard") as Storyboard).Clone();
            sb.Completed += delegate { this.WindowState = WindowState.Minimized; };
            content.BeginStoryboard(sb);
        }

        public void Min()
        {
            Storyboard sb = (this.FindResource("GI.Window.closeStoryboard") as Storyboard).Clone();
            sb.Completed += delegate { this.WindowState = WindowState.Minimized; };
            content.BeginStoryboard(sb);
        }

        public void Normal()
        {
            this.WindowState = WindowState.Normal;
        }
    }
}
