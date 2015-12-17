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


        public void PreviewShow(FileInfo grdFileInfo,FileInfo colorMapFileInfo)
        {
            Application.Current.MainWindow.Cursor = Cursors.Wait;
            GRDPreviewWindow gpw = new GRDPreviewWindow();
            gpw.GRDDrawing(grdFileInfo.FullName,colorMapFileInfo.FullName);
            ColorMap cm = new ColorMap(colorMapFileInfo.FullName);
            for (int i = 17; i >= 0; i--)
            {
                Rectangle rec = new Rectangle();
                rec.Width = 30;
                rec.Height = 30;
                rec.Fill = new SolidColorBrush(cm[i]);
                colors.Children.Add(rec);
            }
                
            gpw.ShowDialog();
            Application.Current.MainWindow.Cursor = Cursors.Arrow; 
        }

        public static void PreviewShow(Window owner, FileInfo grdFileInfo,FileInfo colorMapFileInfo)
        {
            Application.Current.MainWindow.Cursor = Cursors.Wait;
            GRDPreviewWindow gpw = new GRDPreviewWindow();
            gpw.Owner = owner;
            gpw.GRDDrawing(grdFileInfo.FullName,colorMapFileInfo.FullName);
            ColorMap cm = new ColorMap(colorMapFileInfo.FullName);
            for (int i = 17; i >= 0; i--)
            {
                Rectangle rec = new Rectangle();
                rec.Width = 30;
                rec.Height = 30;
                rec.Fill = new SolidColorBrush(cm[i]);
                gpw.colors.Children.Add(rec);
            }
            gpw.ShowDialog();
            Application.Current.MainWindow.Cursor = Cursors.Arrow; 
        }

        private void GRDDrawing(string filePath,string colorMapPath)
        {
            Grd grd = new Grd(filePath);
            WriteableBitmap wb = grd.GrdImage(new ColorMap(colorMapPath));
            grdImage.Source = wb;
                
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
