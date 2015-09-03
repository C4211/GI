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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            #region 逻辑
            int choice = 1;
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
            else if (CurrentState == 2)
            {
                page2args = new double[8];
                switch(choice)
                {
                    case 1:
                        // 球形
                        if (!double.TryParse(arg1_1.Value, out page2args[1]))
                        { Msg("球心X坐标非法！"); return; }
                        else if (!double.TryParse(arg1_2.Value, out page2args[2]))
                        { Msg("球心Y坐标非法！"); return; }
                        else if (!double.TryParse(arg1_3.Value, out page2args[3]))
                        { Msg("球心Z坐标非法！"); return; }
                        else if (!double.TryParse(arg1_4.Value, out page2args[4]))
                        { Msg("球形半径非法！"); return; }
                        else if (!double.TryParse(arg1_5.Value, out page2args[5]))
                        { Msg("球形密度非法！"); return; }
                        break;
                    case 2:
                        // 垂直圆柱体
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
                        break;
                    case 3:
                        // 水平圆柱体
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
                        break;
                    case 4:
                        // 长方体
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
                        break;
                    default:
                        Msg("模型错误");
                        return;
                }
            }
            else if (CurrentState == 3)
            {
                if (!double.TryParse(arg5_1.Value, out page3args[1]))
                { Msg("重心位置X坐标非法！"); return; }
                else if (!double.TryParse(arg5_2.Value, out page3args[2]))
                { Msg("重心位置Y坐标非法！"); return; }
                else if (!double.TryParse(arg5_3.Value, out page3args[3]))
                { Msg("重心位置Z坐标非法！"); return; }
                else if (!double.TryParse(arg5_4.Value, out page3args[4]))
                { Msg("X方向观测面边长非法！"); return; }
                else if (!double.TryParse(arg5_5.Value, out page3args[5]))
                { Msg("X方向观测面分辨率非法！"); return; }
                else if (!double.TryParse(arg5_6.Value, out page3args[6]))
                { Msg("Y方向观测面边长非法！"); return; }
                else if (!double.TryParse(arg5_7.Value, out page3args[7]))
                { Msg("Y方向观测面分辨率非法！"); return; }
                // TODO: 开始计算
                switch(choice)
                {
                    case 1:
                        break;
                    case 2:
                        break;
                    case 3: 
                        break;
                    case 4:
                        break;
                    default:
                        Msg("模型错误");
                        return;
                }
            }
            else if (CurrentState == 4)
            {
                // TODO: 取消计算
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
            }
            else if (CurrentState == MaxState)
            {
                CurrentState = MaxState - 1;
                next.Content = "计算";
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
