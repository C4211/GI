using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GI.Tools
{
    public class ResourceManagerTreeNode : TreeViewItem
    {
        public ResourceManagerTreeNode()
        {
            ItemLeft = 15;
        }

        public ResourceManagerTreeNode(int level)
        {
            ItemLeft = 15;
            Level = level;
        }

        #region 获取设置依赖属性
        /// <summary>
        /// 层级从0开始
        /// </summary>
        public int Level
        {
            get { return (int)GetValue(LevelProperty); }
            set { SetValue(LevelProperty, value); }
        }

        public FileSystemInfo Path { get; set; }

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
        public DrawingBrush Icon
        {
            get { return (DrawingBrush)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }
        public GridLength LevelLeft
        {
            get { return new GridLength(this.Level * ItemLeft); }
        }
        public int ItemLeft { get; set; }
        #endregion

        #region 注册依赖属性
        public static readonly DependencyProperty LevelProperty =
            DependencyProperty.Register("Level", typeof(int), typeof(ResourceManagerTreeNode), new PropertyMetadata(0));
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(ResourceManagerTreeNode));
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(DrawingBrush), typeof(ResourceManagerTreeNode));
        #endregion
    }

    public class ResourceManagerToggleButton : ToggleButton
    {
        public ResourceManagerToggleButton()
        {
        }
        #region 获取设置依赖属性
        /// <summary>
        /// 层级从0开始
        /// </summary>
        public double IcoAngle
        {
            get { return (double)GetValue(IcoAngleProperty); }
            set { SetValue(IcoAngleProperty, value); }
        }
        #endregion

        #region 注册依赖属性
        public static readonly DependencyProperty IcoAngleProperty =
            DependencyProperty.Register("IcoAngle", typeof(double), typeof(ResourceManagerToggleButton));
        #endregion
    }
}
