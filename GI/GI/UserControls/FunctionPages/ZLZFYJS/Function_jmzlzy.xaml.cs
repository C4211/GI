﻿using GI.Functions;
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
    public partial class Function_jmzlzy : FunctionPage
    {
        public Function_jmzlzy()
        {
            InitializeComponent();
            this.titleCn = "界面重力正演";
            this.titleEn = "Interface Gravity Forward Calculation";
        }

        /// <summary>
        /// 0 : 输入文件
        /// 1 : 输入参数
        /// 2 : 计算中
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
                DoInterfaceForward();
            }
            else if (CurrentState == MaxState)
            {
                CurrentState = MaxState - 1;
                next.Content = "计算";
                //取消计算
                if (task != null)
                {
                    IsCanceled = true;
                    InterfaceForward.p.Kill();
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
                        File.Copy(InterfaceForward.outPath, ofd.FileName, true);
                        Msg("已保存！");
                        CloseAndBackConfirm.Reset();
                    }
                    catch
                    {
                        Msg("保存失败！");
                    }
                }
            }
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
                FileInfo fi = new FileInfo(InterfaceForward.outPath);
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

        private async void DoInterfaceForward()
        {
            if (loadingBar.Show("计算中"))
            {
                CloseAndBackConfirm.Set(CloseAndBackConfirm.States.计算正在进行中);
                CurrentState = MaxState;
                IsCanceled = false;
                next.Content = "取消";
                HidePrevAndCancel();
                string path1 = inputPath1.filePath.Text;
                double _referenceDepth, _densityContrast;
                int _coordinateUnit = 0, _depthUnit = 0;
                if (!path1.Trim().EndsWith(".grd", StringComparison.OrdinalIgnoreCase))
                    Msg("输入文件类型不正确！");
                else if (!File.Exists(path1))
                    Msg("输入文件路径不存在！");
                else if (FileNameFilter.CheckGRDFileFormat(path1) == null)
                    Msg("输入文件不是GRD数据格式！");
                else if (!double.TryParse(referenceDepth.Value, out _referenceDepth))
                    Msg("参考深度不合法！");
                else if (!double.TryParse(densityContrast.Value, out _densityContrast))
                    Msg("密度差不合法！");
                else
                {
                    try
                    {
                        if (coordinateUnit0.IsChecked == true)
                            _coordinateUnit = 0;
                        else if (coordinateUnit1.IsChecked == true)
                            _coordinateUnit = 1;
                        if (depthUnit0.IsChecked == true)
                            _depthUnit = 0;
                        else if (coordinateUnit1.IsChecked == true)
                            _depthUnit = 1;
                        task = InterfaceForward.Start(path1, _referenceDepth, _densityContrast, _coordinateUnit, _depthUnit);
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
                            //File.Copy(InterfaceForward.outPath, outPath, true);
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
