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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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

        public static void PreviwShow(Window owner,ResourceManagerTreeNode fileInfo)
        {
            if (!(fileInfo.Path.Extension.Equals(".txt", StringComparison.OrdinalIgnoreCase)
                  ||  fileInfo.Path.Extension.Equals(".dat", StringComparison.OrdinalIgnoreCase)
                  || fileInfo.Path.Extension.Equals(".grd", StringComparison.OrdinalIgnoreCase)))
            {
                MessageWindow.Show(Application.Current.MainWindow,"只能预览txt/dat/grd文件！");
                return;
            }
            FilePreviewWindow fpw = new FilePreviewWindow();
            fpw.Owner = owner;
            fpw.fileName.Text = fileInfo.Path.Name;
            try
            {
                using (var stream = new StreamReader(fileInfo.Path.FullName, Encoding.Default))
                {
                    fpw.fileContent.Text = stream.ReadToEnd();
                }
            }
            catch (Exception)
            {
                MessageWindow.Show(fpw,"读取文件失败！");
                throw;
            }
            fpw.Show();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Storyboard sb = (this.FindResource("GI.Window.closeStoryboard") as Storyboard).Clone();
            sb.Completed += delegate { this.Close(); };
            content.BeginStoryboard(sb);
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
    }
}
