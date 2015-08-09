using GI.Tools;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GI.UserControls
{
    /// <summary>
    /// Function_fxyf.xaml 的交互逻辑
    /// </summary>
    public partial class Function_bggz : FunctionPage
    {
        public Function_bggz()
        {
            InitializeComponent();
            this.titleCn = "布格改正";
            this.titleEn = "Bouguer correction";
        }

        private int PageCurrent = 0;
        private int PageMax
        {
            get { return content.Children.Count-1; }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (PageCurrent < PageMax)
            {
                PageCurrent += 1;
                content.Children[PageCurrent].Visibility = Visibility.Visible;
                Storyboard sb = ((Storyboard)this.FindResource("sb")).Clone();
                ((ThicknessAnimation)sb.Children[0]).To = new Thickness(-PageCurrent*680, 0, 0, 0);
                sb.Completed += delegate { content.Children[PageCurrent-1].Visibility = Visibility.Hidden; };
                content.BeginStoryboard(sb);
                prev.Visibility = Visibility.Visible;
                if(PageCurrent>=PageMax)
                    next.Content = "计算";
            }
            else
            {
                loadingBar.Visibility = Visibility.Visible;

            }
        }

        private void prev_Click(object sender, RoutedEventArgs e)
        {
            if (PageCurrent > 0)
            {
                PageCurrent -= 1;
                content.Children[PageCurrent].Visibility = Visibility.Visible;
                Storyboard sb = ((Storyboard)this.FindResource("sb")).Clone();
                ((ThicknessAnimation)sb.Children[0]).To = new Thickness(-PageCurrent * 680, 0, 0, 0);
                sb.Completed += delegate { content.Children[PageCurrent + 1].Visibility = Visibility.Hidden; };
                content.BeginStoryboard(sb);
                if (PageCurrent <= 0)
                    prev.Visibility = Visibility.Hidden;
                next.Content = "下一步";
                next.Visibility = Visibility.Visible;
            }
        }

        private void DoBouguerCorrection()
        {
            string path1, path2, path3;
            double arg1, arg2, arg3;
            if (!FileNameFilter.CheckFileSuffix(inputPath1.filePath.Text))
                MessageBox.Show(Application.Current.MainWindow, "站点文件类型不正确");
            else if (!FileNameFilter.CheckFileExistence(inputPath1.filePath.Text))
                MessageBox.Show(Application.Current.MainWindow, "站点文件路径不存在");
            else if (!FileNameFilter.CheckFileSuffix(inputPath2.filePath.Text))
                MessageBox.Show(Application.Current.MainWindow, "内区地形文件类型不正确");
            else if (!FileNameFilter.CheckFileExistence(inputPath2.filePath.Text))
                MessageBox.Show(Application.Current.MainWindow, "内区地形文件路径不存在");
            else if (!FileNameFilter.CheckFileSuffix(inputPath3.filePath.Text))
                MessageBox.Show(Application.Current.MainWindow, "外区地形文件类型不正确");
            else if (!FileNameFilter.CheckFileExistence(inputPath3.filePath.Text))
                MessageBox.Show(Application.Current.MainWindow, "外区地形文件路径不存在");
            // else if (判断参数)
            else
            {

            }
        }
    }
}
