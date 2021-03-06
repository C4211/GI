﻿using System;
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
        public static string DefaultPath
        {
            get;
            set;
        }

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
        public new SolidColorBrush BorderBrush
        {
            get { return (SolidColorBrush)GetValue(BorderBrushProperty); }
            set { SetValue(BorderBrushProperty, value); }
        }
        public new bool IsKeyboardFocused
        {
            get { return (bool)GetValue(IsKeyboardFocusedProperty); }
            set { SetValue(IsKeyboardFocusedProperty, value); }
        }
        public string OpenFileFilter
        {
            get { return (string)GetValue(OpenFileFilterProperty); }
            set { SetValue(OpenFileFilterProperty, value); }
        }
        #endregion

        #region 注册依赖属性
        public static readonly new DependencyProperty BorderBrushProperty =
            DependencyProperty.Register("BorderBrush", typeof(SolidColorBrush), typeof(SelectFileControl));
        public static readonly new DependencyProperty IsKeyboardFocusedProperty =
            DependencyProperty.Register("IsKeyboardFocused", typeof(bool), typeof(SelectFileControl));
        public static readonly DependencyProperty OpenFileFilterProperty =
            DependencyProperty.Register("OpenFileFilter", typeof(string), typeof(SelectFileControl),
                new PropertyMetadata("可用文件(*.txt,*.grd,*.dat)|*.txt;*.grd;*.dat|txt文件(*.txt)|*.txt|grd文件(*.grd)|*.grd|dat文件(*.dat)|*.dat|所有文件(*.*)|*.*"));
        #endregion

        private void selectButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            if (DefaultPath == null || DefaultPath == "")
            {
                ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
            else
            {
                ofd.InitialDirectory = DefaultPath;
            }
            ofd.Filter = this.OpenFileFilter;
            ofd.RestoreDirectory = true;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                filePath.Text = ofd.FileName;
            }
        }

        private void filePath_PreviewDrop(object sender, DragEventArgs e)
        {
            filePath.Clear();
        }
    }
}
