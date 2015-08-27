using GI.Functions;
using GI.Tools;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace GI.UserControls
{
    /// <summary>
    /// Function_fxyf.xaml 的交互逻辑
    /// </summary>
    public partial class Function_zzhlb : FunctionPage
    {
        public Function_zzhlb()
        {
            InitializeComponent();
            this.titleCn = "正则化滤波";
            this.titleEn = "Regularization Filter";
        }

        /// <summary>
        /// 0 : 输入文件
        /// 1 : 输入参数
        /// 2 : 计算中
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
                DoRagularizationFilter();
            }
            else if (CurrentState == MaxState)
            {
                CurrentState = MaxState - 1;
                next.Content = "计算";
                if (task != null)
                {
                    IsCanceled = true;
                    RegularizationFilter.p.Kill();
                    loadingBar.Hide();
                    ShowPrevAndCancel();
                }
            }
        }

        private void prev_Click(object sender, RoutedEventArgs e)
        {

            if (CurrentState > 0)
            {
                content.IsEnabled = false; buttons.IsEnabled = false;
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

        private async void DoRagularizationFilter()
        {
            HidePrevAndCancel();
            loadingBar.Show();
            string path1 = inputPath1.filePath.Text;
            string outPath = outputPath1.filePath.Text;
            double _a1, _b1, _f01, _a2, _b2, _f02;
            int choice = 0;
            if (!path1.Trim().EndsWith(".grd", StringComparison.OrdinalIgnoreCase))
                Msg("输入文件类型不正确！");
            else if (!File.Exists(path1))
                Msg("输入文件路径不存在！");
            else if (FileNameFilter.CheckGRDFileFormat(path1) == null)
                Msg("输入文件不是GRD数据格式！");
            else if (!double.TryParse(a1.Text, out _a1))
                Msg("α1非法！");
            else if (!double.TryParse(b1.Text, out _b1))
                Msg("β1非法！");
            else if (!double.TryParse(f01.Text, out _f01))
                Msg("f01非法！");
            else if (!double.TryParse(a2.Text, out _a2))
                Msg("α2非法！");
            else if (!double.TryParse(b2.Text, out _b2))
                Msg("β2非法！");
            else if (!double.TryParse(f02.Text, out _f02))
                Msg("f02非法！");
            else
            {
                try
                {
                    if (choice0.IsChecked == true)
                        choice = 0;
                    else if (choice1.IsChecked == true)
                        choice = 1;
                    else if (choice2.IsChecked == true)
                        choice = 2;
                    task = RegularizationFilter.Start(path1, _a1, _b1, _f01, _a2, _b2, _f02, choice);
                    await task;
                    if (IsCanceled)
                    {
                        loadingBar.Hide();
                        ShowPrevAndCancel();
                        Msg("计算取消!");
                    }
                    else
                    {
                        File.Copy(RegularizationFilter.outPath, outPath, true);
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
                    task = null;
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

        private Task<string> task = null;

        private void Msg(string msg)
        {
            Dispatcher.Invoke(delegate { MessageWindow.Show(Application.Current.MainWindow, msg); });
        }

        private void param_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            double tmp;
            e.Handled = double.TryParse(e.Text, out tmp);
        }
    }
}
