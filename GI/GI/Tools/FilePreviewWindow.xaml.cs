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
    public partial class FilePreviewWindow : Window
    {
        private FilePreviewWindow()
        {
            InitializeComponent();
        }

        public static void PreviwShow(Window owner, ResourceManagerTreeNode fileInfo)
        {
            Application.Current.MainWindow.Cursor = Cursors.Wait;
            if (!(fileInfo.Path.Extension.Equals(".txt", StringComparison.OrdinalIgnoreCase)
                  ||  fileInfo.Path.Extension.Equals(".dat", StringComparison.OrdinalIgnoreCase)
                  || fileInfo.Path.Extension.Equals(".grd", StringComparison.OrdinalIgnoreCase)))
            {
                MessageWindow.Show(Application.Current.MainWindow,"只能预览txt/dat/grd文件！");
                return;
            }
            FilePreviewWindow fpw = new FilePreviewWindow();
            fpw.Owner = owner;
            fpw.Title = fileInfo.Path.Name;
            fpw.fileName.Text = fileInfo.Path.Name;
            fpw.fileName.ToolTip = fileInfo.Path.FullName;
            
                try
                {
                    using (var stream = new StreamReader(fileInfo.Path.FullName, Encoding.Default))
                    {
                        StringBuilder result = new StringBuilder(stream.ReadToEnd());
                        stream.Close();
                        fpw.Dispatcher.Invoke(
                            new Action(() =>
                            {
                                fpw.fileContent.AppendText(result.ToString());
                            }));

                        
                    }
                }
                catch (Exception)
                {
                    fpw.Dispatcher.Invoke(
                            new Action(() =>
                            {
                                MessageWindow.Show(fpw, "读取文件失败！");
                                fpw.Close();
                            }));
                    throw;
                }
                fpw.Show();
                Application.Current.MainWindow.Cursor = Cursors.Arrow;
                fpw.Activate();
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
    }
}
