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

namespace GI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 实现顶栏可拖动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        /// <summary>
        /// 实现顶栏关闭功能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Head_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        /// <summary>
        /// 实现顶栏最小化功能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Head_Min_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        /// <summary>
        /// 重力异常改正点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Group1_Click(object sender, MouseButtonEventArgs e)
        {
            home.BeginStoryboard((Storyboard)Application.Current.Resources["GI.Body.Content.Storyboard.Forward"]);
            group1.BeginStoryboard((Storyboard)Application.Current.Resources["GI.Body.Content.Storyboard.Open"]);
        }

        private void Group1_Back(object sender, RoutedEventArgs e)
        {
            group1.BeginStoryboard((Storyboard)Application.Current.Resources["GI.Body.Content.Storyboard.Backward"]);
            home.BeginStoryboard((Storyboard)Application.Current.Resources["GI.Body.Content.Storyboard.Back"]);
        }

        private void Group2_Click(object sender, MouseButtonEventArgs e)
        {
            home.BeginStoryboard((Storyboard)Application.Current.Resources["GI.Body.Content.Storyboard.Forward"]);
            group2.BeginStoryboard((Storyboard)Application.Current.Resources["GI.Body.Content.Storyboard.Open"]);
        }

        private void Group2_Back(object sender, RoutedEventArgs e)
        {
            group2.BeginStoryboard((Storyboard)Application.Current.Resources["GI.Body.Content.Storyboard.Backward"]);
            home.BeginStoryboard((Storyboard)Application.Current.Resources["GI.Body.Content.Storyboard.Back"]);
        }

        private void Group3_Click(object sender, MouseButtonEventArgs e)
        {
            home.BeginStoryboard((Storyboard)Application.Current.Resources["GI.Body.Content.Storyboard.Forward"]);
            group3.BeginStoryboard((Storyboard)Application.Current.Resources["GI.Body.Content.Storyboard.Open"]);
        }

        private void Group3_Back(object sender, RoutedEventArgs e)
        {
            group3.BeginStoryboard((Storyboard)Application.Current.Resources["GI.Body.Content.Storyboard.Backward"]);
            home.BeginStoryboard((Storyboard)Application.Current.Resources["GI.Body.Content.Storyboard.Back"]);
        }
        private void Group4_Click(object sender, MouseButtonEventArgs e)
        {
            home.BeginStoryboard((Storyboard)Application.Current.Resources["GI.Body.Content.Storyboard.Forward"]);
            group4.BeginStoryboard((Storyboard)Application.Current.Resources["GI.Body.Content.Storyboard.Open"]);
        }

        private void Group4_Back(object sender, RoutedEventArgs e)
        {
            group4.BeginStoryboard((Storyboard)Application.Current.Resources["GI.Body.Content.Storyboard.Backward"]);
            home.BeginStoryboard((Storyboard)Application.Current.Resources["GI.Body.Content.Storyboard.Back"]);
        }
        private void Group5_Click(object sender, MouseButtonEventArgs e)
        {
            home.BeginStoryboard((Storyboard)Application.Current.Resources["GI.Body.Content.Storyboard.Forward"]);
            group5.BeginStoryboard((Storyboard)Application.Current.Resources["GI.Body.Content.Storyboard.Open"]);
        }

        private void Group5_Back(object sender, RoutedEventArgs e)
        {
            group5.BeginStoryboard((Storyboard)Application.Current.Resources["GI.Body.Content.Storyboard.Backward"]);
            home.BeginStoryboard((Storyboard)Application.Current.Resources["GI.Body.Content.Storyboard.Back"]);
        }

        private void Group6_Click(object sender, MouseButtonEventArgs e)
        {
            home.BeginStoryboard((Storyboard)Application.Current.Resources["GI.Body.Content.Storyboard.Forward"]);
            group6.BeginStoryboard((Storyboard)Application.Current.Resources["GI.Body.Content.Storyboard.Open"]);
        }

        private void Group6_Back(object sender, RoutedEventArgs e)
        {
            group6.BeginStoryboard((Storyboard)Application.Current.Resources["GI.Body.Content.Storyboard.Backward"]);
            home.BeginStoryboard((Storyboard)Application.Current.Resources["GI.Body.Content.Storyboard.Back"]);
        }
    }
}
