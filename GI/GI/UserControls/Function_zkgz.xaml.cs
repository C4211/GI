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
                HidePrevAndCancel();
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
                    loadingBar.Hide();
                    ShowPrevAndCancel();
                }
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
        private async void DoFreeAirCorrection()
        {
            string inPath = inputPath1.filePath.Text;
            string outPath = outputPath1.filePath.Text;
            int choice = 1;
            HidePrevAndCancel();
            loadingBar.Show();
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
                    if (Task_zkgz.Result != null)
                    {
                        loadingBar.Hide();
                        ShowPrevAndCancel();
                        Msg(Task_zkgz.Result);
                    }
                    else if (IsCanceled)
                    {
                        loadingBar.Hide();
                        ShowPrevAndCancel();
                        Msg("计算取消!");
                    }
                    else
                    {
                        loadingBar.Hide();
                        ShowPrevAndCancel();
                        Msg("计算完成！");
                    }
                }
                catch (Exception e)
                {
                    //Msg(e.Message);
                    MessageBox.Show(e.Message);
                }
                finally
                {
                    Task_zkgz = null;
                }
            }
            ShowPrevAndCancel();
            loadingBar.Hide();
            Dispatcher.Invoke(delegate
            {
                CurrentState = MaxState - 1;
                next.Content = "计算";
            });
        }

        private Task<string> Task_zkgz = null;

        private void Msg(string msg)
        {
            Dispatcher.Invoke(delegate { MessageWindow.Show(Application.Current.MainWindow, msg); });
        }
    }
}
