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
    public partial class Function_pplb : FunctionPage
    {
        /// <summary>
        /// 构造函数，设置中英文标题
        /// </summary>
        public Function_pplb()
        {
            InitializeComponent();
            this.titleCn = "匹配滤波";
            this.titleEn = "Matching Filter";
        }

        /// <summary>
        /// 计算状态编号
        /// 0 : 输入文件
        /// 1 : 输入参数
        /// 2 : 选择场
        /// 3 : 计算中
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
                // 检查输入文件路径
                string inPath = inputPath1.filePath.Text;
                if (!inPath.Trim().EndsWith(".grd", StringComparison.OrdinalIgnoreCase))
                {
                    Msg("输入文件类型不正确！");
                    return;
                }
                else if (!File.Exists(inPath))
                {
                    Msg("输入文件路径不存在！");
                    return;
                }
                else if (FileNameFilter.CheckGRDFileFormat(inPath) == null)
                {
                    Msg("输入文件不是GRD数据格式！");
                    return;
                }
            }
            else if (CurrentState == 1)
            {
                // 检查参数
                double _arg0, _arg1, _arg2;
                if (!double.TryParse(arg0.Value, out _arg0))
                {
                    Msg("深源埋深非法！");
                    return;
                }
                else if (!double.TryParse(arg1.Value, out _arg1))
                {
                    Msg("浅源埋深非法！");
                    return;
                }
                else if (!double.TryParse(arg2.Value, out _arg2))
                {
                    Msg("纵轴截距非法！");
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
                prev.Visibility = Visibility.Visible;
                if (CurrentState == MaxState - 1)
                    next.Content = "计算";
                return;
            }
            else if (CurrentState == MaxState - 1)
            {
                DoMatchingFilter();
            }
            else if (CurrentState == MaxState)
            {
                CurrentState = MaxState - 1;
                next.Content = "计算";
                if (task != null)
                {
                    IsCanceled = true;
                    MatchingFilter.p.Kill();
                    loadingBar.Hide();
                    ShowPrevAndCancel();
                }
            }
            else if (CurrentState == MaxState + 1)
            {
                System.Windows.Forms.SaveFileDialog ofd = new System.Windows.Forms.SaveFileDialog();
                ofd.Filter = "txt文件(*.txt)|*.txt|grd文件(*.grd)|*.grd|dat文件(*.dat)|*.dat";
                ofd.FilterIndex = 2;
                if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    try
                    {
                        File.Copy(MatchingFilter.outPath, ofd.FileName, true);
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
                FileInfo fi = new FileInfo(MatchingFilter.outPath);
                GRDPreviewWindow.PreviewShow(Application.Current.MainWindow, fi);
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
        /// 开始计算
        /// </summary>
        private async void DoMatchingFilter()
        {
            if (loadingBar.Show("计算中"))
            {
                CloseAndBackConfirm.Set(CloseAndBackConfirm.States.计算正在进行中);
                CurrentState = MaxState;
                IsCanceled = false;
                next.Content = "取消";
                HidePrevAndCancel();
                string path1 = inputPath1.filePath.Text;
                double _arg0, _arg1, _arg2;
                int choice = 0;
                if (!path1.Trim().EndsWith(".grd", StringComparison.OrdinalIgnoreCase))
                    Msg("输入文件类型不正确！");
                else if (!File.Exists(path1))
                    Msg("输入文件路径不存在！");
                else if (FileNameFilter.CheckGRDFileFormat(path1) == null)
                    Msg("输入文件不是GRD数据格式！");
                else if (!double.TryParse(arg0.Value, out _arg0))
                    Msg("深源埋深非法！");
                else if (_arg0 <= 0)
                    Msg("深源埋深应大于0！");
                else if (!double.TryParse(arg1.Value, out _arg1))
                    Msg("浅源埋深非法！");
                else if (_arg1 <= 0)
                    Msg("浅源埋深应大于0！");
                else if (!double.TryParse(arg2.Value, out _arg2))
                    Msg("纵轴截距非法！");
                else
                {
                    try
                    {
                        DeleteErrorStatus();
                        _arg0 *= double.Parse((arg1.SelectedItem as ComboBoxItem).Tag.ToString());
                        _arg1 *= double.Parse((arg1.SelectedItem as ComboBoxItem).Tag.ToString());
                        _arg2 *= double.Parse((arg2.SelectedItem as ComboBoxItem).Tag.ToString());
                        if (choice0.IsChecked == true)
                            choice = 0;
                        else if (choice1.IsChecked == true)
                            choice = 1;
                        task = MatchingFilter.Start(path1, _arg0, _arg1, _arg2, choice);
                        await task;
                        if (IsCanceled)
                        {
                            loadingBar.Hide();
                            ShowPrevAndCancel();
                            Msg("计算取消!");
                        }
                        else
                        {
                            string error = CheckErrorStatus();
                            if (error == null)
                            {
                                Completed();
                                return;
                            }
                            throw new Exception(error);
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
        /// 计算任务
        /// </summary>
        private Task<string> task = null;

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

        /// <summary>
        /// 删除遗留的错误信息
        /// </summary>
        private void DeleteErrorStatus()
        {
            if (File.Exists("error_status.txt"))
                File.Delete("error_status.txt");
        }

        /// <summary>
        /// 检查错误信息
        /// </summary>
        /// <returns></returns>
        private string CheckErrorStatus()
        {
            string error;
            if (!File.Exists("error_status.txt"))
                return null;
            using (var file = new StreamReader("error_status.txt", Encoding.GetEncoding("GB2312")))
            {
                error = file.ReadToEnd().Trim();
                if (error != "")
                    return error;
            }
            return null;
        }
    }
}
