using GI.Tools;
using GI.UserControls;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GI
{

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            contentStack.Push(home);
            home.zlycgz.Click += Zlycgz_Open;
        }

        #region 返回
        /// <summary>
        /// 返回层级栈
        /// </summary>
        private Stack<FunctionPage> contentStack = new Stack<FunctionPage>();
        /// <summary>
        /// 当前层级关闭动画
        /// </summary>
        private Storyboard sbBackward = (Storyboard)Application.Current.Resources["GI.Body.Content.Storyboard.Backward"];
        /// <summary>
        /// 上一层级返回动画
        /// </summary>
        private Storyboard sbBack = (Storyboard)Application.Current.Resources["GI.Body.Content.Storyboard.Back"];
        /// <summary>
        /// 返回不可用动画
        /// </summary>
        private Storyboard sbBackDefault = (Storyboard)Application.Current.Resources["GI.Head.Back.Storybord.Default"];
        /// <summary>
        /// 返回按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Group_Back(object sender, MouseButtonEventArgs e)
        {
            Page_Back();
        }
        private void Page_Back()
        {
            if (contentStack.Count > 1)
            {
                Storyboard sb = sbBackward.Clone();
                Grid current = contentStack.Pop();
                Grid prev = contentStack.Peek();
                current.BeginStoryboard(sb);
                prev.BeginStoryboard(sbBack);
                sb.Completed += delegate { body.Children.Remove(current); };
                ChangeHeadTitle((prev as FunctionPage).titleCn, (prev as FunctionPage).titleEn);
            }
            if (contentStack.Count <= 1)
                headLogo.BeginStoryboard(sbBackDefault);
        }
        /// <summary>
        /// 阻止返回按钮事件冒泡到标题栏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Preview_Group_Back(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            ((Rectangle)sender).Fill = (DrawingBrush)Application.Current.Resources["GI.Window.Head.Back.Press"];
        }
        /// <summary>
        /// 鼠标进入变成hover样式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Group_Back_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Rectangle)sender).Fill = (DrawingBrush)Application.Current.Resources["GI.Window.Head.Back.Hover"];
            if (e.LeftButton == MouseButtonState.Pressed)
                ((Rectangle)sender).Fill = (DrawingBrush)Application.Current.Resources["GI.Window.Head.Back.Press"];
        }
        /// <summary>
        ///  鼠标离开恢复默认样式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Group_Back_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Rectangle)sender).Fill = (DrawingBrush)Application.Current.Resources["GI.Window.Head.Back.Default"];
        }
        #endregion

        #region 窗体状态控制
        /// <summary>
        /// 实现顶栏可拖动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        /// <summary>
        /// 实现顶栏关闭功能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Head_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        /// <summary>
        /// 实现顶栏最小化功能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Head_Min_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        #endregion

        #region 主页按钮控制
        /// <summary>
        /// 返回变为可用动画
        /// </summary>
        private Storyboard sbBackEnable = (Storyboard)Application.Current.Resources["GI.Head.Back.Storybord.Enable"];
        /// <summary>
        /// 当前层级前进动画
        /// </summary>
        private Storyboard sbForward = (Storyboard)Application.Current.Resources["GI.Body.Content.Storyboard.Forward"];
        /// <summary>
        /// 下一层级打开动画
        /// </summary>
        private Storyboard sbOpen = (Storyboard)Application.Current.Resources["GI.Body.Content.Storyboard.Open"];
        /// <summary>
        /// 打开输入Group
        /// </summary>
        /// <param name="sender">需要打开的Group</param>
        private void Group_Open(FunctionPage sender)
        {
            if (body.Children.Contains(sender))
                body.Children.Remove(sender);
            body.Children.Add(sender);
            contentStack.Peek().BeginStoryboard(sbForward);
            sender.BeginStoryboard(sbOpen);
            contentStack.Push(sender);
            ChangeHeadTitle(sender.titleCn, sender.titleEn);
            if (contentStack.Count == 2)
                headLogo.BeginStoryboard(sbBackEnable);
        }
        /// <summary>
        /// 重力异常改正点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Zlycgz_Open(object sender, RoutedEventArgs e)
        {
            functionPage_zlycgz = null;
            functionPage_zlycgz = new FunctionPage_zlycgz();
            functionPage_zlycgz.dxgz.Click += dxgz_Click;
            functionPage_zlycgz.bggz.Click += Bggz_Open;
            functionPage_zlycgz.zkgz.Click += Zkgz_Open;
            Group_Open(functionPage_zlycgz);
        }
        private FunctionPage_zlycgz functionPage_zlycgz = null;
        /// <summary>
        /// 地形改正点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dxgz_Click(object sender, RoutedEventArgs e)
        {
            functionPage_dxgz = null;
            functionPage_dxgz = new FunctionPage_dxgz();
            functionPage_dxgz.fxyf.Click += fxyf_Click;
            functionPage_dxgz.gsjff.Click += gsjff_Click;
            functionPage_dxgz.flybhf.Click += flybhf_Click;
            Group_Open(functionPage_dxgz);
        }
        private FunctionPage_dxgz functionPage_dxgz = null;
        /// <summary>
        /// 傅里叶变换法点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void flybhf_Click(object sender, RoutedEventArgs e)
        {
            function_flybhf = null;
            function_flybhf = new Function_flybhf();
            function_flybhf.cancel.Click += delegate { Page_Back(); };
            Group_Open(function_flybhf);
        }
        private Function_flybhf function_flybhf = null;
        /// <summary>
        /// 高斯积分法点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gsjff_Click(object sender, RoutedEventArgs e)
        {
            function_gsjff = null;
            function_gsjff = new Function_gsjff();
            function_gsjff.cancel.Click += delegate { Page_Back(); };
            Group_Open(function_gsjff);
        }
        private Function_gsjff function_gsjff = null;
        /// <summary>
        /// 方形域法点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fxyf_Click(object sender, RoutedEventArgs e)
        {
            function_fxyf = null;
            function_fxyf = new Function_fxyf();
            function_fxyf.cancel.Click += delegate { Page_Back(); };
            Group_Open(function_fxyf);
        }
        private Function_fxyf function_fxyf = null;
        /// <summary>
        /// 自空改正点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Zkgz_Open(object sender, RoutedEventArgs e)
        {
            function_zkgz = null;
            function_zkgz = new Function_zkgz();
            function_zkgz.cancel.Click += delegate { Page_Back(); };
            Group_Open(function_zkgz);
        }
        private Function_zkgz function_zkgz = null;
        /// <summary>
        /// 布格改正点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Bggz_Open(object sender, RoutedEventArgs e)
        {
            function_bggz = null;
            function_bggz = new Function_bggz();
            function_bggz.cancel.Click += delegate { Page_Back(); };
            Group_Open(function_bggz);
        }
        private Function_bggz function_bggz = null;
        ///// <summary>
        ///// 重力数据处理点击事件
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void Group2_Open(object sender, RoutedEventArgs e)
        //{
        //    Group_Open(new Page(group2, "重力数据处理", "Gravity Data Processing"));
        //}
        ///// <summary>
        ///// 重力正反演计算点击事件
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void Group3_Open(object sender, RoutedEventArgs e)
        //{
        //    Group_Open(new Page(group3, "重力正反演计算", "Gravity forward calculation"));
        //}
        ///// <summary>
        ///// 重力数据解释点击事件
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void Group4_Open(object sender, RoutedEventArgs e)
        //{
        //    Group_Open(new Page(group4, "重力数据解释", "Gravity data interpretation"));
        //}
        ///// <summary>
        ///// 地质体参数计算点击事件
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void Group5_Open(object sender, RoutedEventArgs e)
        //{
        //    Group_Open(new Page(group5, "地质体参数计算", "Calculation of geological parameters"));
        //}
        ///// <summary>
        ///// Grd画图点击事件
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void Group6_Open(object sender, RoutedEventArgs e)
        //{
        //    Group_Open(new Page(group6, "GRD画图", "Grd drawing"));
        //}
        #endregion

        #region 标题栏控制
        /// <summary>
        /// 标题消失动画
        /// </summary>
        private Storyboard sbTitleDisable = (Storyboard)Application.Current.Resources["GI.Head.Title.Storybord.Disable"];
        /// <summary>
        /// 标题出现动画
        /// </summary>
        private Storyboard sbTitleDefault = (Storyboard)Application.Current.Resources["GI.Head.Title.Storybord.Default"];
        /// <summary>
        /// 修改标题栏文字
        /// </summary>
        /// <param name="title">主标题</param>
        /// <param name="subtitle">副标题</param>
        private void ChangeHeadTitle(string title, string subtitle)
        {
            Storyboard sbTitleDisable_ = sbTitleDisable.Clone();
            sbTitleDisable_.Completed += delegate
            {
                headTitle.Text = title;
                headSubTitle.Text = subtitle;
                headTitle.BeginStoryboard(sbTitleDefault);
                headSubTitle.BeginStoryboard(sbTitleDefault);
            };
            headTitle.BeginStoryboard(sbTitleDisable_, HandoffBehavior.Compose, true);
            headSubTitle.BeginStoryboard(sbTitleDisable_, HandoffBehavior.Compose, true);
        }
        #endregion




    }
}