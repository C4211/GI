using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using GI.Tools;

namespace GI
{
    /// <summary>
    /// grdtest.xaml 的交互逻辑
    /// </summary>
    public partial class grdtest : Window
    {


        public grdtest()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            Grd grd = new Grd(path.filePath.Text);
            WriteableBitmap wb = grd.GrdImage();
            img.Source = wb;

        }

        
    }
}
