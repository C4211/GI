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
                System.Windows.Forms.SaveFileDialog ofd = new System.Windows.Forms.SaveFileDialog();
                ofd.Filter = "txt文件(*.txt)|*.txt|dat文件(*.dat)|*.dat";
                ofd.FilterIndex = 2;
                if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    try
                    {
                        File.Copy(@"out.DAT", ofd.FileName, true);
                        Msg("已保存！");
                    }
                    catch
                    {
                        Msg("保存失败！");
                    }
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

            if (CurrentState > 0 && CurrentState <= MaxState)
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
            else if (CurrentState == MaxState + 1)
            {
                FileInfo fi = new FileInfo(FreeAirCorrection.outPath);
                FilePreviewWindow.PreviwShow(Application.Current.MainWindow, fi);
            }
        }
        private async void DoFreeAirCorrection()
        {
            if (loadingBar.Show("计算中"))
            {
                CurrentState = MaxState;
                IsCanceled = false;
                next.Content = "取消";
                string inPath = inputPath1.filePath.Text;
                int choice = 1;
                HidePrevAndCancel();

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
                        Task_zkgz = FreeAirCorrection.Start(inPath, choice);
                        await Task_zkgz;
                        if (Task_zkgz.Result!=null)
                        {
                            Msg(Task_zkgz.Result);
                        }
                        else if (!IsCanceled)
                        {
                            Completed();
                            return;
                        }
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
                ShowPrevAndCancel();
                loadingBar.Hide();
                Dispatcher.Invoke(delegate
                {
                    CurrentState = MaxState - 1;
                    next.Content = "计算";
                });
            }
        }

        private Task<string> Task_zkgz = null;

        private void Msg(string msg)
        {
            Dispatcher.Invoke(delegate { MessageWindow.Show(Application.Current.MainWindow, msg); });
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            loadingBar.Hide();
            cancel.Visibility = Visibility.Visible;
            back.Visibility = Visibility.Collapsed;
            prev.Content = "上一步";
            prev.Visibility = Visibility.Visible;
            next.Content = "计算";
            next.Visibility = Visibility.Visible;
            CurrentState = MaxState - 1;
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
    }
}
