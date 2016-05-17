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
        /// <summary>
        /// 构造函数，设置中英文标题
        /// </summary>
        public Function_zkgz()
        {
            InitializeComponent();
            this.titleCn = "自空改正";
            this.titleEn = "Free Air Correction";
        }

        /// <summary>
        /// 计算状态编号
        /// 0 : 输入文件
        /// 1 : 输入参数
        /// 2 : 计算中
        /// </summary>
        private int CurrentState = 0;
        /// <summary>
        /// 结束状态编号
        /// </summary>
        private int MaxState { get { return content.Children.Count; } }
        /// <summary>
        /// 是否已经取消计算
        /// </summary>
        private bool IsCanceled = false;

        /// <summary>
        /// 点击下一步
        /// </summary>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            #region 逻辑
            if (CurrentState == 0)
            {
                // 输入文件路径判断
                string inPath = inputPath1.filePath.Text;
                if (inPath.Trim() == "")
                {
                    Msg("未选择文件！");
                    return;
                }
                else if (!FileNameFilter.CheckFileSuffix(inPath))
                {
                    Msg("输入文件类型不正确！");
                    return;
                }
                else if (!File.Exists(inPath))
                {
                    Msg("输入文件路径不存在！");
                    return;
                }
            }
            #endregion

            #region 界面
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
                //prev.Visibility = Visibility.Visible;
                ButtonShow(prev);
                if (CurrentState == MaxState - 1)
                    //next.Content = "计算";
                    ButtonChangeContent(next, "计算");
                return;
            }
            else if (CurrentState == MaxState - 1)
            {
                DoFreeAirCorrection();
            }
            else if (CurrentState == MaxState)
            {
                if (Task_zkgz != null)
                {
                    IsCanceled = true;
                    FreeAirCorrection.Stop();
                    ShowPrevAndCancel();
                    loadingBar.Hide();
                    Msg("计算取消！");
                }
                CurrentState = MaxState - 1;
                //next.Content = "计算";
                ButtonChangeContent(next, "计算");
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
                        File.Copy(FreeAirCorrection.outPath, ofd.FileName, true);
                        Msg("已保存！");
                        CloseAndBackConfirm.Reset();
                    }
                    catch
                    {
                        Msg("保存失败！");
                    }
                }
            }
            #endregion
        }

        /// <summary>
        /// 隐藏上一步和取消按钮
        /// </summary>
        private void HidePrevAndCancel()
        {
            Dispatcher.Invoke(delegate
            {
                //prev.Visibility = Visibility.Hidden;
                ButtonHide(prev);
                ButtonHide(cancel);
                //cancel.Visibility = Visibility.Hidden;
            });
        }
        /// <summary>
        /// 显示上一步和取消按钮
        /// </summary>
        private void ShowPrevAndCancel()
        {
            Dispatcher.Invoke(delegate
            {
                //prev.Visibility = Visibility.Visible;
                ButtonShow(prev);
                //cancel.Visibility = Visibility.Visible;
                ButtonShow(cancel);
            });
        }
        /// <summary>
        /// 点击上一步
        /// </summary>
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
                    //prev.Visibility = Visibility.Hidden;
                    ButtonHide(prev);
                //next.Content = "下一步";
                ButtonChangeContent(next, "下一步");
            }
            else if (CurrentState == MaxState + 1)
            {
                FileInfo fi = new FileInfo(FreeAirCorrection.outPath);
                FilePreviewWindow.PreviewShow(Application.Current.MainWindow, fi);
            }
        }
        /// <summary>
        /// 开始计算
        /// </summary>
        private async void DoFreeAirCorrection()
        {
            if (loadingBar.Show("计算中"))
            {
                CloseAndBackConfirm.Set(CloseAndBackConfirm.States.计算正在进行中);
                CurrentState = MaxState;
                IsCanceled = false;
                //next.Content = "取消";
                ButtonChangeContent(next, "取消");
                string inPath = inputPath1.filePath.Text;
                int choice = 1;
                HidePrevAndCancel();

                if (!FileNameFilter.CheckFileSuffix(inPath))
                    Msg("输入文件类型不正确！");
                else if (!File.Exists(inPath))
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
                        if (Task_zkgz.Result != null)
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
                        CloseAndBackConfirm.Reset();
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
                    //next.Content = "计算";
                    ButtonChangeContent(next, "计算");
                });
                CloseAndBackConfirm.Reset();
            }
        }

        /// <summary>
        /// 计算任务
        /// </summary>
        private Task<string> Task_zkgz = null;

        /// <summary>
        /// 弹出通知
        /// </summary>
        private void Msg(string msg)
        {
            Dispatcher.Invoke(delegate { MessageWindow.Show(Application.Current.MainWindow, msg); });
        }

        /// <summary>
        /// 点击返回
        /// </summary>
        private void back_Click(object sender, RoutedEventArgs e)
        {
            if (CloseAndBackConfirm.Start(CloseAndBackConfirm.Actions.返回))
            {
                loadingBar.Hide();
                //cancel.Visibility = Visibility.Visible;
                ButtonShow(cancel);
                //back.Visibility = Visibility.Collapsed;
                ButtonRemove(back);
                //prev.Content = "上一步";
                ButtonChangeContent(prev, "上一步");
                //prev.Visibility = Visibility.Visible;
                ButtonShow(prev);
                //next.Content = "计算";
                ButtonChangeContent(next, "计算");
                //next.Visibility = Visibility.Visible;
                ButtonShow(next);
                CurrentState = MaxState - 1;
            }
        }

        /// <summary>
        /// 计算完成
        /// </summary>
        private void Completed()
        {
            loadingBar.changeState("计算完成", false);
            CurrentState = MaxState + 1;
            //prev.Content = "预览";
            ButtonChangeContent(prev, "预览");
            //prev.Visibility = Visibility.Visible;
            ButtonShow(prev);
            //next.Content = "保存";
            ButtonChangeContent(next, "保存");
            //next.Visibility = Visibility.Visible;
            ButtonShow(next);
            //cancel.Visibility = Visibility.Collapsed;
            ButtonRemove(cancel);
            //back.Visibility = Visibility.Visible;
            ButtonShow(back);
            CloseAndBackConfirm.Set(CloseAndBackConfirm.States.计算结果未保存);
        }

        /// <summary>
        /// 改变按钮文字
        /// </summary>
        private void ButtonChangeContent(Button sender, string content)
        {
            Storyboard sbhide = (Application.Current.FindResource("GI.Body.Button.Content.Hide") as Storyboard).Clone();
            Storyboard sbshow = (Application.Current.FindResource("GI.Body.Button.Content.Show") as Storyboard).Clone();
            sbhide.Completed += delegate { sender.Content = content; sender.BeginStoryboard(sbshow); };
            sender.BeginStoryboard(sbhide);
        }

        /// <summary>
        /// 显示按钮
        /// </summary>
        private void ButtonShow(Button sender)
        {
            Storyboard sb = (Application.Current.FindResource("GI.Body.Button.Show") as Storyboard).Clone();
            sender.BeginStoryboard(sb);
        }

        /// <summary>
        /// 隐藏按钮
        /// </summary>
        private void ButtonHide(Button sender)
        {
            Storyboard sb = (Application.Current.FindResource("GI.Body.Button.Hide") as Storyboard).Clone();
            sender.BeginStoryboard(sb);
        }

        /// <summary>
        /// 移除按钮
        /// </summary>
        private void ButtonRemove(Button sender)
        {
            Storyboard sb = (Application.Current.FindResource("GI.Body.Button.Remove") as Storyboard).Clone();
            sender.BeginStoryboard(sb);
        }
    }
}
