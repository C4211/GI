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
    /// LoadingBar.xaml 的交互逻辑
    /// </summary>
    public partial class LoadingBar : UserControl
    {
        public LoadingBar()
        {
            InitializeComponent();
            sb = this.FindResource("GI.UserControl.LoadingBar.Storyboard") as Storyboard;
            sbshow = (this.FindResource("GI.UserControl.LoadingBar.Show") as Storyboard).Clone();
            sbhide = (this.FindResource("GI.UserControl.LoadingBar.Hide") as Storyboard).Clone();
            loadingshow = (this.FindResource("GI.UserControl.LoadingBar.Show") as Storyboard).Clone();
            loadinghide = (this.FindResource("GI.UserControl.LoadingBar.Hide") as Storyboard).Clone();
            titleshow = (this.FindResource("GI.UserControl.Title.Show") as Storyboard).Clone();
            titlehide = (this.FindResource("GI.UserControl.Title.Hide") as Storyboard).Clone();
        }
        Storyboard sb;
        Storyboard sbshow;
        Storyboard sbhide;
        Storyboard titleshow;
        Storyboard titlehide;
        Storyboard loadingshow;
        Storyboard loadinghide;
        public void Show()
        {
            Dispatcher.Invoke(
                delegate
                {
                    this.Visibility = Visibility.Visible;
                    this.BeginStoryboard(sbshow);
                    sb.Begin();
                    loading.BeginStoryboard(loadingshow);
                });
        }

        public void Show(string state)
        {
            Dispatcher.Invoke(
                delegate
                {
                    this.Visibility = Visibility.Visible;
                    loadingTitle.Text = state;
                    loadingTitle.BeginStoryboard(titleshow);
                    this.BeginStoryboard(sbshow);
                    sb.Begin();
                    loading.BeginStoryboard(loadingshow);
                });
        }

        public void changeState(string state)
        {
            Dispatcher.Invoke(
                delegate
                {
                    titlehide.Completed += delegate { loadingTitle.Text = state; loadingTitle.BeginStoryboard(titleshow); };
                    loadingTitle.BeginStoryboard(titlehide);
                });
        }

        public void changeState(string state, bool showloading)
        {
            Dispatcher.Invoke(
                delegate
                {
                    titlehide.Completed += delegate { loadingTitle.Text = state; loadingTitle.BeginStoryboard(titleshow); };
                    loadingTitle.BeginStoryboard(titlehide);
                    if (showloading == true)
                    {
                        sb.Begin();
                        loading.BeginStoryboard(loadingshow);
                    }
                    else
                    {
                        loadinghide.Completed += delegate { sb.Stop(); };
                        loading.BeginStoryboard(loadinghide);
                    }
                });
        }

        public void Hide()
        {
            Dispatcher.Invoke(
                delegate
                {
                    loadingTitle.BeginStoryboard(titlehide);
                    loading.BeginStoryboard(loadinghide);
                    sbhide.Completed += delegate { this.Visibility = Visibility.Hidden; sb.Stop();  };
                    this.BeginStoryboard(sbhide);
                });
        }

    }
}
