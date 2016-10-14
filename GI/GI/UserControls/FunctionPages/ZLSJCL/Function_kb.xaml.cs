using GI.Functions;
using GI.Tools;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace GI.UserControls
{
    /// <summary>
    /// Function_fxyf.xaml 的交互逻辑
    /// </summary>
    public partial class Function_kb : FunctionPage
    {
        /// <summary>
        /// 构造函数，设置中英文标题
        /// </summary>
        public Function_kb()
        {
            InitializeComponent();
            this.titleCn = "扩边";
            this.titleEn = "Expand";
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
        /// 计算参数集合
        /// </summary>
        private int[] data;

        /// <summary>
        /// 点击下一步
        /// </summary>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int Nx_out = 0, Ny_out = 0;
            #region 逻辑

            if (CurrentState == 0)
            {
                // 检查输入文件路径
                string inPath = inputPath1.filePath.Text;
                try
                {
                    data = Expand.Init(inPath);
                    Nx_input.Text = data[0].ToString();
                    Ny_input.Text = data[1].ToString();
                    Nx_output.Text = data[2].ToString();
                    Ny_output.Text = data[3].ToString();
                }
                catch (Exception ex)
                {
                    Msg(ex.Message);
                    return;
                }
            }
            else if (CurrentState == 1)
            {
                // 检查扩边后行列数
                if (!int.TryParse(Nx_output.Text, out Nx_out))
                {
                    Msg("扩边后行数非法！");
                    return;
                }
                if (!int.TryParse(Ny_output.Text, out Ny_out))
                {
                    Msg("扩边后列数非法！");
                    return;
                }
                if (Nx_out < data[0])
                {
                    Msg("扩边后的行数小于原始行数！");
                    return;
                }
                if (Ny_out < data[1])
                {
                    Msg("扩边后的列数小于原始列数！");
                    return;
                }

            }
            #endregion

            #region 界面
            if (CurrentState < MaxState - 1)
            {
                content.IsEnabled = false;
                buttons.IsEnabled = false;
                CurrentState = 1;
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
                DoExpand(Nx_out, Ny_out);
            }
            else if (CurrentState == MaxState)
            {
                CurrentState = MaxState - 1;
                next.Content = "计算";
                if (task != null)
                {
                    IsCanceled = true;
                    Expand.p.Kill();
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
                        File.Copy(Expand.outPath, ofd.FileName, true);
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
                FileInfo fi = new FileInfo(Expand.outPath);
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
        private async void DoExpand(int Nx_out, int Ny_out)
        {
            if (loadingBar.Show("计算中"))
            {
                CloseAndBackConfirm.Set(CloseAndBackConfirm.States.计算正在进行中);
                CurrentState = MaxState;
                IsCanceled = false;
                next.Content = "取消";
                HidePrevAndCancel();
                try
                {
                    DeleteErrorStatus();
                    task = Expand.Start(Nx_out, Ny_out);
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
        /// 输入时校验参数是否合法
        /// </summary>
        private void N_output_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            int tmp;
            e.Handled = !int.TryParse(e.Text, out tmp);
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
                if (error != "9999")
                    return error;
            }
            return null;
        }
    }
}
