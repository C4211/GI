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
        public Function_dztcsfy()
        {
            InitializeComponent();
            this.titleCn = "地质体参数反演";
            this.titleEn = "Geological Parameters Inversion";
            cssz = content.Children[MaxState - 1] as Grid;
        }

        /// <summary>
        /// 0 : 选项
        /// 1 : 输入文件
        /// 2 : choice1-3=计算中 choice4=输入参数
        /// 3 :                  choice4=计算中
        /// </summary>
        private int CurrentState = 0;
        private int MaxState { get { return content.Children.Count; } }
        private bool IsCanceled = false;
        private Grid cssz;

        private void Page2_Reset()
        {
            Page1choice1.Visibility = Visibility.Hidden;
            Page1choice2.Visibility = Visibility.Hidden;
            Page1choice3.Visibility = Visibility.Hidden;
            Page1choice4.Visibility = Visibility.Hidden;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            #region 逻辑
            if (CurrentState == 1)
            {
                if (choice1.IsChecked == true)
                {
                    string path1 = choice1path1.filePath.Text;
                    string path2 = choice1path2.filePath.Text;
                    if (!FileNameFilter.CheckFileSuffix(path1))
                    {
                        Msg("重力异常数据文件类型不正确！");
                        return;
                    }
                    else if (!File.Exists(path1))
                    {
                        Msg("重力异常数据路径不存在！");
                        return;
                    }
                    try
                    {
                        if (!File.Exists(path2))
                            File.Create(path2).Dispose();
                    }
                    catch (Exception ex)
                    {
                        Msg("输出文件路径异常！\n" + ex.Message);
                        return;
                    }
                    // TODO: 计算1
                }
                else if (choice2.IsChecked == true)
                {
                    string path1 = choice2path1.filePath.Text;
                    string path2 = choice2path2.filePath.Text;
                    if (!FileNameFilter.CheckFileSuffix(path1))
                    {
                        Msg("重力异常数据文件类型不正确！");
                        return;
                    }
                    else if (!File.Exists(path1))
                    {
                        Msg("重力异常数据路径不存在！");
                        return;
                    }
                    try
                    {
                        if (!File.Exists(path2))
                            File.Create(path2).Dispose();
                    }
                    catch (Exception ex)
                    {
                        Msg("输出文件路径异常！\n" + ex.Message);
                        return;
                    }
                    // TODO: 计算2
                }
                else if (choice3.IsChecked == true)
                {
                    string path1 = choice3path1.filePath.Text;
                    string path2 = choice3path2.filePath.Text;
                    string path3 = choice3path3.filePath.Text;
                    if (!FileNameFilter.CheckFileSuffix(path1))
                    {
                        Msg("重力异常数据文件类型不正确！");
                        return;
                    }
                    else if (!File.Exists(path1))
                    {
                        Msg("重力异常数据路径不存在！");
                        return;
                    }
                    if (!FileNameFilter.CheckFileSuffix(path2))
                    {
                        Msg("重力梯度异常数据文件类型不正确！");
                        return;
                    }
                    else if (!File.Exists(path2))
                    {
                        Msg("重力梯度异常数据路径不存在！");
                        return;
                    }
                    try
                    {
                        if (!File.Exists(path3))
                            File.Create(path3).Dispose();
                    }
                    catch (Exception ex)
                    {
                        Msg("输出文件路径异常！\n" + ex.Message);
                        return;
                    }
                    // TODO: 计算3
                }
                else if (choice4.IsChecked == true)
                {
                    string path1 = choice4path1.filePath.Text;
                    string path2 = choice4path2.filePath.Text;
                    string path3 = choice4path3.filePath.Text;
                    if (!FileNameFilter.CheckFileSuffix(path1))
                    {
                        Msg("重力异常数据文件类型不正确！");
                        return;
                    }
                    else if (!File.Exists(path1))
                    {
                        Msg("重力异常数据路径不存在！");
                        return;
                    }
                    if (!FileNameFilter.CheckFileSuffix(path2))
                    {
                        Msg("重力梯度异常数据文件类型不正确！");
                        return;
                    }
                    else if (!File.Exists(path2))
                    {
                        Msg("重力梯度异常数据路径不存在！");
                        return;
                    }
                    try
                    {
                        if (!File.Exists(path3))
                            File.Create(path3).Dispose();
                    }
                    catch (Exception ex)
                    {
                        Msg("输出文件路径异常！\n" + ex.Message);
                        return;
                    }
                }
            }
            else if (CurrentState == 2)
            {
                if (choice1.IsChecked == true || choice2.IsChecked == true || choice3.IsChecked == true)
                {
                    // TODO: 取消计算
                }
                else if (choice4.IsChecked == true)
                {
                    double arg1, arg2;
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
                    // TODO: 计算4
                }
            }
            else if (CurrentState == 3)
            {
                if (choice4.IsChecked == true)
                {
                    // TODO: 取消计算
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
                //开始计算
                HidePrevAndCancel();
                loadingBar.Show();
                Msg("计算文件暂未添加");
                ShowPrevAndCancel();
                loadingBar.Hide();
                CurrentState = MaxState - 1;
                next.Content = "计算";
            }
            else if (CurrentState == MaxState)
            {
                CurrentState = MaxState - 1;
                next.Content = "计算";
                //取消计算
                loadingBar.Hide();
                ShowPrevAndCancel();
                Msg("计算已取消");
            }
            #endregion
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

        private void Msg(string msg)
        {
            Dispatcher.Invoke(delegate { MessageWindow.Show(Application.Current.MainWindow, msg); });
        }
        //private void Button_Click_1(object sender, RoutedEventArgs e)
        //{
        //    string unit = ((ComboBoxItem)midu.SelectedItem).Content.ToString();
        //    string Converter = ((ComboBoxItem)midu.SelectedItem).Tag.ToString();
        //    string value;
        //    if(midu.Value!=null)
        //    {
        //        value = midu.Value.ToString();
        //    }
        //    else
        //    {
        //        value = "null";
        //    }
        //    MessageWindow.Show("值：" + value + "\n" + "单位:" + unit + "\n" + "转换:" + Converter);
        //}
    }
}
