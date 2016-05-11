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

        public static void PreviewShow(ResourceManagerTreeNode grdFileInfo, int roundDecimals, int colors)
        {
            if (FilePreviewWindow.showWindows.ContainsKey(grdFileInfo.Path.FullName))
            {
                FilePreviewWindow.showWindows[grdFileInfo.Path.FullName].Activate();
                return;
            }
            GRDPreviewWindow gpw = new GRDPreviewWindow(grdFileInfo.Path.FullName);
            gpw.fileName.Text = grdFileInfo.Path.Name;
            SelectColorItem sci = (SelectColorItem)gpw.inputPath2.SelectedItem;
            gpw.colors = colors;
            gpw.Title = grdFileInfo.Path.Name;
            gpw.roundDecimals = roundDecimals;
            gpw.openSb.Completed += delegate
            {
                gpw.isOpen = true;
                gpw.inputPath2.SelectedIndex = -1;
                gpw.inputPath2.SelectedIndex = 0;
                gpw.round.SelectedIndex = 2;
            };
            gpw.Show();
            FilePreviewWindow.showWindows.Add(grdFileInfo.Path.FullName, gpw);
        }

        public static void PreviewShow(Window owner, FileInfo grdFileInfo, SelectColorBox colorBox, int roundDecimals, int colors,int roundIndex)
        {
            GRDPreviewWindow gpw = new GRDPreviewWindow(grdFileInfo.FullName);
            gpw.Owner = owner;
            gpw.colors = colors;
            gpw.Title = "GRD画图";
            gpw.roundDecimals = roundDecimals;
            SelectColorItem sci = (SelectColorItem)colorBox.SelectedItem;
            gpw.openSb.Completed += delegate
            {
                gpw.isOpen = true;
                gpw.inputPath2.SelectedIndex = -1;
                gpw.inputPath2.SelectedIndex = colorBox.SelectedIndex;
                gpw.round.SelectedIndex = roundIndex;
            };
            gpw.ShowInTaskbar = false;
            gpw.ShowDialog();
        }

        private void GRDDrawing(string filePath,string colorMapPath,int colors)
        {
            this.Cursor = Cursors.Wait;
            Grd grd = new Grd(filePath);
            WriteableBitmap wb = grd.GrdImage(new ColorMap(colorMapPath,colors));
            grdImage.Source = wb;
            colorRect.Fill = SelectColorItem.ColorBrush(colorMapPath);
            grdOutGrid.Visibility = Visibility.Visible;
            this.Cursor = Cursors.Arrow; 
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
            if(isOpen &&  ((SelectColorBox)sender).SelectedItem!=null)
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
                case 1:
                    encoder = new PngBitmapEncoder();
                    break;
                case 2:
                    encoder = new JpegBitmapEncoder();
                    break;
                case 3:
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
            FilePreviewWindow.showWindows.Remove(this.filePath);
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
        private double min = 1, max = 1.5;//最小/最大放大倍数

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
            if (transform.X - mouseXY.X + position.X >= 0)
            {
                transform.X = 0;

            }
            else if (transform.X - mouseXY.X + position.X <= -(grdContent.Width*(transform2.ScaleX-1)))
            {
                transform.X = -(transform2.ScaleX - 1) * grdContent.Width;
            }
            else
            {
                transform.X -= mouseXY.X - position.X;
            }
            if (transform.Y - mouseXY.Y + position.Y >= 0)
            {
                transform.Y = 0;
            }
            else if (transform.Y - mouseXY.Y + position.Y <= -(transform2.ScaleY - 1) * grdContent.Height)
            {
                transform.Y = -(transform2.ScaleY - 1) * grdContent.Height;
            }
            else
            {
                transform.Y -= mouseXY.Y - position.Y;
            }
            
            mouseXY = position;
        }


        private void DowheelZoom(TransformGroup group, Point point, double delta)
        {
            //修改缩放中心(跟随鼠标）
            var transform1 = group.Children[1] as TranslateTransform;
            var pointToContent = group.Inverse.Transform(point);
            //控制图片缩放倍数
            var transform = group.Children[0] as ScaleTransform;
            if (transform.ScaleX + delta < min)
            {
                transform.ScaleX = min;
                transform.ScaleY = min;
                transform1.X = 0;
                transform1.Y = 0;
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
                transform1.X = -((pointToContent.X * transform.ScaleX) - point.X);
                transform1.Y = -((pointToContent.Y * transform.ScaleY) - point.Y);
            }
            if (transform.ScaleX <= 1)
            {
                grdContent.Cursor = Cursors.Arrow;
            }
            else
            {
                grdContent.Cursor = Cursors.Hand;
            }

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
            var point = e.GetPosition(saveToImageContent);
            var group = grdContent.FindResource("TfGroup1") as TransformGroup;
            var delta = e.Delta * 0.001;
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

        public bool isOpen = false;
        public Storyboard openSb = (Application.Current.FindResource("GI.Window.openStoryboard") as Storyboard).Clone();
        private void content_Loaded(object sender, RoutedEventArgs e)
        {
            content.BeginStoryboard(openSb);
        }

        

    }
}
