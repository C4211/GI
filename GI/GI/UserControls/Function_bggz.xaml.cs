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
    public partial class Function_bggz : FunctionPage
    {
        public Function_bggz()
        {
            InitializeComponent();
            this.titleCn = "布格改正";
            this.titleEn = "Bouguer correction";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (content.ActualWidth + content.Margin.Left > 680)
            {
                Storyboard sb = (Storyboard)this.FindResource("sb");
                ((ThicknessAnimation)sb.Children[0]).To = new Thickness(content.Margin.Left - 680, 0, 0, 0);
                sb.Begin();
                prev.Visibility = Visibility.Visible;
                next.Content = "计算";
            }
        }

        private void prev_Click(object sender, RoutedEventArgs e)
        {
            if (content.Margin.Left < 0)
            {
                Storyboard sb = (Storyboard)this.FindResource("sb");
                ((ThicknessAnimation)sb.Children[0]).To = new Thickness(content.Margin.Left + 680, 0, 0, 0);
                sb.Begin();
                prev.Visibility = Visibility.Hidden;
                next.Content = "下一步";
                next.Visibility = Visibility.Visible;
            }
        }
    }
}
