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
    /// <summary>
    /// Function_fxyf.xaml 的交互逻辑
    /// </summary>
    public partial class Function_cxdskjy : FunctionPage
    {
        public Function_cxdskjy()
        {
            InitializeComponent();
            this.titleCn = "垂向导数（空间域）";
            this.titleEn = "Vertical Derivative(Spatial Domain)";
        }

        /// <summary>
        /// 0 : 输入文件
        /// 1 : 输入参数
        /// 2 : 计算中
        /// </summary>
        private int CurrentState = 0;
        private int MaxState { get { return content.Children.Count; } }
        private bool IsCanceled = false;
        private double[] data;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentState < MaxState - 1)
            {
                content.IsEnabled = false;
                buttons.IsEnabled = false;
                string inPath = inputPath1.filePath.Text;
                try
                {
                    data = VerticalDerivativeSpace.Init(inPath);
                    dx.Text = data[0].ToString();
                    dy.Text = data[1].ToString();
                }
                catch (Exception ex)
                {
                    content.IsEnabled = true;
                    buttons.IsEnabled = true;
                    Msg(ex.Message);
                    return;
                }
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
                //开始计算
                HidePrevAndCancel();
                DoVerticalDerivativeSpace();
            }
            else if (CurrentState == MaxState)
            {
                CurrentState = MaxState - 1;
                next.Content = "计算";
                //取消计算
                if (task != null)
                {
                    IsCanceled = true;
                    VerticalDerivativeSpace.p.Kill();
                    loadingBar.Hide();
                    ShowPrevAndCancel();
                }
            }
        }

        private void prev_Click(object sender, RoutedEventArgs e)
        {

            if (CurrentState > 0)
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

        private async void DoVerticalDerivativeSpace()
        {
            HidePrevAndCancel();
            loadingBar.Show();
            string outPath = outputPath1.filePath.Text;
            int order = 1, choice = 0;
            try
            {
                if (order1.IsChecked == true)
                    order = 1;
                else if (order2.IsChecked == true)
                    order = 2;
                if (choice0.IsChecked == true)
                    choice = 0;
                else if (choice1.IsChecked == true)
                    choice = 1;
                else if (choice2.IsChecked == true)
                    choice = 2;
                else if (choice3.IsChecked == true)
                    choice = 3;
                else if (choice4.IsChecked == true)
                    choice = 4;
                task = VerticalDerivativeSpace.Start(order, choice);
                await task;
                if (IsCanceled)
                {
                    loadingBar.Hide();
                    ShowPrevAndCancel();
                    Msg("计算取消!");
                }
                else
                {
                    File.Copy(VerticalDerivativeSpace.outPath, outPath, true);
                    loadingBar.Hide();
                    ShowPrevAndCancel();
                    Msg("计算完成");
                }
            }
            catch (Exception e)
            {
                Msg(e.Message);
            }
            finally
            {
                task = null;
                loadingBar.Hide();
                ShowPrevAndCancel();
                Dispatcher.Invoke(delegate
                {
                    CurrentState = MaxState - 1;
                    next.Content = "计算";
                });
            }
        }

        private Task<string> task = null;

        private void Msg(string msg)
        {
            Dispatcher.Invoke(delegate { MessageWindow.Show(Application.Current.MainWindow, msg); });
        }
    }
}
