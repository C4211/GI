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
                DoBouguerCorrection();
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

        private async void DoBouguerCorrection()
        {
            loadingBar.Show();
            string path1 = inputPath1.filePath.Text;
            string path2 = inputPath2.filePath.Text;
            string path3 = inputPath3.filePath.Text;
            double _arg1, _arg2, _arg3;
            if (!FileNameFilter.CheckFileSuffix(path1))
                Msg("站点文件类型不正确!");
            else if (!FileNameFilter.CheckFileExistence(path1))
                Msg("站点文件路径不存在!");
            else if (!FileNameFilter.CheckFileSuffix(path2))
                Msg("内区地形文件类型不正确!");
            else if (!FileNameFilter.CheckFileExistence(path2))
                Msg("内区地形文件路径不存在!");
            else if (!FileNameFilter.CheckFileSuffix(path3))
                Msg("外区地形文件类型不正确!");
            else if (!FileNameFilter.CheckFileExistence(path3))
                Msg("外区地形文件路径不存在!");
            else if (!double.TryParse(arg1.Value, out _arg1))
                Msg("密度值非法!");
            else if (!double.TryParse(arg2.Value, out _arg2))
                Msg("内区半径非法!");
            else if (!double.TryParse(arg3.Value, out _arg3))
                Msg("外区半径非法!");
            else
            {
                _arg1 *= double.Parse((arg1.SelectedItem as ComboBoxItem).Tag.ToString());
                _arg2 *= double.Parse((arg2.SelectedItem as ComboBoxItem).Tag.ToString());
                _arg3 *= double.Parse((arg3.SelectedItem as ComboBoxItem).Tag.ToString());
                Task_bggz = BouguerCorrection.Start(path1, path2, path3, _arg1, _arg2, _arg3);
                await Task_bggz;
                string s = Task_bggz.Result;
                Msg("计算完毕!");
            }
            loadingBar.Hide();
        }

        private Task<string> Task_bggz;

        private void Msg(string msg)
        {
            Dispatcher.Invoke(delegate { MessageBox.Show(Application.Current.MainWindow, msg); });
        }
        //private void Button_Click_1(object sender, RoutedEventArgs e)
        //{
        //    string unit = ((ComboBoxItem)midu.SelectedItem).Content.ToString();
        //    string Converter = ((ComboBoxItem)midu.SelectedItem).Tag.ToString();
        //    string value;
        //    if(midu.Value!=null)
        //    {
        //        value = midu.Value.ToString();
        //    }
        //    else
        //    {
        //        value = "null";
        //    }
        //    MessageBox.Show("值：" + value + "\n" + "单位:" + unit + "\n" + "转换:" + Converter);
        //}
    }
}
