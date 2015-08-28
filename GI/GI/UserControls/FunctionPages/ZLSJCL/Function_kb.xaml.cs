using GI.Functions;
using GI.Tools;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace GI.UserControls
{
    /// <summary>
    /// Function_fxyf.xaml 的交互逻辑
    /// </summary>
    public partial class Function_kb : FunctionPage
    {
        public Function_kb()
        {
            InitializeComponent();
            this.titleCn = "扩边";
            this.titleEn = "Expand";
        }

        /// <summary>
        /// 0 : 输入文件
        /// 1 : 输入参数
        /// 2 : 计算中
        /// </summary>
        private int CurrentState = 0;
        private int MaxState { get { return content.Children.Count; } }
        private bool IsCanceled = false;
        private int[] data;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentState < MaxState - 1)
            {
                content.IsEnabled = false;
                buttons.IsEnabled = false;
                string inPath = inputPath1.filePath.Text;
                try
                {
                    data = Expand.Init(inPath);
                    Nx_input.Text = data[0].ToString();
                    Ny_input.Text = data[1].ToString();
                    Nx_output.Text = data[2].ToString();
                    Ny_output.Text = data[3].ToString();
                }
                catch (Exception ex)
                {
                    Msg(ex.Message);
                    content.IsEnabled = true;
                    buttons.IsEnabled = true;
                    return;
                }
                CurrentState = 1;
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
                int Nx_out = int.Parse(Nx_output.Text);
                int Ny_out = int.Parse(Ny_output.Text);
                if (Nx_out < data[0])
                    Msg("扩边后的行数小于原始行数！请重新输入！");
                else if (Ny_out < data[1])
                    Msg("扩边后的列数小于原始列数！请重新输入！");
                else
                {
                    CurrentState = MaxState;
                    IsCanceled = false;
                    next.Content = "取消";
                    HidePrevAndCancel();
                    DoExpand(Nx_out, Ny_out);
                }
            }
            else if (CurrentState == MaxState)
            {
                CurrentState = MaxState - 1;
                next.Content = "计算";
                if (task != null)
                {
                    IsCanceled = true;
                    Expand.p.Kill();
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

        private async void DoExpand(int Nx_out, int Ny_out)
        {
            HidePrevAndCancel();
            loadingBar.Show();
            string outPath = outputPath1.filePath.Text;
            try
            {
                task = Expand.Start(Nx_out, Ny_out);
                await task;
                if (IsCanceled)
                {
                    loadingBar.Hide();
                    ShowPrevAndCancel();
                    Msg("计算取消!");
                }
                else
                {
                    File.Copy(Expand.outPath, outPath, true);
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

        private void N_output_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            int tmp;
            e.Handled = !int.TryParse(e.Text, out tmp);
        }
    }
}
