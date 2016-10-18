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
    public partial class Function_zljmfy : FunctionPage
    {
        /// <summary>
        /// 构造函数，设置中英文标题
        /// </summary>
        public Function_zljmfy()
        {
            InitializeComponent();
            this.titleCn = "重力界面反演";
            this.titleEn = "Gravity Interface Inversion Calculation";
        }

        /// <summary>
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
        /// 计算参数
        /// </summary>
        double refDepth, contrast, wh, sh, criterio, truncation;
        /// <summary>
        /// 最大计算迭代次数
        /// </summary>
        int maxIter;
        /// <summary>
        /// 输入文件路径
        /// </summary>
        string inPath;

        /// <summary>
        /// 点击下一步
        /// </summary>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            #region 逻辑

            if (CurrentState == 0)
            {
                // 检查输入文件路径
                inPath = inputPath1.filePath.Text;
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
                if (!double.TryParse(arg_refDepth.Value, out refDepth))
                {
                    Msg("参考深度不合法！");
                    return;
                }
                else if (!double.TryParse(arg_contrast.Value, out contrast))
                {
                    Msg("密度差不合法！");
                    return;
                }
                else if (!double.TryParse(arg_wh.Text, out wh))
                {
                    Msg("最小截断频率不合法！");
                    return;
                }
                else if (!double.TryParse(arg_sh.Text, out sh))
                {
                    Msg("最大截断频率不合法！");
                    return;
                }
                else if (!int.TryParse(arg_maxIter.Text, out maxIter))
                {
                    Msg("最大迭代次数不合法！");
                    return;
                }
                else if (!double.TryParse(arg_criterio.Text, out criterio))
                {
                    Msg("收敛准则不合法！");
                    return;
                }
                else if (!double.TryParse(arg_truncation.Text, out truncation))
                {
                    Msg("截断窗口数据长度不合法！");
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
                //开始计算
                DoInterfaceInversion();
            }
            else if (CurrentState == MaxState)
            {
                CurrentState = MaxState - 1;
                next.Content = "计算";
                //取消计算
                if (task != null)
                {
                    IsCanceled = true;
                    InterfaceInversion.p.Kill();
                    loadingBar.Hide();
                    ShowPrevAndCancel();
                }
            }
            else if (CurrentState == MaxState + 1)
            {
                System.Windows.Forms.SaveFileDialog ofd = new System.Windows.Forms.SaveFileDialog();
                ofd.Filter = "txt文件(*.txt)|*.txt|grd文件(*.grd)|*.grd|dat文件(*.dat)|*.dat";
                ofd.FilterIndex = 2;
                ofd.FileName = "topoout.grd";
                if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    try
                    {
                        //保存
                        File.Copy(InterfaceInversion.outPath1, ofd.FileName, true);
                        Msg("界面输出文件已保存！");
                        CloseAndBackConfirm.Reset();
                    }
                    catch
                    {
                        Msg("保存失败！");
                    }
                }
                System.Windows.Forms.SaveFileDialog ofd1 = new System.Windows.Forms.SaveFileDialog();
                ofd1.Filter = "txt文件(*.txt)|*.txt|grd文件(*.grd)|*.grd|dat文件(*.dat)|*.dat";
                ofd1.FilterIndex = 2;
                ofd1.FileName = "bouinverted.grd";
                if (ofd1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    try
                    {
                        //保存
                        File.Copy(InterfaceInversion.outPath2, ofd1.FileName, true);
                        Msg("重力输出文件已保存！");
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
                //预览
                FileInfo fi = new FileInfo(InterfaceInversion.outPath1);
                GRDPreviewWindow.PreviewShow(Application.Current.MainWindow, fi);
                fi = new FileInfo(InterfaceInversion.outPath2);
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
        private async void DoInterfaceInversion()
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
                    //开始进程
                    task = InterfaceInversion.Start(inPath, contrast, criterio, refDepth, wh, sh, truncation, maxIter);
                    await task;
                    if (IsCanceled)
                    {
                        loadingBar.Hide();
                        ShowPrevAndCancel();
                        Msg("计算取消!");
                    }
                    else
                    {
                        Completed();
                        return;
                        //File.Copy(计算类.outPath, outPath, true);
                        //loadingBar.Hide();
                        //ShowPrevAndCancel();
                        //Msg("计算完成");
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
    }
}
