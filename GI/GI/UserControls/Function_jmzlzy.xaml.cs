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
                CurrentState = MaxState;
                IsCanceled = false;
                next.Content = "取消";
                //开始计算
            }
            else if (CurrentState == MaxState)
            {
                CurrentState = MaxState - 1;
                next.Content = "计算";
                //取消计算
            }
        }

        private void prev_Click(object sender, RoutedEventArgs e)
        {

            if (CurrentState > 0)
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
