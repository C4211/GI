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
    public partial class Function_dztzlzltdjs : FunctionPage
    {
        public Function_dztzlzltdjs()
        {
            InitializeComponent();
            this.titleCn = "地质体重力/重力梯度计算";
            this.titleEn = "Gravity And Gravity Gradient Calculation";
        }

        /// <summary>
        /// 0 : 选择模型
        /// 1 : 模型参数设置
        /// 2 : 观测面参数设置
        /// 3 : 计算类型
        /// 4 : 计算中
        /// </summary>
        private int CurrentState = 0;
        private int MaxState { get { return content.Children.Count; } }
        private bool IsCanceled = false;
        double[] page2args, page3args;
        double cenZ1, sizeZ1;
        int choice = 1;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            #region 逻辑
            if (CurrentState == 0)
            {
                if (choice1.IsChecked == true)
                    choice = 1;
                else if (choice2.IsChecked == true)
                    choice = 2;
                else if (choice3.IsChecked == true)
                    choice = 3;
                else if (choice4.IsChecked == true)
                    choice = 4;
            }
            else if (CurrentState == 1)
            {
                page2args = new double[8];
                switch (choice)
                {
                    case 1:
                        // 球形
                        page2args[0] = 1;
                        if (!double.TryParse(arg1_1.Value, out page2args[1]))
                        { Msg("球心X坐标非法！"); return; }
                        else if (!double.TryParse(arg1_2.Value, out page2args[2]))
                        { Msg("球心Y坐标非法！"); return; }
                        else if (!double.TryParse(arg1_3.Value, out page2args[3]))
                        { Msg("球心Z坐标非法！"); return; }
                        else if (!double.TryParse(arg1_4.Value, out page2args[4]))
                        { Msg("球形半径非法！"); return; }
                        else if (!double.TryParse(arg1_5.Value, out page2args[5]) || page2args[5] > 23000)
                        { Msg("球形密度非法！"); return; }
                        page2args[1] *= double.Parse((arg1_1.SelectedItem as ComboBoxItem).Tag.ToString());
                        page2args[2] *= double.Parse((arg1_2.SelectedItem as ComboBoxItem).Tag.ToString());
                        page2args[3] *= double.Parse((arg1_3.SelectedItem as ComboBoxItem).Tag.ToString());
                        page2args[4] *= double.Parse((arg1_4.SelectedItem as ComboBoxItem).Tag.ToString());
                        page2args[5] *= double.Parse((arg1_5.SelectedItem as ComboBoxItem).Tag.ToString());
                        cenZ1 = page2args[3];
                        sizeZ1 = page2args[4] * 2;
                        break;
                    case 2:
                        // 垂直圆柱体
                        page2args[0] = 2;
                        if (!double.TryParse(arg2_1.Value, out page2args[1]))
                        { Msg("顶面中心X坐标非法！"); return; }
                        else if (!double.TryParse(arg2_2.Value, out page2args[2]))
                        { Msg("顶面中心Y坐标非法！"); return; }
                        else if (!double.TryParse(arg2_3.Value, out page2args[3]))
                        { Msg("顶面中心Z坐标非法！"); return; }
                        else if (!double.TryParse(arg2_4.Value, out page2args[4]))
                        { Msg("垂直圆柱体顶面深度非法！"); return; }
                        else if (!double.TryParse(arg2_5.Value, out page2args[5]))
                        { Msg("垂直圆柱体半径非法！"); return; }
                        else if (!double.TryParse(arg2_6.Value, out page2args[6]))
                        { Msg("垂直圆柱体高度非法！"); return; }
                        else if (!double.TryParse(arg2_7.Value, out page2args[7]))
                        { Msg("垂直圆柱体密度非法！"); return; }
                        else if (page2args[3] + page2args[4] != 0)
                        { Msg("模型参数设置不合理！"); return; }
                        page2args[1] *= double.Parse((arg2_1.SelectedItem as ComboBoxItem).Tag.ToString());
                        page2args[2] *= double.Parse((arg2_2.SelectedItem as ComboBoxItem).Tag.ToString());
                        page2args[3] *= double.Parse((arg2_3.SelectedItem as ComboBoxItem).Tag.ToString());
                        page2args[4] *= double.Parse((arg2_4.SelectedItem as ComboBoxItem).Tag.ToString());
                        page2args[5] *= double.Parse((arg2_5.SelectedItem as ComboBoxItem).Tag.ToString());
                        page2args[6] *= double.Parse((arg2_6.SelectedItem as ComboBoxItem).Tag.ToString());
                        page2args[7] *= double.Parse((arg2_7.SelectedItem as ComboBoxItem).Tag.ToString());
                        if (page2args[7] > 23000)
                        { Msg("垂直圆柱体密度非法！"); return; }
                        cenZ1 = page2args[3] - page2args[6] / 2;
                        sizeZ1 = page2args[6];
                        break;
                    case 3:
                        // 水平圆柱体
                        page2args[0] = 3;
                        if (!double.TryParse(arg3_1.Value, out page2args[1]))
                        { Msg("左面中心X坐标非法！"); return; }
                        else if (!double.TryParse(arg3_2.Value, out page2args[2]))
                        { Msg("左面中心Y坐标非法！"); return; }
                        else if (!double.TryParse(arg3_3.Value, out page2args[3]))
                        { Msg("左面中心Z坐标非法！"); return; }
                        else if (!double.TryParse(arg3_4.Value, out page2args[4]))
                        { Msg("水平圆柱体顶面深度非法！"); return; }
                        else if (!double.TryParse(arg3_5.Value, out page2args[5]))
                        { Msg("水平圆柱体半径非法！"); return; }
                        else if (!double.TryParse(arg3_6.Value, out page2args[6]))
                        { Msg("水平圆柱体高度非法！"); return; }
                        else if (!double.TryParse(arg3_7.Value, out page2args[7]))
                        { Msg("水平圆柱体密度非法！"); return; }
                        else if (page2args[3] + page2args[4] + page2args[5] != 0)
                        { Msg("模型参数设置不合理！"); return; }
                        page2args[1] *= double.Parse((arg3_1.SelectedItem as ComboBoxItem).Tag.ToString());
                        page2args[2] *= double.Parse((arg3_2.SelectedItem as ComboBoxItem).Tag.ToString());
                        page2args[3] *= double.Parse((arg3_3.SelectedItem as ComboBoxItem).Tag.ToString());
                        page2args[4] *= double.Parse((arg3_4.SelectedItem as ComboBoxItem).Tag.ToString());
                        page2args[5] *= double.Parse((arg3_5.SelectedItem as ComboBoxItem).Tag.ToString());
                        page2args[6] *= double.Parse((arg3_6.SelectedItem as ComboBoxItem).Tag.ToString());
                        page2args[7] *= double.Parse((arg3_7.SelectedItem as ComboBoxItem).Tag.ToString());
                        if (page2args[7] > 23000)
                        { Msg("水平圆柱体密度非法！"); return; }
                        cenZ1 = page2args[3];
                        sizeZ1 = page2args[5] * 2;
                        break;
                    case 4:
                        // 长方体
                        page2args[0] = 4;
                        if (!double.TryParse(arg4_1.Value, out page2args[1]))
                        { Msg("X方向下界非法！"); return; }
                        else if (!double.TryParse(arg4_2.Value, out page2args[2]))
                        { Msg("X方向上界非法！"); return; }
                        else if (!double.TryParse(arg4_3.Value, out page2args[3]))
                        { Msg("Y方向下界非法！"); return; }
                        else if (!double.TryParse(arg4_4.Value, out page2args[4]))
                        { Msg("Y方向上界非法！"); return; }
                        else if (!double.TryParse(arg4_5.Value, out page2args[5]))
                        { Msg("Z方向下界非法！"); return; }
                        else if (!double.TryParse(arg4_6.Value, out page2args[6]))
                        { Msg("Z方向上界非法！"); return; }
                        else if (!double.TryParse(arg4_7.Value, out page2args[7]))
                        { Msg("长方体密度非法！"); return; }
                        page2args[1] *= double.Parse((arg4_1.SelectedItem as ComboBoxItem).Tag.ToString());
                        page2args[2] *= double.Parse((arg4_2.SelectedItem as ComboBoxItem).Tag.ToString());
                        page2args[3] *= double.Parse((arg4_3.SelectedItem as ComboBoxItem).Tag.ToString());
                        page2args[4] *= double.Parse((arg4_4.SelectedItem as ComboBoxItem).Tag.ToString());
                        page2args[5] *= double.Parse((arg4_5.SelectedItem as ComboBoxItem).Tag.ToString());
                        page2args[6] *= double.Parse((arg4_6.SelectedItem as ComboBoxItem).Tag.ToString());
                        page2args[7] *= double.Parse((arg4_7.SelectedItem as ComboBoxItem).Tag.ToString());
                        if (page2args[7] > 23000)
                        { Msg("长方体密度非法！"); return; }
                        else if (page2args[1] >= page2args[2] || page2args[3] >= page2args[4] || page2args[5] >= page2args[6])
                        { Msg("模型参数设置不合理！"); return; }
                        cenZ1 = (page2args[5] + page2args[6]) / 2;
                        sizeZ1 = page2args[6] - page2args[5];
                        break;
                    default:
                        Msg("模型错误");
                        return;
                }
            }
            else if (CurrentState == 2)
            {
                page3args = new double[8];
                if (!double.TryParse(arg5_1.Value, out page3args[1]))
                { Msg("重心位置X坐标非法！"); return; }
                else if (!double.TryParse(arg5_2.Value, out page3args[2]))
                { Msg("重心位置Y坐标非法！"); return; }
                else if (!double.TryParse(arg5_3.Value, out page3args[3]))
                { Msg("重心位置Z坐标非法！"); return; }
                else if (!double.TryParse(arg5_4.Value, out page3args[4]))
                { Msg("X方向观测面边长非法！"); return; }
                else if (!double.TryParse(arg5_5.Value, out page3args[5]))
                { Msg("Y方向观测面边长非法！"); return; }
                else if (!double.TryParse(arg5_6.Value, out page3args[6]))
                { Msg("X方向观测面分辨率非法！"); return; }
                else if (!double.TryParse(arg5_7.Value, out page3args[7]))
                { Msg("Y方向观测面分辨率非法！"); return; }
                page3args[1] *= double.Parse((arg5_1.SelectedItem as ComboBoxItem).Tag.ToString());
                page3args[2] *= double.Parse((arg5_2.SelectedItem as ComboBoxItem).Tag.ToString());
                page3args[3] *= double.Parse((arg5_3.SelectedItem as ComboBoxItem).Tag.ToString());
                page3args[4] *= double.Parse((arg5_4.SelectedItem as ComboBoxItem).Tag.ToString());
                page3args[5] *= double.Parse((arg5_5.SelectedItem as ComboBoxItem).Tag.ToString());
                page3args[6] *= double.Parse((arg5_6.SelectedItem as ComboBoxItem).Tag.ToString());
                page3args[7] *= double.Parse((arg5_7.SelectedItem as ComboBoxItem).Tag.ToString());
                if (page3args[5] > page3args[4])
                { Msg("观测面分辨率大于观测面边长！"); return; }
                else if (page3args[7] > page3args[6])
                { Msg("观测面分辨率大于观测面边长！"); return; }
                else if (page3args[3] <= cenZ1 + sizeZ1 * 1.0 / 2)
                { Msg("观测面位置和模型位置不匹配！"); return; }
            }
            #endregion

            #region 界面
            if (CurrentState == 0)
            {
                reset_Pagechoice();
                if (choice1.IsChecked == true)
                    Page1choice1.Visibility = Visibility.Visible;
                else if (choice2.IsChecked == true)
                    Page1choice2.Visibility = Visibility.Visible;
                else if (choice3.IsChecked == true)
                    Page1choice3.Visibility = Visibility.Visible;
                else if (choice4.IsChecked == true)
                    Page1choice4.Visibility = Visibility.Visible;
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
                DoGeoForward();
            }
            else if (CurrentState == MaxState)
            {
                CurrentState = MaxState - 1;
                next.Content = "计算";
                // 取消计算
                if (task != null)
                {
                    IsCanceled = true;
                    GeoForward.p.Kill();
                    loadingBar.Hide();
                    ShowPrevAndCancel();
                }
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
                        File.Copy(GeoForward.outPath, ofd.FileName, true);
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

        private void reset_Pagechoice()
        {
            Page1choice1.Visibility = Visibility.Hidden;
            Page1choice2.Visibility = Visibility.Hidden;
            Page1choice3.Visibility = Visibility.Hidden;
            Page1choice4.Visibility = Visibility.Hidden;
        }

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
                FileInfo fi = new FileInfo(GeoForward.outPath);
                FilePreviewWindow.PreviwShow(Application.Current.MainWindow, fi);
            }
        }

        private async void DoGeoForward()
        {
            if (loadingBar.Show("计算中"))
            {
                CloseAndBackConfirm.Set(CloseAndBackConfirm.States.计算正在进行中);
                CurrentState = MaxState;
                IsCanceled = false;
                next.Content = "取消";
                HidePrevAndCancel();
                {
                    int choice = 1;
                    if (Page4choice1.IsChecked == true)
                        choice = 1;
                    else if (Page4choice2.IsChecked == true)
                        choice = 2;
                    try
                    {
                        task = GeoForward.Start(page2args, page3args, choice);
                        await task;
                        if (IsCanceled)
                        {
                            loadingBar.Hide();
                            ShowPrevAndCancel();
                            Msg("计算取消!");
                        }
                        else if (task.Result != "")
                        {
                            loadingBar.Hide();
                            ShowPrevAndCancel();
                            Msg(task.Result);
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

        private Task<string> task = null;

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
