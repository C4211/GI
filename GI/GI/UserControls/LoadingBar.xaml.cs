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
    /// LoadingBar.xaml 的交互逻辑
    /// </summary>
    public partial class LoadingBar : UserControl
    {
        public LoadingBar()
        {
            InitializeComponent();
        }

        public void Show()
        {
            Dispatcher.Invoke(delegate { this.Visibility = System.Windows.Visibility.Visible; });
        }

        public void Hide()
        {
            Dispatcher.Invoke(delegate { this.Visibility = System.Windows.Visibility.Hidden; });
        }
    }
}
