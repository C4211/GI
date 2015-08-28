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
            home.zlsjcl.Click += Zlsjcl_Open;
            home.zlzfyjs.Click += Zlzfyjs_Open;
            home.zlsjjs.Click += Zlsjjs_Open;
            home.dztcsjs.Click += Dztcsjs_Open;
            home.grdht.Click += Grdht_Open;
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
                sb.Completed += delegate { body.Children.Remove(current); current = null; };
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
            e.Handled = true;
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
            foreach (FilePreviewWindow w in this.OwnedWindows)
            {
                w.Min();
            }
            Storyboard sb = (this.FindResource("GI.Window.closeStoryboard") as Storyboard).Clone();
            sb.Completed += delegate { this.WindowState = WindowState.Minimized; };
            content.BeginStoryboard(sb);
        }
        #endregion

        #region 主页按钮控制

        #region 动画和函数
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
        #endregion

        #region 重力异常改正
        /// <summary>
        /// 重力异常改正点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Zlycgz_Open(object sender, RoutedEventArgs e)
        {
            functionPage_zlycgz = new FunctionPage_zlycgz();
            functionPage_zlycgz.dxgz.Click += dxgz_Click;
            functionPage_zlycgz.bggz.Click += Bggz_Open;
            functionPage_zlycgz.zkgz.Click += Zkgz_Open;
            Group_Open(functionPage_zlycgz);
            functionPage_zlycgz = null;
        }
        private FunctionPage_zlycgz functionPage_zlycgz = null;
        /// <summary>
        /// 地形改正点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dxgz_Click(object sender, RoutedEventArgs e)
        {
            functionPage_dxgz = new FunctionPage_dxgz();
            functionPage_dxgz.fxyf.Click += fxyf_Click;
            functionPage_dxgz.gsjff.Click += gsjff_Click;
            functionPage_dxgz.flybhf.Click += flybhf_Click;
            Group_Open(functionPage_dxgz);
            functionPage_dxgz = null;
        }
        private FunctionPage_dxgz functionPage_dxgz = null;
        /// <summary>
        /// 傅里叶变换法点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void flybhf_Click(object sender, RoutedEventArgs e)
        {
            function_flybhf = new Function_flybhf();
            function_flybhf.cancel.Click += delegate { Page_Back(); };
            Group_Open(function_flybhf);
            function_flybhf = null;
        }
        private Function_flybhf function_flybhf = null;
        /// <summary>
        /// 高斯积分法点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gsjff_Click(object sender, RoutedEventArgs e)
        {
            function_gsjff = new Function_gsjff();
            function_gsjff.cancel.Click += delegate { Page_Back(); };
            Group_Open(function_gsjff);
            function_gsjff = null;
        }
        private Function_gsjff function_gsjff = null;
        /// <summary>
        /// 方形域法点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fxyf_Click(object sender, RoutedEventArgs e)
        {
            function_fxyf = new Function_fxyf();
            function_fxyf.cancel.Click += delegate { Page_Back(); };
            Group_Open(function_fxyf);
            function_fxyf = null;
        }
        private Function_fxyf function_fxyf = null;
        /// <summary>
        /// 自空改正点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Zkgz_Open(object sender, RoutedEventArgs e)
        {
            function_zkgz = new Function_zkgz();
            function_zkgz.cancel.Click += delegate { Page_Back(); };
            Group_Open(function_zkgz);
            function_zkgz = null;
        }
        private Function_zkgz function_zkgz = null;
        /// <summary>
        /// 布格改正点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Bggz_Open(object sender, RoutedEventArgs e)
        {
            function_bggz = new Function_bggz();
            function_bggz.cancel.Click += delegate { Page_Back(); };
            Group_Open(function_bggz);
            function_bggz = null;
        }
        private Function_bggz function_bggz = null;
        #endregion

        #region 重力数据处理
        /// <summary>
        /// 重力数据处理点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Zlsjcl_Open(object sender, RoutedEventArgs e)
        {
            functionPage_zlsjcl = new FunctionPage_zlsjcl();
            functionPage_zlsjcl.kb.Click += kb_Click;
            functionPage_zlsjcl.lb.Click += lb_Click;
            functionPage_zlsjcl.xsyt.Click += xsyt_Click;
            functionPage_zlsjcl.ds.Click += ds_Click;
            functionPage_zlsjcl.glpfx.Click +=glpfx_Click;
            Group_Open(functionPage_zlsjcl);
            functionPage_zlsjcl = null;
        }
        private FunctionPage_zlsjcl functionPage_zlsjcl = null;

        /// <summary>
        /// 功率谱分析点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void glpfx_Click(object sender, RoutedEventArgs e)
        {
            function_glpfx = new Function_glpfx();
            function_glpfx.cancel.Click += delegate { Page_Back(); };
            Group_Open(function_glpfx);
            function_glpfx = null;
        }
        private Function_glpfx function_glpfx = null;

        /// <summary>
        /// 导数点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ds_Click(object sender, RoutedEventArgs e)
        {
            functionPage_ds = new FunctionPage_ds();
            functionPage_ds.fxds.Click += fxds_Click;
            functionPage_ds.cxdskjy.Click += cxdskjy_Click;
            functionPage_ds.cxdsply.Click += cxdsply_Click;
            Group_Open(functionPage_ds);
            functionPage_ds = null;
        }
        private FunctionPage_ds functionPage_ds = null;

        /// <summary>
        /// 垂向导数空间域
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cxdskjy_Click(object sender, RoutedEventArgs e)
        {
            function_cxdskjy = new Function_cxdskjy();
            function_cxdskjy.cancel.Click += delegate { Page_Back(); };
            Group_Open(function_cxdskjy);
            function_cxdskjy = null;
        }
        private Function_cxdskjy function_cxdskjy = null;

        /// <summary>
        /// 垂向导数（频率域）点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cxdsply_Click(object sender, RoutedEventArgs e)
        {
            function_cxdsply = new Function_cxdsply();
            function_cxdsply.cancel.Click += delegate { Page_Back(); };
            Group_Open(function_cxdsply);
            function_cxdsply = null;
        }
        private Function_cxdsply function_cxdsply = null;

        /// <summary>
        /// 方向导数点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fxds_Click(object sender, RoutedEventArgs e)
        {
            function_fxds = new Function_fxds();
            function_fxds.cancel.Click += delegate { Page_Back(); };
            Group_Open(function_fxds);
            function_fxds = null;
        }
        private Function_fxds function_fxds = null;

        /// <summary>
        /// 向上延拓点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void xsyt_Click(object sender, RoutedEventArgs e)
        {
            function_xsyt = new Function_xsyt();
            function_xsyt.cancel.Click += delegate { Page_Back(); };
            Group_Open(function_xsyt);
            function_xsyt = null;
        }
        private Function_xsyt function_xsyt = null;

        /// <summary>
        /// 滤波点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lb_Click(object sender, RoutedEventArgs e)
        {
            functionPage_lb = new FunctionPage_lb();
            functionPage_lb.wnlb.Click += wnlb_Click;
            functionPage_lb.pplb.Click += pplb_Click;
            functionPage_lb.zzhlb.Click += zzhlb_Click;
            functionPage_lb.bcyhlb.Click += bcyhlb_Click;
            Group_Open(functionPage_lb);
            functionPage_lb = null;
        }
        private FunctionPage_lb functionPage_lb = null;

        /// <summary>
        /// 匹配滤波点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pplb_Click(object sender, RoutedEventArgs e)
        {
            function_pplb = new Function_pplb();
            function_pplb.cancel.Click += delegate { Page_Back(); };
            Group_Open(function_pplb);
            function_pplb = null;
        }
        private Function_pplb function_pplb = null;

        /// <summary>
        /// 补偿圆滑滤波
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bcyhlb_Click(object sender, RoutedEventArgs e)
        {
            function_bcyhlb = new Function_bcyhlb();
            function_bcyhlb.cancel.Click += delegate { Page_Back(); };
            Group_Open(function_bcyhlb);
            function_bcyhlb = null;
        }
        private Function_bcyhlb function_bcyhlb = null;

        /// <summary>
        /// 正则化滤波点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void zzhlb_Click(object sender, RoutedEventArgs e)
        {
            function_zzhlb = new Function_zzhlb();
            function_zzhlb.cancel.Click += delegate { Page_Back(); };
            Group_Open(function_zzhlb);
            function_zzhlb = null;
        }
        private Function_zzhlb function_zzhlb = null;

        /// <summary>
        /// 维纳滤波点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void wnlb_Click(object sender, RoutedEventArgs e)
        {
            function_wnlb = new Function_wnlb();
            function_wnlb.cancel.Click += delegate { Page_Back(); };
            Group_Open(function_wnlb);
            function_wnlb = null;
        }
        private Function_wnlb function_wnlb = null;

        /// <summary>
        /// 扩边点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void kb_Click(object sender, RoutedEventArgs e)
        {
            function_kb = new Function_kb();
            function_kb.cancel.Click += delegate { Page_Back(); };
            Group_Open(function_kb);
            function_kb = null;
        }
        private Function_kb function_kb = null;
       
        #endregion

        #region 重力正反演计算
        /// <summary>
        /// 重力正反演计算点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Zlzfyjs_Open(object sender, RoutedEventArgs e)
        {
            functionPage_zlzfyjs = new FunctionPage_zlzfyjs();
            functionPage_zlzfyjs.jmzlzy.Click += jmzlzy_Click;
            functionPage_zlzfyjs.zljmfy.Click += zljmfy_Click;
            Group_Open(functionPage_zlzfyjs);
            functionPage_zlzfyjs = null;
        }
        private FunctionPage_zlzfyjs functionPage_zlzfyjs = null;

        /// <summary>
        /// 界面重力正演点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void jmzlzy_Click(object sender, RoutedEventArgs e)
        {
            function_jmzlzy = new Function_jmzlzy();
            function_jmzlzy.cancel.Click += delegate { Page_Back(); };
            Group_Open(function_jmzlzy);
            function_jmzlzy = null;
        }
        private Function_jmzlzy function_jmzlzy = null;

        /// <summary>
        /// 重力界面反演点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void zljmfy_Click(object sender, RoutedEventArgs e)
        {
            function_zljmfy = new Function_zljmfy();
            function_zljmfy.cancel.Click += delegate { Page_Back(); };
            Group_Open(function_zljmfy);
            function_zljmfy = null;
        }
        private Function_zljmfy function_zljmfy = null;
        #endregion

        #region 重力数据解释
        /// <summary>
        /// 重力数据解释点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Zlsjjs_Open(object sender, RoutedEventArgs e)
        {
            functionPage_zlsjjs = new FunctionPage_zlsjjs();
            functionPage_zlsjjs.ewjm.Click += ewjm_Click;
            functionPage_zlsjjs.swjm.Click += swjm_Click;
            Group_Open(functionPage_zlsjjs);
            functionPage_zlsjjs = null;
        }
        private FunctionPage_zlsjjs functionPage_zlsjjs = null;

        /// <summary>
        /// 二维建模点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ewjm_Click(object sender, RoutedEventArgs e)
        {
            function_ewjm = new Function_ewjm();
            function_ewjm.cancel.Click += delegate { Page_Back(); };
            Group_Open(function_ewjm);
            function_ewjm = null;
        }
        private Function_ewjm function_ewjm = null;
        /// <summary>
        /// 三维建模点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void swjm_Click(object sender, RoutedEventArgs e)
        {
            function_swjm = new Function_swjm();
            function_swjm.cancel.Click += delegate { Page_Back(); };
            Group_Open(function_swjm);
            function_swjm = null;
        }
        private Function_swjm function_swjm = null;
        #endregion

        #region 地质体参数计算
        /// <summary>
        /// 地质体参数计算点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Dztcsjs_Open(object sender, RoutedEventArgs e)
        {
            functionPage_dztcsjs = new FunctionPage_dztcsjs();
            functionPage_dztcsjs.dztzlzltdjs.Click += dztzlzltdjs_Click;
            functionPage_dztcsjs.dztcsfy.Click += dztcsfy_Click;
            Group_Open(functionPage_dztcsjs);
            functionPage_dztcsjs = null;
        }
        private FunctionPage_dztcsjs functionPage_dztcsjs = null;

        /// <summary>
        /// 地质体重力/重力梯度计算点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dztzlzltdjs_Click(object sender, RoutedEventArgs e)
        {
            function_dztzlzltdjs = new Function_dztzlzltdjs();
            function_dztzlzltdjs.cancel.Click += delegate { Page_Back(); };
            Group_Open(function_dztzlzltdjs);
            function_dztzlzltdjs = null;
        }
        private Function_dztzlzltdjs function_dztzlzltdjs = null;

        /// <summary>
        /// 地质体参数反演
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dztcsfy_Click(object sender, RoutedEventArgs e)
        {
            function_dztcsfy = new Function_dztcsfy();
            function_dztcsfy.cancel.Click += delegate { Page_Back(); };
            Group_Open(function_dztcsfy);
            function_dztcsfy = null;
        }
        private Function_dztcsfy function_dztcsfy = null;
        #endregion

        #region Grd画图
        ///<summary>
        /// Grd画图点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grdht_Open(object sender, RoutedEventArgs e)
        {
            function_grdht = new Function_grdht();
            function_grdht.cancel.Click += delegate { Page_Back(); };
            Group_Open(function_grdht);
            function_grdht = null;
        }
        private Function_grdht function_grdht = null;
        #endregion
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

        #region Window事件效果
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Storyboard sb = (this.FindResource("GI.Window.openStoryboard") as Storyboard).Clone();
            content.BeginStoryboard(sb);
        }
        #endregion

        private void Window_Activated(object sender, EventArgs e)
        {
            Storyboard sb = (this.FindResource("GI.MainWindow.Border.Focus") as Storyboard).Clone();
            content.BeginStoryboard(sb);
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            Storyboard sb = (this.FindResource("GI.MainWindow.Border.Default") as Storyboard).Clone();
            content.BeginStoryboard(sb);
        }

        private bool isClosed = false;
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (isClosed == false)
            {
                e.Cancel = true;
                Storyboard sb = (this.FindResource("GI.Window.closeStoryboard") as Storyboard).Clone();
                sb.Completed += delegate { isClosed = true; this.Close(); };
                content.BeginStoryboard(sb);
            }
            else
            {
                e.Cancel = false;
            }
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if(this.WindowState == WindowState.Normal)
            {
                Storyboard sb = (this.FindResource("GI.Window.openStoryboard") as Storyboard).Clone();
                content.BeginStoryboard(sb);
                foreach (FilePreviewWindow w in this.OwnedWindows)
                {
                    w.Normal();
                }
            }
        }
    }
}