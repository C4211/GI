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
    public partial class Function_grdht : FunctionPage
    {
        /// <summary>
        /// 构造函数，设置中英文标题
        /// </summary>
        public Function_grdht()
        {
            InitializeComponent();
            this.titleCn = "GRD画图";
            this.titleEn = "GRD Drawing";
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
        /// 点击下一步
        /// </summary>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //if (CurrentState < MaxState - 1)
            //{
            //    content.IsEnabled = false;
            //    buttons.IsEnabled = false;
            //    CurrentState = 1;
            //    content.Children[CurrentState].Visibility = Visibility.Visible;
            //    Storyboard sb = ((Storyboard)this.FindResource("sb")).Clone();
            //    ((ThicknessAnimation)sb.Children[0]).To = new Thickness(-CurrentState * 680, 0, 0, 0);
            //    sb.Completed += delegate { content.Children[CurrentState - 1].Visibility = Visibility.Hidden; content.IsEnabled = true; buttons.IsEnabled = true; };
            //    content.BeginStoryboard(sb);
            //    prev.Visibility = Visibility.Visible;
            //    //GrdPreviewWindow.PreviwShow(Application.Current.MainWindow);
            //    if (CurrentState == MaxState - 1)
            //        next.Content = "计算";
            //    return;
            //}
            //else 
            if (CurrentState == 0)
            {
                //CurrentState = MaxState;
                //IsCanceled = false;
                //next.Content = "取消";
                //开始计算
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
                SelectColorItem sci = (SelectColorItem)inputPath2.SelectedItem;
                if (sci == null)
                {
                    Msg("未找到颜色文件！");
                    return;
                }
                GRDPreviewWindow.PreviewShow(Application.Current.MainWindow, new FileInfo(inPath), inputPath2, 2);



            }
            else if (CurrentState == MaxState)
            {
                CurrentState = MaxState - 1;
                next.Content = "画图";
                //取消计算
            }
        }

        /// <summary>
        /// 点击上一步
        /// </summary>
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
        /// 弹出通知
        /// </summary>
        private void Msg(string msg)
        {
            Dispatcher.Invoke(delegate { MessageWindow.Show(Application.Current.MainWindow, msg); });
        }
    }
}
