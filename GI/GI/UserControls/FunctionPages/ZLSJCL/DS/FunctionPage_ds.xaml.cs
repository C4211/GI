﻿using GI.Tools;
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
    public partial class FunctionPage_ds : FunctionPage
    {
        /// <summary>
        /// 构造函数，设置中英文标题
        /// </summary>
        public FunctionPage_ds()
        {
            InitializeComponent();
            this.titleCn = "导数";
            this.titleEn = "Derivatives";
        }

    }
}
