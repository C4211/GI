using GI.Functions;
using GI.Tools;
using System;
using System.Collections.Generic;
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
    public partial class Function_zkgz : FunctionPage
    {
        public Function_zkgz()
        {
            InitializeComponent();
            this.titleCn = "自空改正";
            this.titleEn = "Free Air Correction";
        }

        /// <summary>
        /// 0 : 输入文件
        /// 1 : 输入参数
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
                DoFreeAirCorrection();
            }
            else if (CurrentState == MaxState)
            {
                CurrentState = MaxState - 1;
                next.Content = "计算";
                if (Task_zkgz != null)
                {
                    IsCanceled = true;
                    FreeAirCorrection.Stop();
                    ShowPrevAndCancel();
                    loadingBar.Hide();
                    Msg("计算取消！");
                }
            }
            else if (CurrentState == MaxState + 1)
            {
                Msg("暂未添加该功能");
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
        private void prev_Click(object sender, RoutedEventArgs e)
        {

            if (CurrentState > 0 && CurrentState<=MaxState )
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
            if (CurrentState == MaxState + 1)
            {
                Msg("暂未添加该功能");
            }
        }
        private async void DoFreeAirCorrection()
        {
            string inPath = inputPath1.filePath.Text;
            string outPath = outputPath1.filePath.Text;
            int choice = 1;
            HidePrevAndCancel();
            loadingBar.Show("计算中");
            if (!FileNameFilter.CheckFileSuffix(inPath))
                Msg("输入文件类型不正确！");
            else if (!FileNameFilter.CheckFileExistence(inPath))
                Msg("输入文件路径不存在！");
            else
            {
                try
                {
                    if (choice1.IsChecked == true)
                        choice = 1;
                    else if (choice2.IsChecked == true)
                        choice = 2;
                    else if (choice3.IsChecked == true)
                        choice = 3;
                    else if (choice4.IsChecked == true)
                        choice = 4;
                    Task_zkgz = null;
                    Task_zkgz = FreeAirCorrection.Start(inPath, outPath, choice);
                    await Task_zkgz;
                    
                }
                catch (Exception e)
                {
                    Msg(e.Message);
                }
                finally
                {
                    Task_zkgz = null;
                }
            }
            if (!IsCanceled)
            {
                Completed();
                return;
            }
            ShowPrevAndCancel();
            loadingBar.Hide();
            Dispatcher.Invoke(delegate
            {
                CurrentState = MaxState - 1;
                next.Content = "计算";
            });
        }

        private void Completed()
        {
            loadingBar.changeState("计算完成", false);
            CurrentState = MaxState + 1;
            prev.Content = "预览";
            prev.Visibility = Visibility.Visible;
            next.Content = "保存";
            next.Visibility = Visibility.Visible;
            cancel.Visibility = Visibility.Collapsed;
            back.Visibility = Visibility.Visible;
            
        }

        private Task Task_zkgz = null;

        private void Msg(string msg)
        {
            Dispatcher.Invoke(delegate { MessageWindow.Show(Application.Current.MainWindow, msg); });
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            Task_zkgz = null;
            loadingBar.Hide();
            cancel.Visibility = Visibility.Visible;
            back.Visibility = Visibility.Collapsed;
            prev.Content = "上一步";
            prev.Visibility = Visibility.Visible;
            next.Content = "计算";
            next.Visibility = Visibility.Visible;
            CurrentState = MaxState - 1;
        }
    }
}
