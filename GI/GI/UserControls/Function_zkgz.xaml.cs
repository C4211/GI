using GI.Tools;
using System;
using System.Collections.Generic;
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
    public partial class Function_zkgz : FunctionPage
    {
        public Function_zkgz()
        {
            InitializeComponent();
            this.titleCn = "自空改正";
            this.titleEn = "Free Air Correction";
        }

        /// <summary>
        /// 0 : 输入文件
        /// 1 : 输入参数
        /// </summary>
        private int CurrentState = 0;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentState == 0)
            {
                next.IsEnabled = false;
                CurrentState = 1;
                content.Children[CurrentState].Visibility = Visibility.Visible;
                Storyboard sb = ((Storyboard)this.FindResource("sb")).Clone();
                ((ThicknessAnimation)sb.Children[0]).To = new Thickness(-CurrentState * 680, 0, 0, 0);
                sb.Completed += delegate { content.Children[CurrentState - 1].Visibility = Visibility.Hidden; next.IsEnabled = true; };
                content.BeginStoryboard(sb);
                prev.Visibility = Visibility.Visible;
                next.Content = "计算";
                return;
            }
            else if (CurrentState == 1)
            {
                DoFreeAirCorrection();
            }
        }

        private void prev_Click(object sender, RoutedEventArgs e)
        {

            if (CurrentState > 0)
            {
                next.IsEnabled = false;
                CurrentState -= 1;
                content.Children[CurrentState].Visibility = Visibility.Visible;
                Storyboard sb = ((Storyboard)this.FindResource("sb")).Clone();
                ((ThicknessAnimation)sb.Children[0]).To = new Thickness(-CurrentState * 680, 0, 0, 0);
                sb.Completed += delegate { content.Children[CurrentState + 1].Visibility = Visibility.Hidden; next.IsEnabled = true; };
                content.BeginStoryboard(sb);
                if (CurrentState <= 0)
                    prev.Visibility = Visibility.Hidden;
                next.Content = "下一步";
                next.Visibility = Visibility.Visible;
            }
        }
        private async void DoFreeAirCorrection()
        {
            string inPath = inputPath1.filePath.Text;
            string outPath = outputPath1.filePath.Text;
            int choice = 1;
            if (!FileNameFilter.CheckFileSuffix(inPath))
                Msg("输入文件类型不正确！");
            else if (!FileNameFilter.CheckFileExistence(inPath))
                Msg("输入文件路径不存在！");
            else
            {
                try
                {
                    if (choice1.IsChecked == true)
                        choice = 1;
                    else if (choice2.IsChecked == true)
                        choice = 2;
                    else if (choice2.IsChecked == true)
                        choice = 3;
                    else if (choice2.IsChecked == true)
                        choice = 4;
                    task = null;
                    task = FreeAirCorrectionStart.Start(inPath, outPath, choice);
                    await task;
                    Msg("计算完成！");
                }
                catch (Exception e)
                {
                    Msg(e.Message);
                }
            }
        }

        private Task task = null;

        private void Msg(string msg)
        {
            Dispatcher.Invoke(delegate { MessageBox.Show(Application.Current.MainWindow, msg); });
        }
    }
}
