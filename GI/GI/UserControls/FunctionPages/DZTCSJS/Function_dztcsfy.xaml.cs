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
    public partial class Function_dztcsfy : FunctionPage
    {
        /// <summary>
        /// 构造函数，设置中英文标题
        /// </summary>
        public Function_dztcsfy()
        {
            InitializeComponent();
            this.titleCn = "地质体参数反演";
            this.titleEn = "Geological Parameters Inversion";
            cssz = content.Children[MaxState - 1] as Grid;
        }

        /// <summary>
        /// 计算状态编号 
        /// 0 : 选项
        /// 1 : 输入文件
        /// 2 : choice1-3=计算中 choice4=输入参数
        /// 3 : choice4=计算中
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
        /// 计算方法选项为4时新增的一页控件
        /// </summary>
        private Grid cssz;

        /// <summary>
        /// 计算方法选项
        /// </summary>
        private int choice;
        /// <summary>
        /// 输入文件路径
        /// </summary>
        private string inPath1, inPath2;
        /// <summary>
        /// 计算参数
        /// </summary>
        private double arg1, arg2;

        /// <summary>
        /// 重置第二页控件为隐藏状态
        /// </summary>
        private void Page2_Reset()
        {
            Page1choice1.Visibility = Visibility.Hidden;
            Page1choice2.Visibility = Visibility.Hidden;
            Page1choice3.Visibility = Visibility.Hidden;
            Page1choice4.Visibility = Visibility.Hidden;
        }
        /// <summary>
        /// 点击下一步
        /// </summary>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            #region 校验
            if (CurrentState == 0)
            {
                if (choice1.IsChecked == true)
                    choice = 0;
                else if (choice2.IsChecked == true)
                    choice = 1;
                else if (choice3.IsChecked == true)
                    choice = 2;
                else
                    choice = 3;
            }
            else if (CurrentState == 1)
            {
                switch (choice)
                {
                    case 0:
                        inPath1 = choice1path1.filePath.Text;
                        if (inPath1.Trim() == "")
                        {
                            Msg("输入文件不存在！");
                            return;
                        }
                        else if (!FileNameFilter.CheckFileSuffix(inPath1))
                        {
                            Msg("重力异常数据文件类型不正确！");
                            return;
                        }
                        else if (!File.Exists(inPath1))
                        {
                            Msg("重力异常数据路径不存在！");
                            return;
                        }
                        break;
                    case 1:
                        inPath1 = choice2path1.filePath.Text;
                        if (inPath1.Trim() == "")
                        {
                            Msg("输入文件不存在！");
                            return;
                        }
                        else if (!FileNameFilter.CheckFileSuffix(inPath1))
                        {
                            Msg("重力异常数据文件类型不正确！");
                            return;
                        }
                        else if (!File.Exists(inPath1))
                        {
                            Msg("重力异常数据路径不存在！");
                            return;
                        }
                        break;
                    case 2:
                        inPath1 = choice3path1.filePath.Text;
                        inPath2 = choice3path2.filePath.Text;
                        if (inPath1.Trim() == "" || inPath2.Trim() == "")
                        {
                            Msg("输入文件不存在！");
                            return;
                        }
                        else if (!FileNameFilter.CheckFileSuffix(inPath1))
                        {
                            Msg("重力异常数据文件类型不正确！");
                            return;
                        }
                        else if (!File.Exists(inPath1))
                        {
                            Msg("重力异常数据路径不存在！");
                            return;
                        }
                        if (!FileNameFilter.CheckFileSuffix(inPath2))
                        {
                            Msg("重力梯度异常数据文件类型不正确！");
                            return;
                        }
                        else if (!File.Exists(inPath2))
                        {
                            Msg("重力梯度异常数据路径不存在！");
                            return;
                        }
                        break;
                    case 3:
                        inPath1 = choice4path1.filePath.Text;
                        inPath2 = choice4path2.filePath.Text;
                        if (inPath1.Trim() == "" || inPath2.Trim() == "")
                        {
                            Msg("输入文件不存在！");
                            return;
                        }
                        else if (!FileNameFilter.CheckFileSuffix(inPath1))
                        {
                            Msg("重力异常数据文件类型不正确！");
                            return;
                        }
                        else if (!File.Exists(inPath1))
                        {
                            Msg("重力异常数据路径不存在！");
                            return;
                        }
                        if (!FileNameFilter.CheckFileSuffix(inPath2))
                        {
                            Msg("重力梯度异常数据文件类型不正确！");
                            return;
                        }
                        else if (!File.Exists(inPath2))
                        {
                            Msg("重力梯度异常数据路径不存在！");
                            return;
                        }
                        break;
                    default:
                        Msg("选项异常！请尝试重新选择");
                        return;
                }

            }
            else if (CurrentState == 2)
            {
                if (choice == 3)
                {
                    if (!double.TryParse(choice4arg1.Text, out arg1))
                    {
                        Msg("重力异常相对误差不合法！");
                        return;
                    }
                    else if (!double.TryParse(choice4arg2.Text, out arg2))
                    {
                        Msg("重力梯度异常相对误差不合法！");
                        return;
                    }
                }
            }
            #endregion

            #region 界面
            Page2_Reset();
            if (choice1.IsChecked == true)
            {
                Page1choice1.Visibility = Visibility.Visible;
            }
            else if (choice2.IsChecked == true)
            {
                Page1choice2.Visibility = Visibility.Visible;
            }
            else if (choice3.IsChecked == true)
            {
                Page1choice3.Visibility = Visibility.Visible;
            }
            else if (choice4.IsChecked == true)
            {
                Page1choice4.Visibility = Visibility.Visible;
            }
            if (choice4.IsChecked == false && MaxState == 3)
            {
                content.Children.Remove(content.Children[MaxState - 1]);
            }
            else if (choice4.IsChecked == true && MaxState == 2)
            {
                content.Children.Add(cssz);
            }
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
                // 开始计算
                DoGeoBackward();
            }
            else if (CurrentState == MaxState)
            {
                CurrentState = MaxState - 1;
                next.Content = "计算";
                // 取消计算
                CancelGeoBackward();
            }
            else if (CurrentState == MaxState + 1)
            {
                System.Windows.Forms.SaveFileDialog ofd = new System.Windows.Forms.SaveFileDialog();
                ofd.Filter = "txt文件(*.txt)|*.txt|dat文件(*.dat)|*.dat";
                ofd.FilterIndex = 1;
                if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    try
                    {
                        File.Copy(GeoBackward.outPath, ofd.FileName, true);
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
        /// 开始计算
        /// </summary>
        private async void DoGeoBackward()
        {
            if (loadingBar.Show("计算中"))
            {
                CloseAndBackConfirm.Set(CloseAndBackConfirm.States.计算正在进行中);
                CurrentState = MaxState;
                IsCanceled = false;
                next.Content = "取消";
                HidePrevAndCancel();
                {
                    try
                    {
                        if (choice == 0 || choice == 1)
                            task = GeoBackward.Start(choice, inPath1);
                        else if (choice == 2)
                            task = GeoBackward.Start(choice, inPath1, inPath2);
                        else
                            task = GeoBackward.Start(choice, inPath1, inPath2, arg1, arg2);

                        await task;
                        if (IsCanceled)
                        {
                            loadingBar.Hide();
                            ShowPrevAndCancel();
                            Msg("计算取消!");
                        }
                        else if (task.Result != "")
                        {
                            Msg(task.Result);
                            loadingBar.Hide();
                            ShowPrevAndCancel();
                        }
                        else
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
                CloseAndBackConfirm.Reset();
            }
        }

        /// <summary>
        /// 取消计算
        /// </summary>
        private void CancelGeoBackward()
        {
            if (task != null)
            {
                IsCanceled = true;
                GeoBackward.p.Kill();
                loadingBar.Hide();
                ShowPrevAndCancel();
            }
        }

        /// <summary>
        /// 计算任务
        /// </summary>
        private Task<string> task = null;

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
                    prev.Visibility = Visibility.Hidden;
                next.Content = "下一步";
            }
            else if (CurrentState == MaxState + 1)
            {
                FileInfo fi = new FileInfo(GeoBackward.outPath);
                FilePreviewWindow.PreviewShow(Application.Current.MainWindow, fi);
            }
        }

        /// <summary>
        /// 隐藏上一步和取消按钮
        /// </summary>
        private void HidePrevAndCancel()
        {
            Dispatcher.Invoke(delegate
            {
                prev.Visibility = Visibility.Hidden;
                cancel.Visibility = Visibility.Hidden;
            });
        }
        /// <summary>
        /// 显示上一步和取消按钮
        /// </summary>
        private void ShowPrevAndCancel()
        {
            Dispatcher.Invoke(delegate
            {
                prev.Visibility = Visibility.Visible;
                cancel.Visibility = Visibility.Visible;
            });
        }

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
                cancel.Visibility = Visibility.Visible;
                back.Visibility = Visibility.Collapsed;
                prev.Content = "上一步";
                prev.Visibility = Visibility.Visible;
                next.Content = "计算";
                next.Visibility = Visibility.Visible;
                CurrentState = MaxState - 1;
            }
        }

        /// <summary>
        /// 计算完成
        /// </summary>
        private void Completed()
        {
            CloseAndBackConfirm.Set(CloseAndBackConfirm.States.计算结果未保存);
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
