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

        /// <summary>
        /// 0 : 输入文件
        /// 1 : 输入参数
        /// 2 : 计算中
        /// </summary>
        private int CurrentState = 0;
        private bool IsCanceled = false;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentState == 0)
            {
                next.IsEnabled = false;
                CurrentState = 1;
                content.Children[CurrentState].Visibility = Visibility.Visible;
                Storyboard sb = ((Storyboard)this.FindResource("sb")).Clone();
                ((ThicknessAnimation)sb.Children[0]).To = new Thickness(-CurrentState * 680, 0, 0, 0);
                sb.Completed += delegate { content.Children[CurrentState - 1].Visibility = Visibility.Hidden; next.IsEnabled = true; };
                content.BeginStoryboard(sb);
                prev.Visibility = Visibility.Visible;
                next.Content = "计算";
                return;
            }
            else if (CurrentState == 1)
            {
                CurrentState = 2;
                IsCanceled = false;
                next.Content = "取消";
                DoBouguerCorrection();
            }
            else if (CurrentState == 2)
            {
                CurrentState = 1;
                next.Content = "计算";
                if (Task_bggz != null)
                {
                    IsCanceled = true;
                    BouguerCorrection.p.Kill();
                    loadingBar.Hide();
                    ShowPrevAndCancel();
                }
            }
        }

        private void prev_Click(object sender, RoutedEventArgs e)
        {

            if (CurrentState > 0)
            {
                next.IsEnabled = false;
                CurrentState -= 1;
                content.Children[CurrentState].Visibility = Visibility.Visible;
                Storyboard sb = ((Storyboard)this.FindResource("sb")).Clone();
                ((ThicknessAnimation)sb.Children[0]).To = new Thickness(-CurrentState * 680, 0, 0, 0);
                sb.Completed += delegate { content.Children[CurrentState + 1].Visibility = Visibility.Hidden; next.IsEnabled = true; };
                content.BeginStoryboard(sb);
                if (CurrentState <= 0)
                    prev.Visibility = Visibility.Hidden;
                next.Content = "下一步";
                next.Visibility = Visibility.Visible;
            }
        }
        private void HidePrevAndCancel()
        {
            Dispatcher.Invoke(delegate
            {
                prev.Visibility = Visibility.Hidden;
                cancel.Visibility = Visibility.Hidden;
            });
        }
        private void ShowPrevAndCancel()
        {
            Dispatcher.Invoke(delegate
            {
                prev.Visibility = Visibility.Visible;
                cancel.Visibility = Visibility.Visible;
            });
        }

        private async void DoBouguerCorrection()
        {
            HidePrevAndCancel();
            loadingBar.Show();
            string path1 = inputPath1.filePath.Text;
            string path2 = inputPath2.filePath.Text;
            string path3 = inputPath3.filePath.Text;
            string outPath = outputPath1.filePath.Text;
            double _arg1, _arg2, _arg3;
            if (!FileNameFilter.CheckFileSuffix(path1))
                Msg("站点文件类型不正确！");
            else if (!FileNameFilter.CheckFileExistence(path1))
                Msg("站点文件路径不存在！");
            else if (!FileNameFilter.CheckFileSuffix(path2))
                Msg("内区地形文件类型不正确！");
            else if (!FileNameFilter.CheckFileExistence(path2))
                Msg("内区地形文件路径不存在！");
            else if (!FileNameFilter.CheckFileSuffix(path3))
                Msg("外区地形文件类型不正确！");
            else if (!FileNameFilter.CheckFileExistence(path3))
                Msg("外区地形文件路径不存在！");
            else if (!double.TryParse(arg1.Value, out _arg1))
                Msg("密度值非法！");
            else if (!double.TryParse(arg2.Value, out _arg2))
                Msg("内区半径非法！");
            else if (!double.TryParse(arg3.Value, out _arg3))
                Msg("外区半径非法！");
            else
            {
                try
                {
                    _arg1 *= double.Parse((arg1.SelectedItem as ComboBoxItem).Tag.ToString());
                    _arg2 *= double.Parse((arg2.SelectedItem as ComboBoxItem).Tag.ToString());
                    _arg3 *= double.Parse((arg3.SelectedItem as ComboBoxItem).Tag.ToString());
                    Task_bggz = BouguerCorrection.Start(path1, path2, path3, _arg1, _arg2, _arg3);
                    await Task_bggz;
                    if (IsCanceled)
                    {
                        loadingBar.Hide();
                        ShowPrevAndCancel();
                        Msg("计算取消!");
                    }
                    else
                    {
                        File.Copy(@"out.DAT", outPath, true);
                        loadingBar.Hide();
                        ShowPrevAndCancel();
                        Msg(Task_bggz.Result);
                    }
                }
                catch (Exception e)
                {
                    Msg(e.Message);
                }
                finally
                {
                    Task_bggz = null;
                }
            }
            Dispatcher.Invoke(delegate
            {
                CurrentState = 1;
                next.Content = "计算";
            });
            loadingBar.Hide();
            ShowPrevAndCancel();
        }

        private Task<string> Task_bggz = null;

        private void Msg(string msg)
        {
            Dispatcher.Invoke(delegate { MessageWindow.Show(Application.Current.MainWindow, msg); });
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
