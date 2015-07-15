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

namespace GI
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class FunctionGroup : UserControl
    {
        public FunctionGroup()
        {
            InitializeComponent();
        }

        #region 获取、设置依赖属性
        public DrawingBrush Icon
        {
            get { return (DrawingBrush)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public string Summary
        {
            get { return (string)GetValue(SummaryProperty); }
            set { SetValue(SummaryProperty, value); }
        }

        public Visibility Bd
        {
            get { return (Visibility)GetValue(BdProperty); }
            set { SetValue(BdProperty, value); }
        }
        #endregion

        #region 注册依赖属性
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(DrawingBrush), typeof(FunctionGroup));
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(FunctionGroup));
        public static readonly DependencyProperty SummaryProperty = DependencyProperty.Register("Summary", typeof(string), typeof(FunctionGroup));
        public static readonly DependencyProperty BdProperty = DependencyProperty.Register("Bd", typeof(Visibility), typeof(FunctionGroup), new PropertyMetadata(Visibility.Collapsed));
        #endregion
    }
}
