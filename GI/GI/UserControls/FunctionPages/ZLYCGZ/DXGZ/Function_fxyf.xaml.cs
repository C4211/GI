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
    public partial class Function_fxyf : FunctionPage
    {
        /// <summary>
        /// 构造函数，设置中英文标题
        /// </summary>
        public Function_fxyf()
        {
            InitializeComponent();
            this.titleCn = "方形域法";
            this.titleEn = "TC";
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
                string path1 = inputPath1.filePath.Text;
                string path2 = inputPath2.filePath.Text;
                string path3 = inputPath3.filePath.Text;
                if (!FileNameFilter.CheckFileSuffix(path1))
                {
                    Msg("站点文件类型不正确！");
                    return;
                }
                else if (!File.Exists(path1))
                {
                    Msg("站点文件路径不存在！");
                    return;
                }
                else if (!FileNameFilter.CheckFileSuffix(path2))
                {
                    Msg("内区地形文件类型不正确！");
                    return;
                }
                else if (!File.Exists(path2))
                {
                    Msg("内区地形文件路径不存在！");
                    return;
                }
                else if (!FileNameFilter.CheckFileSuffix(path3))
                {
                    Msg("外区地形文件类型不正确！");
                    return;
                }
                else if (!File.Exists(path3))
                {
                    Msg("外区地形文件路径不存在！");
                    return;
                }
            }
            else if (CurrentState == 1)
            {
                // 参数判断
                double _arg1, _arg2, _arg3;
                if (!double.TryParse(arg1.Value, out _arg1))
                {
                    Msg("密度值非法！");
                    return;
                }
                else if (!double.TryParse(arg2.Value, out _arg2))
                {
                    Msg("内区半径非法！");
                    return;
                }
                else if (_arg2 <= 0)
                {
                    Msg("内区半径应大于0！");
                    return;
                }
                else if (!double.TryParse(arg3.Value, out _arg3))
                {
                    Msg("外区半径非法！");
                    return;
                }
                else if (_arg3 <= 0)
                {
                    Msg("外区半径应大于0！");
                    return;
                }
                else if (_arg2 > _arg3)
                {
                    Msg("内区参数值比外区参数值大！");
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
                DoFXYF();
            }
            else if (CurrentState == MaxState)
            {
                CurrentState = MaxState - 1;
                next.Content = "计算";
                if (Task_fxyf != null)
                {
                    IsCanceled = true;
                    FXYF.p.Kill();
                    loadingBar.Hide();
                    ShowPrevAndCancel();
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
                        File.Copy(FXYF.outPath, ofd.FileName, true);
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
                FileInfo fi = new FileInfo(FXYF.outPath);
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
        /// 开始计算
        /// </summary>
        private async void DoFXYF()
        {
            if (loadingBar.Show("计算中"))
            {
                CloseAndBackConfirm.Set(CloseAndBackConfirm.States.计算正在进行中);
                CurrentState = MaxState;
                IsCanceled = false;
                next.Content = "取消";
                HidePrevAndCancel();
                string path1 = inputPath1.filePath.Text;
                string path2 = inputPath2.filePath.Text;
                string path3 = inputPath3.filePath.Text;
                double _arg1, _arg2, _arg3;
                if (!FileNameFilter.CheckFileSuffix(path1))
                    Msg("站点文件类型不正确！");
                else if (!File.Exists(path1))
                    Msg("站点文件路径不存在！");
                else if (!FileNameFilter.CheckFileSuffix(path2))
                    Msg("内区地形文件类型不正确！");
                else if (!File.Exists(path2))
                    Msg("内区地形文件路径不存在！");
                else if (!FileNameFilter.CheckFileSuffix(path3))
                    Msg("外区地形文件类型不正确！");
                else if (!File.Exists(path3))
                    Msg("外区地形文件路径不存在！");
                else if (!double.TryParse(arg1.Value, out _arg1))
                    Msg("密度值非法！");
                else if (!double.TryParse(arg2.Value, out _arg2))
                    Msg("内区半径非法！");
                else if (_arg2 <= 0)
                    Msg("内区半径应大于0！");
                else if (!double.TryParse(arg3.Value, out _arg3))
                    Msg("外区半径非法！");
                else if (_arg3 <= 0)
                    Msg("外区半径应大于0！");
                else
                {
                    try
                    {
                        DeleteErrorStatus();
                        _arg1 *= double.Parse((arg1.SelectedItem as ComboBoxItem).Tag.ToString());
                        _arg2 *= double.Parse((arg2.SelectedItem as ComboBoxItem).Tag.ToString());
                        _arg3 *= double.Parse((arg3.SelectedItem as ComboBoxItem).Tag.ToString());
                        Task_fxyf = FXYF.Start(path1, path2, path3, _arg1, _arg2, _arg3);
                        await Task_fxyf;
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
                        Task_fxyf = null;
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
        private Task<string> Task_fxyf = null;

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
