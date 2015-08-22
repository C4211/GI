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
        }
        Storyboard sb;
        Storyboard sbshow;
        Storyboard sbhide;
        public void Show()
        {
            Dispatcher.Invoke(
                delegate
                {
                    this.Visibility = Visibility.Visible;
                    this.BeginStoryboard(sbshow);
                    sb.Begin();
                });
        }

        public void Hide()
        {
            Dispatcher.Invoke(
                delegate 
                { 
                    sb.Stop();
                    this.BeginStoryboard(sbhide);
                    sbhide.Completed += delegate{ this.Visibility = Visibility.Hidden; };
                });
        }
    }
}
