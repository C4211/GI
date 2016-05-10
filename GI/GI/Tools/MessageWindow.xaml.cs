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
using System.Windows.Shapes;

namespace GI.Tools
{
    /// <summary>
    /// MessageWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MessageWindow : Window
    {
        private MessageWindow()
        {
            InitializeComponent();
        }

        public static void Show(string message)
        {
            MessageWindow mw = new MessageWindow();
            mw.OK.Visibility = Visibility.Visible;
            mw.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            mw.messageText.Text = message;
            mw.ShowDialog();
        }
        public static void Show(Window owner,string message)
        {
            MessageWindow mw = new MessageWindow();
            mw.messageText.Text = message;
            mw.OK.Visibility = Visibility.Visible;
            mw.Owner = owner;
            mw.ShowInTaskbar = false;
            mw.ShowDialog();
        }

        public static MessageBoxResult Show(Window owner, string message, MessageBoxButton btn)
        {
            MessageWindow mw = new MessageWindow();
            mw.messageText.Text = message;
            mw.Owner = owner;
            if (btn == MessageBoxButton.OKCancel)
            {
                mw.OKCancel.Visibility = Visibility.Visible;
                mw.ShowDialog();
            }
            return mw.mbr;
        }
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
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

        private MessageBoxResult mbr = new MessageBoxResult();
        private void OKCancelOK_Click(object sender, RoutedEventArgs e)
        {
            mbr = MessageBoxResult.OK;
            this.Close();
        }

        private void OKCancelCancel_Click(object sender, RoutedEventArgs e)
        {
            mbr = MessageBoxResult.Cancel;
            this.Close();
        }

    }
}
