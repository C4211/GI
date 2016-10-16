using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace GI.Tools
{
    /// <summary>
    /// HelpWindow.xaml 的交互逻辑
    /// </summary>
    public partial class HelpWindow : Window
    {
        public HelpWindow(Window owner)
        {
            InitializeComponent();
            this.Owner = owner;
            this.Title = "使用帮助";
            this.ShowInTaskbar = false;
        }

        public HelpWindow(Window owner,int index)
        {
            InitializeComponent();
            this.Owner = owner;
            this.Title = "使用帮助";
            this.ShowInTaskbar = false;
            this.f_index = index;
        }

        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private bool isClosed = false;
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (isClosed == false)
            {
                e.Cancel = true;
                Storyboard sb = (this.FindResource("GI.Window.closeStoryboard") as Storyboard).Clone();
                sb.Completed += delegate { isClosed = true; this.Close(); };
                content.BeginStoryboard(sb);
            }
            else
            {
                e.Cancel = false;
            }
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            Storyboard sb = (this.FindResource("GI.Window.Border.Focus") as Storyboard).Clone();
            content.BeginStoryboard(sb);
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            Storyboard sb = (this.FindResource("GI.Window.Border.Default") as Storyboard).Clone();
            content.BeginStoryboard(sb);
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
            {
                Storyboard sb = (this.FindResource("GI.Window.openStoryboard") as Storyboard).Clone();
                content.BeginStoryboard(sb);
            }
        }

        private void Head_Min_Click(object sender, RoutedEventArgs e)
        {
            Storyboard sb = (this.FindResource("GI.Window.closeStoryboard") as Storyboard).Clone();
            sb.Completed += delegate { this.WindowState = WindowState.Minimized; };
            content.BeginStoryboard(sb);
        }

        public void Min()
        {
            Storyboard sb = (this.FindResource("GI.Window.closeStoryboard") as Storyboard).Clone();
            sb.Completed += delegate { this.WindowState = WindowState.Minimized; };
            content.BeginStoryboard(sb);
        }

        public void Normal()
        {
            this.WindowState = WindowState.Normal;
        }

        int f_index = 0;
        private void content_Loaded(object sender, RoutedEventArgs e)
        {
            Storyboard sb = (Application.Current.FindResource("GI.Window.openStoryboard") as Storyboard).Clone();
            sb.Completed += delegate { moveToIndex(f_index); };
            content.BeginStoryboard(sb);
        }

        private bool isMove = false;
        private int index = 0;
        private void Grid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!isMove)
            {
                if (e.Delta < 0 && index < 10)
                {
                    moveToIndex(index+1);
                }
                else if (e.Delta > 0 && index > 0)
                {
                    moveToIndex(index-1);
                }
            }
        }

        private void moveToIndex(int index)
        {
            if (this.index != index)
            {
                isMove = true;
                Storyboard sb = (Application.Current.FindResource("GI.Help.Content.Storyboard") as Storyboard).Clone();
                sb.Completed += delegate { isMove = false; };
                DoubleAnimation da = (sb.Children[0]) as DoubleAnimation;
                da.To = 7200 - 720 * index;
                images.BeginStoryboard(sb);
                setMenu(index);
                this.index = index;
            }
        }

        private void setMenu(int index)
        {
            SolidColorBrush scb0 = Application.Current.FindResource("GI.Colors.Body.text") as SolidColorBrush;
            SolidColorBrush scb1 = Application.Current.FindResource("GI.Colors.Essential") as SolidColorBrush;
            Grid g = menu.Children[this.index] as Grid;
            TextBlock tb = g.Children[0] as TextBlock;
            tb.Foreground = scb0;
            Ellipse e = g.Children[1] as Ellipse;
            e.Fill = scb0;

            Grid g1 = menu.Children[index] as Grid;
            TextBlock tb1 = g1.Children[0] as TextBlock;
            tb1.Foreground = scb1;
            Ellipse e1 = g1.Children[1] as Ellipse;
            e1.Fill = scb1;
        }

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            SolidColorBrush scb = Application.Current.FindResource("GI.Colors.Essential.Press") as SolidColorBrush;
            SolidColorBrush scb1 = Application.Current.FindResource("GI.Colors.Essential") as SolidColorBrush;
            Grid g = sender as Grid;
            g.Cursor = Cursors.Hand;
            TextBlock tb = g.Children[0] as TextBlock;
            Brush scb_o = tb.Foreground;
            tb.Foreground = scb;
            Ellipse el = g.Children[1] as Ellipse;
            el.Fill = scb;
            g.MouseUp += delegate { scb_o = scb1; Grid_MouseUp(g); };
            g.MouseLeave += delegate { tb.Foreground = scb_o; el.Fill = scb_o; g.Cursor = Cursors.Arrow; };
        }

        private void Grid_MouseUp(object sender)
        {
            Grid g = sender as Grid;
            moveToIndex(Int16.Parse(g.Tag.ToString()));
        }
    }
}
