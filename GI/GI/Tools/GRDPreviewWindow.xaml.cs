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
    public partial class GRDPreviewWindow : Window
    {
        private GRDPreviewWindow()
        {
            InitializeComponent();
        }

        private GRDPreviewWindow(string filePath)
        {
            InitializeComponent();
            this.filePath = filePath;
        }

        private int colors = 90;
        private int roundDecimals = 3;

        public void PreviewShow(FileInfo grdFileInfo, int roundDecimals,int colors)
        {
            Application.Current.MainWindow.Cursor = Cursors.Wait;
            GRDPreviewWindow gpw = new GRDPreviewWindow(grdFileInfo.FullName);
            SelectColorItem sci = (SelectColorItem)gpw.inputPath2.SelectedItem;
            gpw.colors = colors;
            gpw.roundDecimals = roundDecimals;
            gpw.GRDDrawing(grdFileInfo.FullName, sci.ColorFilePath, colors);
            gpw.UnitFill(grdFileInfo.FullName, roundDecimals);
            //gpw.inputPath2.Loaded += delegate { gpw.SaveImage(gpw.grdContent); };
            gpw.ShowDialog();
            Application.Current.MainWindow.Cursor = Cursors.Arrow; 
        }

        public static void PreviewShow(Window owner, FileInfo grdFileInfo, SelectColorBox colorBox, int roundDecimals, int colors,int roundIndex)
        {
            
            Application.Current.MainWindow.Cursor = Cursors.Wait;
            GRDPreviewWindow gpw = new GRDPreviewWindow(grdFileInfo.FullName);
            gpw.Owner = owner;
            gpw.colors = colors;
            gpw.roundDecimals = roundDecimals;
            SelectColorItem sci = (SelectColorItem)colorBox.SelectedItem;
            gpw.GRDDrawing(grdFileInfo.FullName, sci.ColorFilePath, colors);
            gpw.UnitFill(grdFileInfo.FullName, roundDecimals);
            gpw.inputPath2.Loaded += delegate { gpw.inputPath2.SelectedIndex = colorBox.SelectedIndex; gpw.round.SelectedIndex = roundIndex; };
            gpw.ShowDialog();
            Application.Current.MainWindow.Cursor = Cursors.Arrow; 
        }

        private void GRDDrawing(string filePath,string colorMapPath,int colors)
        {
            Grd grd = new Grd(filePath);
            WriteableBitmap wb = grd.GrdImage(new ColorMap(colorMapPath,colors));
            grdImage.Source = wb;
            colorRect.Fill = SelectColorItem.ColorBrush(colorMapPath);
        }

        private void UnitFill(string filePath,int decimals)
        {
            Grd grd = new Grd(filePath);
            double intervalX = (grd.maxx-grd.minx)/grdX.Children.Count;
            for(int i = 0;i<grdX.Children.Count;i++)
            {
                ((TextBlock)grdX.Children[i]).Text = (Math.Round((grd.minx + intervalX * i), decimals)).ToString("f"+decimals);
            }

            double intervalY = (grd.maxy - grd.miny) / grdY.Children.Count;
            for (int i = 0; i < grdY.Children.Count; i++)
            {
                ((TextBlock)grdY.Children[i]).Text = (Math.Round((grd.miny + intervalY * i), decimals)).ToString("f" + decimals);
            }

            double intervalZ = (grd.max - grd.min) / grdZ.Children.Count;
            for (int i = 0; i < grdZ.Children.Count; i++)
            {
                ((TextBlock)grdZ.Children[i]).Text = (grd.max - intervalZ * i).ToString("0.000");
            }
            
        }

        private string filePath;
        private void inputPath2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GRDDrawing(filePath, ((SelectColorItem)(((SelectColorBox)sender).SelectedItem)).ColorFilePath, colors);
        }
        private void round_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var decimals = (ComboBoxItem)((ComboBox)sender).SelectedItem;
            roundDecimals = Int16.Parse(decimals.Tag.ToString());
            UnitFill(filePath, roundDecimals);
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog ofd = new System.Windows.Forms.SaveFileDialog();
            ofd.Filter = "png文件(*.png)|*.png|jpeg文件(*.jpeg)|*.jpeg|bmp文件(*.bmp)|*.bmp";
            ofd.FilterIndex = 1;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    SaveToImage(saveToImageContent, ofd.FileName, ofd.FilterIndex);
                    Msg("已保存！");
                }
                catch
                {
                    Msg("保存失败！");
                }
            }
        }

        //private RenderTargetBitmap grdbmp;
        private void SaveToImage(FrameworkElement ui, string fileName,int img)
        {
            BitmapEncoder encoder = null;
            switch (img)
            {
                case 0:
                    encoder = new PngBitmapEncoder();
                    break;
                case 1:
                    encoder = new JpegBitmapEncoder();
                    break;
                case 2:
                    encoder = new BmpBitmapEncoder();
                    break;
                default:
                    throw new Exception("图像编码失败！");
            }
            RenderTargetBitmap grdbmp = new RenderTargetBitmap((int)ui.ActualWidth, (int)ui.ActualHeight, 96d, 96d,
                PixelFormats.Pbgra32);
            grdbmp.Render(ui);
            System.IO.FileStream fs = new System.IO.FileStream(fileName, System.IO.FileMode.Create);
            encoder.Frames.Add(BitmapFrame.Create(grdbmp));
            encoder.Save(fs);
            fs.Close();
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

        private void Msg(string msg)
        {
            Dispatcher.Invoke(delegate { MessageWindow.Show(Application.Current.MainWindow, msg); });
        }

        #region 放大功能

        private bool mouseDown;
        private Point mouseXY;
        private double min = 0.6, max = 3.0;//最小/最大放大倍数

        private void Domousemove(Canvas img, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }
            var group = grdContent.FindResource("TfGroup1") as TransformGroup;
            var transform = group.Children[1] as TranslateTransform;
            var transform2 = group.Children[0] as ScaleTransform;
            //获取拖拽后的鼠标坐标
            var position = e.GetPosition(fileContentGrid);
            //修改图片偏移量
            transform.X -= mouseXY.X - position.X;
            transform.Y -= mouseXY.Y - position.Y;
            mouseXY = position;
        }


        private void DowheelZoom(TransformGroup group, Point point, double delta)
        {
            //修改缩放中心(跟随鼠标）
            var transform1 = group.Children[1] as TranslateTransform;
            //point.X += transform1.X;
            //point.Y += transform1.Y;
            var pointToContent = group.Inverse.Transform(point);
            //控制图片缩放倍数
            var transform = group.Children[0] as ScaleTransform;
            if (transform.ScaleX + delta < min)
            {
                transform.ScaleX = min;
                transform.ScaleY = min;
            }
            else if (transform.ScaleX + delta > max)
            {
                transform.ScaleX = max;
                transform.ScaleY = max;
            }
            else
            {
                transform.ScaleX += delta;
                transform.ScaleY += delta;
            }
            transform1.X = -1 * ((pointToContent.X * transform.ScaleX) - point.X);
            transform1.Y = -1 * ((pointToContent.Y * transform.ScaleY) - point.Y);
        }


        private void ContentControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var img = sender as Canvas;
            if (img == null)
            {
                return;
            }
            img.CaptureMouse();
            mouseDown = true;
            //获取点击时的鼠标坐标
            mouseXY = e.GetPosition(fileContentGrid);
        }

        private void ContentControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var img = sender as Canvas;
            if (img == null)
            {
                return;
            }
            img.ReleaseMouseCapture();
            mouseDown = false;
        }

        private void ContentControl_MouseMove(object sender, MouseEventArgs e)
        {
            var img = sender as Canvas;
            if (img == null)
            {
                return;
            }
            if (mouseDown)
            {
                Domousemove(img, e);
            }
        }

        private void ContentControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var img = sender as Canvas;
            if (img == null)
            {
                return;
            }
            var point = e.GetPosition(fileContentGrid);
            var group = grdContent.FindResource("TfGroup1") as TransformGroup;
            var delta = e.Delta * 0.0005;
            DowheelZoom(group, point, delta);
        }

        #endregion

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            grdGrid.Visibility = Visibility.Visible;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            grdGrid.Visibility = Visibility.Hidden;
        }

        

    }
}
