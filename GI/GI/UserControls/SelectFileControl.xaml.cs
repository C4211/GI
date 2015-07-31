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
    /// SelectPathControl.xaml 的交互逻辑
    /// </summary>
    public partial class SelectFileControl : UserControl
    {
        public SelectFileControl()
        {
            InitializeComponent();
            filePath.IsKeyboardFocusedChanged += filePath_IsKeyboardFocusedChanged;
        }

        #region 绑定键盘焦点
        /// <summary>
        /// 绑定键盘焦点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void filePath_IsKeyboardFocusedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            SetValue(IsKeyboardFocusedProperty, filePath.IsKeyboardFocused);
        }
        #endregion

        #region 获取设置依赖属性
        public SolidColorBrush BorderBrush
        {
            get { return (SolidColorBrush)GetValue(BorderBrushProperty); }
            set { SetValue(BorderBrushProperty, value); }
        }
        public bool IsKeyboardFocused
        {
            get { return (bool)GetValue(IsKeyboardFocusedProperty); }
            set { SetValue(IsKeyboardFocusedProperty, value); }
        }
        #endregion

        #region 注册依赖属性
        public static readonly DependencyProperty BorderBrushProperty =
            DependencyProperty.Register("BorderBrush", typeof(SolidColorBrush), typeof(SelectFileControl));
        public static readonly DependencyProperty IsKeyboardFocusedProperty =
            DependencyProperty.Register("IsKeyboardFocused", typeof(bool), typeof(SelectFileControl));
        #endregion
    }
}
