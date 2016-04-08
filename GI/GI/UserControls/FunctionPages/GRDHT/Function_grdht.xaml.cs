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
        public Function_grdht()
        {
            InitializeComponent();
            this.titleCn = "GRD画图";
            this.titleEn = "GRD Drawing";
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
                    Msg("请选择颜色文件！");
                    return;
                }
                GRDPreviewWindow.PreviewShow(Application.Current.MainWindow, new FileInfo(inPath), inputPath2,2,90,2);



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
    }
}
