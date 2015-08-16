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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GI.UserControls
{
    /// <summary>
    /// FunctionPage.xaml 的交互逻辑
    /// </summary>
    public partial class FunctionPage_home : FunctionPage
    {
        public FunctionPage_home()
        {
            InitializeComponent();
            this.titleCn = Application.Current.Resources["GI.Detail.Name.Cn"] as string;
            this.titleEn = Application.Current.Resources["GI.Detail.Name.En"] as string;
        }


        private void About_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MessageWindow.Show(Application.Current.MainWindow, "版权所有：\n中国科学院测量与地球物理研究所");
        }
        
    }
}
