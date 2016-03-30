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
        public GRDPreviewWindow()
        {
            InitializeComponent();
        }

        private static int colors = 90;

        public void PreviewShow(FileInfo grdFileInfo)
        {
            Application.Current.MainWindow.Cursor = Cursors.Wait;
            GRDPreviewWindow gpw = new GRDPreviewWindow();
            SelectColorItem sci = (SelectColorItem)gpw.inputPath2.SelectedItem;
            gpw.GRDDrawing(grdFileInfo.FullName, sci.ColorFilePath, colors);
            gpw.UnitFill(grdFileInfo.FullName);
            filePath = grdFileInfo.FullName;
            gpw.inputPath2.Loaded += delegate { gpw.SaveImage(gpw.grdContent); };
            gpw.ShowDialog();
            Application.Current.MainWindow.Cursor = Cursors.Arrow; 
        }

        public static void PreviewShow(Window owner, FileInfo grdFileInfo, SelectColorBox colorBox)
        {
            Application.Current.MainWindow.Cursor = Cursors.Wait;
            GRDPreviewWindow gpw = new GRDPreviewWindow();
            gpw.Owner = owner;
            SelectColorItem sci = (SelectColorItem)colorBox.SelectedItem;
            gpw.GRDDrawing(grdFileInfo.FullName, sci.ColorFilePath, colors);
            gpw.UnitFill(grdFileInfo.FullName);
            gpw.filePath = grdFileInfo.FullName;
            gpw.inputPath2.Loaded += delegate { gpw.inputPath2.SelectedIndex = colorBox.SelectedIndex; gpw.SaveImage(gpw.grdContent); };
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

        private void UnitFill(string filePath)
        {
            Grd grd = new Grd(filePath);
            double intervalX = (grd.maxx-grd.minx)/grdX.Children.Count;
            for(int i = 0;i<grdX.Children.Count;i++)
            {
                ((TextBlock)grdX.Children[i]).Text = (grd.minx + intervalX*i).ToString("0.000000").Substring(0,7);
            }

            double intervalY = (grd.maxy - grd.miny) / grdY.Children.Count;
            for (int i = 0; i < grdY.Children.Count; i++)
            {
                ((TextBlock)grdY.Children[i]).Text = (grd.maxy - intervalY * i).ToString("0.000000").Substring(0,7);
            }

            double intervalZ = (grd.max - grd.min) / grdZ.Children.Count;
            for (int i = 0; i < grdZ.Children.Count; i++)
            {
                ((TextBlock)grdZ.Children[i]).Text = (grd.max - intervalZ * i).ToString("0.0000000000").Substring(0,10);
            }
            
        }

        private string filePath = "";
        private void inputPath2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GRDDrawing(filePath, ((SelectColorItem)(((SelectColorBox)sender).SelectedItem)).ColorFilePath, colors);
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveImage(grdContent);
            System.Windows.Forms.SaveFileDialog ofd = new System.Windows.Forms.SaveFileDialog();
            ofd.Filter = "png文件(*.png)|*.png";
            ofd.FilterIndex = 1;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    SaveToImage(grdbmp, ofd.FileName);
                    Msg("已保存！");
                }
                catch
                {
                    Msg("保存失败！");
                }
            }
        }

        private RenderTargetBitmap grdbmp;
        private void SaveImage(FrameworkElement ui)
        {
            grdbmp = new RenderTargetBitmap((int)ui.ActualWidth, (int)ui.ActualHeight, 96d, 96d,
                PixelFormats.Pbgra32);
            grdbmp.Render(ui);
        }
        private void SaveToImage(RenderTargetBitmap grdbmp, string fileName)
        {
            System.IO.FileStream fs = new System.IO.FileStream(fileName, System.IO.FileMode.Create);
            BitmapEncoder encoder = new PngBitmapEncoder();
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
        

        
    }
}
