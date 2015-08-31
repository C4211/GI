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
    public partial class Function_glpfx : FunctionPage
    {
        public Function_glpfx()
        {
            InitializeComponent();
            this.titleCn = "功率谱分析";
            this.titleEn = "Power Spectrum Analysis";
        }
        /// <summary>
        /// 0 : 输入文件
        /// 1 : 计算中
        /// </summary>
        private int CurrentState = 0;
        private int MaxState { get { return content.Children.Count; } }
        private bool IsCanceled = false;

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
                DoPSD();
            }
            else if (CurrentState == 1)
            {
                if (task != null)
                {
                    IsCanceled = true;
                    PSD.p.Kill();
                    loadingBar.Hide();
                    ShowPrevAndCancel();
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
            else if (CurrentState == MaxState)
            {
                CurrentState = MaxState - 1;
                next.Content = "计算";
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
                        File.Copy(PSD.outPath, ofd.FileName, true);
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

        private void prev_Click(object sender, RoutedEventArgs e)
        {

            if (CurrentState > 0 && CurrentState <= MaxState)
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
            else if (CurrentState == MaxState + 1)
            {
                FileInfo fi = new FileInfo(PSD.outPath);
                FilePreviewWindow.PreviwShow(Application.Current.MainWindow, fi);
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

        private async void DoPSD()
        {
            if (loadingBar.Show("计算中"))
            {
                CloseAndBackConfirm.Set(CloseAndBackConfirm.States.计算正在进行中);
                CurrentState = MaxState;
                IsCanceled = false;
                next.Content = "取消";
                HidePrevAndCancel();
                string path1 = inputPath1.filePath.Text;
                if (!path1.Trim().EndsWith(".grd", StringComparison.OrdinalIgnoreCase))
                    Msg("输入文件类型不正确！");
                else if (!File.Exists(path1))
                    Msg("输入文件路径不存在！");
                else if (FileNameFilter.CheckGRDFileFormat(path1) == null)
                    Msg("输入文件不是GRD数据格式！");
                else
                {
                    try
                    {
                        task = PSD.Start(path1);
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
                            //File.Copy(PSD.outPath, outPath, true);
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
        private Task<string> task = null;

        private void Msg(string msg)
        {
            Dispatcher.Invoke(delegate { MessageWindow.Show(Application.Current.MainWindow, msg); });
        }

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
