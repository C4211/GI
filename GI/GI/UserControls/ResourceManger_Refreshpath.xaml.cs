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
    /// ResourceManger_Addpath.xaml 的交互逻辑
    /// </summary>
    public partial class ResourceManger_Refreshpath : Button
    {
        public ResourceManger_Refreshpath()
        {
            InitializeComponent();
        }
        #region 获取、设置依赖属性
        public SolidColorBrush Color
        {
            get { return (SolidColorBrush)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }
        #endregion

        #region 注册依赖属性
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(SolidColorBrush), typeof(ResourceManger_Refreshpath));
        #endregion
    }
}
