using GI.Functions;
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
    public partial class Function_gsjff : FunctionPage
    {
        public Function_gsjff()
        {
            InitializeComponent();
            this.titleCn = "高斯积分法";
            this.titleEn = "TC Gauss";
        }
        /// <summary>
        /// 0 : 输入文件
        /// 1 : 输入参数
        /// 2 : 选择区域
        /// 3 : 计算中
        /// </summary>
        private int CurrentState = 0;
        private int MaxState { get { return content.Children.Count; } }
        private bool IsCanceled = false;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentState < MaxState - 1)
            {
                content.IsEnabled = false;
                buttons.IsEnabled = false;
                CurrentState += 1;
                content.Children[CurrentState].Visibility = Visibility.Visible;
                Storyboard sb = ((Storyboard)this.FindResource("sb")).Clone();
                ((ThicknessAnimation)sb.Children[0]).To = new Thickness(-CurrentState * 680, 0, 0, 0);
                sb.Completed += delegate { content.Children[CurrentState - 1].Visibility = Visibility.Hidden; content.IsEnabled = true; buttons.IsEnabled = true; };
                content.BeginStoryboard(sb);
                prev.Visibility = Visibility.Visible;
                if (CurrentState == MaxState - 1)
                    next.Content = "计算";
                return;
            }
            else if (CurrentState == MaxState - 1)
            {
                CurrentState = MaxState;
                IsCanceled = false;
                next.Content = "取消";
                HidePrevAndCancel();
                DoGSJFF();
            }
            else if (CurrentState == MaxState)
            {
                CurrentState = MaxState-1;
                next.Content = "计算";
                if (Task_gsjff != null)
                {
                    IsCanceled = true;
                    GSJFF.p.Kill();
                    loadingBar.Hide();
                    ShowPrevAndCancel();
                }
            }
        }

        private void prev_Click(object sender, RoutedEventArgs e)
        {

            if (CurrentState > 0)
            {
                content.IsEnabled = false;buttons.IsEnabled = false;
                CurrentState -= 1;
                content.Children[CurrentState].Visibility = Visibility.Visible;
                Storyboard sb = ((Storyboard)this.FindResource("sb")).Clone();
                ((ThicknessAnimation)sb.Children[0]).To = new Thickness(-CurrentState * 680, 0, 0, 0);
                sb.Completed += delegate { content.Children[CurrentState + 1].Visibility = Visibility.Hidden; content.IsEnabled = true; buttons.IsEnabled = true; };
                content.BeginStoryboard(sb);
                if (CurrentState <= 0)
                    prev.Visibility = Visibility.Hidden;
                next.Content = "下一步";
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

        private async void DoGSJFF()
        {
            HidePrevAndCancel();
            loadingBar.Show();
            string path1 = inputPath1.filePath.Text;
            string path2 = inputPath2.filePath.Text;
            string path3 = inputPath3.filePath.Text;
            string outPath = outputPath1.filePath.Text;
            double _arg1, _arg2, _arg3;
            int choice = 1;
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
                    if (choice1.IsChecked == true)
                        choice = 1;
                    else if (choice2.IsChecked == true)
                        choice = 0;
                    _arg1 *= double.Parse((arg1.SelectedItem as ComboBoxItem).Tag.ToString());
                    _arg2 *= double.Parse((arg2.SelectedItem as ComboBoxItem).Tag.ToString());
                    _arg3 *= double.Parse((arg3.SelectedItem as ComboBoxItem).Tag.ToString());
                    Task_gsjff = GSJFF.Start(path1, path2, path3, _arg1, _arg2, _arg3, choice);
                    await Task_gsjff;
                    if (IsCanceled)
                    {
                        loadingBar.Hide();
                        ShowPrevAndCancel();
                        Msg("计算取消!");
                    }
                    else
                    {
                        File.Copy(GSJFF.outPath, outPath, true);
                        loadingBar.Hide();
                        ShowPrevAndCancel();
                        Msg("计算完成");
                    }
                }
                catch (Exception e)
                {
                    Msg(e.Message);
                }
                finally
                {
                    Task_gsjff = null;
                }
            }
            loadingBar.Hide();
            ShowPrevAndCancel();
            Dispatcher.Invoke(delegate
            {
                CurrentState = MaxState - 1;
                next.Content = "计算";
            });
        }

        private Task<string> Task_gsjff = null;

        private void Msg(string msg)
        {
            Dispatcher.Invoke(delegate { MessageWindow.Show(Application.Current.MainWindow, msg); });
        }
    }
}
