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

        public string TitleText
        {
            get { return (string)GetValue(TitleTextProperty); }
            set { SetValue(TitleTextProperty, value); }
        }

        public FontFamily TitleFontFamily
        {
            get { return (FontFamily)GetValue(TitleFontFamilyProperty); }
            set { SetValue(TitleFontFamilyProperty, value); }
        }

        public double TitleFontSize
        {
            get { return (double)GetValue(TitleFontSizeProperty); }
            set { SetValue(TitleFontSizeProperty, value); }
        }

        public SolidColorBrush TitleForeground
        {
            get { return (SolidColorBrush)GetValue(TitleFontFamilyProperty); }
            set { SetValue(TitleForegroundProperty, value); }
        }

        public string SummaryText
        {
            get { return (string)GetValue(SummaryTextProperty); }
            set { SetValue(SummaryTextProperty, value); }
        }

        public FontFamily SummaryFontFamily
        {
            get { return (FontFamily)GetValue(SummaryFontFamilyProperty); }
            set { SetValue(SummaryFontFamilyProperty, value); }
        }

        public double SummaryFontSize
        {
            get { return (double)GetValue(SummaryFontSizeProperty); }
            set { SetValue(SummaryFontSizeProperty, value); }
        }
        public SolidColorBrush SummaryForeground
        {
            get { return (SolidColorBrush)GetValue(TitleFontFamilyProperty); }
            set { SetValue(SummaryForegroundProperty, value); }
        }

        public SolidColorBrush BdColor
        {
            get { return (SolidColorBrush)GetValue(BdColorProperty); }
            set { SetValue(BdColorProperty, value); }
        }
        #endregion

        #region 注册依赖属性
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(DrawingBrush), typeof(FunctionGroup));
        public static readonly DependencyProperty TitleTextProperty =
            DependencyProperty.Register("TitleText", typeof(string), typeof(FunctionGroup));
        public static readonly DependencyProperty TitleFontFamilyProperty =
            DependencyProperty.Register("TitleFontFamily", typeof(FontFamily), typeof(FunctionGroup));
        public static readonly DependencyProperty TitleFontSizeProperty =
            DependencyProperty.Register("TitleFontSize", typeof(double), typeof(FunctionGroup));
        public static readonly DependencyProperty TitleForegroundProperty =
            DependencyProperty.Register("TitleForeground", typeof(SolidColorBrush), typeof(FunctionGroup));
        public static readonly DependencyProperty SummaryTextProperty =
            DependencyProperty.Register("SummaryText", typeof(string), typeof(FunctionGroup));
        public static readonly DependencyProperty SummaryFontFamilyProperty =
            DependencyProperty.Register("SummaryFontFamily", typeof(FontFamily), typeof(FunctionGroup));
        public static readonly DependencyProperty SummaryFontSizeProperty =
            DependencyProperty.Register("SummaryFontSize", typeof(double), typeof(FunctionGroup));
        public static readonly DependencyProperty SummaryForegroundProperty =
            DependencyProperty.Register("SummaryForeground", typeof(SolidColorBrush), typeof(FunctionGroup));
        public static readonly DependencyProperty BdColorProperty =
            DependencyProperty.Register("BdColor", typeof(SolidColorBrush), typeof(FunctionGroup),
                new PropertyMetadata(new SolidColorBrush((Color)Application.Current.Resources["GI.Colors.Body.FunctionGroup.Default"])));
        #endregion
    }
}
