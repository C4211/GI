using GI.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// ResourceManager.xaml 的交互逻辑
    /// </summary>
    public partial class ResourceManager : Grid
    {
        public ResourceManager()
        {
            InitializeComponent();
            roots = UserConfigUtil.GetPaths();
        }

        private List<DirectoryInfo> roots = new List<DirectoryInfo>();

        #region 控制LoadingBar
        private void StartLoading()
        {
            Dispatcher.Invoke(delegate
            {
                loadingBar.Show();
            });
        }

        private void StopLoading()
        {
            Dispatcher.Invoke(delegate
            {
                loadingBar.Hide();
            });
        }
        #endregion

        private async void resourceTree_Loaded(object sender, RoutedEventArgs e)
        {
            StartLoading();
            await Task.Factory.StartNew(RefreshTreeView);
            StopLoading();
        }

        private async void ResourceManger_Refreshpath_Click(object sender, RoutedEventArgs e)
        {
            StartLoading();
            await Task.Factory.StartNew(RefreshTreeView);
            StopLoading();
        }

        public void RefreshTreeView()
        {
            List<ResourceTreeNode> result = null;
            List<ResourceManagerTreeNode> list = null;
            ResourceManagerTreeNode parentNode;
            //try
            //{
            Thread.Sleep(600);
            result = LoadResourceTree(roots);
            Dispatcher.Invoke(delegate
            {
                    //resourceTree.Items.Refresh();
                    resourceTree.Items.Clear();
                foreach (var node in result)
                {
                    parentNode = new ResourceManagerTreeNode();
                    parentNode.Path = node.Info;
                    parentNode.Title = node.Info.Name;
                    list = FillDataToResourceTreeView(node);
                    foreach (var l in list)
                    {
                        parentNode.Items.Add(l);
                    }
                    if (parentNode.ContextMenu == null)
                    {
                        ContextMenu menu = new ContextMenu();
                        MenuItem item = new MenuItem();
                        item.Header = "移除路径";
                        item.Click += item_Click;
                        menu.Items.Add(item);
                        parentNode.ContextMenu = menu;
                    }
                    else
                    {
                        MenuItem item = new MenuItem();
                        item.Header = "移除路径";
                        item.Click += item_Click;
                        parentNode.ContextMenu.Items.Add(item);
                    }
                    MenuItem item1 = new MenuItem();
                    item1.Header = "打开目录";
                    item1.Click += delegate { Open_File(parentNode.Path.FullName); };
                    parentNode.ContextMenu.Items.Add(item1);
                    resourceTree.Items.Add(parentNode);
                }
            });
            //}
            //catch
            //{
            //    Dispatcher.Invoke(delegate { MessageWindow.Show(Application.Current.MainWindow, @"读取目录失败！"); });
            //}
            //finally
            //{
            if (result != null)
                result = null;
            if (list != null)
                list = null;
            UserConfigUtil.SavePaths(roots);
            //}
        }

        /// <summary>
        /// 右键菜单打开文件位置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Open_File(string fileFullName)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo("Explorer.exe");
                psi.Arguments = "/root,/select," + fileFullName;
                System.Diagnostics.Process.Start(psi);
            }
            catch (Exception e)
            {
                MessageWindow.Show(Application.Current.MainWindow, e.Message);
            }
        }
        private void Open_FilePath(string fileFullName)
        {
            try
            {
                ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("Explorer.exe");
                psi.Arguments = "/n,/select," + fileFullName;
                System.Diagnostics.Process.Start(psi);
            }
            catch (Exception e)
            {
                MessageWindow.Show(Application.Current.MainWindow, e.Message);
            }
        }

        /// <summary>
        /// 右键菜单：删除
        /// </summary>
        private async void item_Click(object sender, RoutedEventArgs e)
        {
            ResourceManagerTreeNode node = resourceTree.SelectedItem as ResourceManagerTreeNode;
            if (node != null)
            {
                while (node.Parent as ResourceManagerTreeNode != null && (node.Parent as ResourceManagerTreeNode).Parent != null)
                    node = node.Parent as ResourceManagerTreeNode;
                //MessageBox.Show(node.Path.FullName);
                if (MessageWindow.Show(Application.Current.MainWindow, "确定从资源管理器中要移除该路径？\n" + node.Path.FullName, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    StartLoading();
                    for (int i = 0; i < roots.Count; i++)
                    {
                        var root = roots[i];
                        if (root.FullName == node.Path.FullName)
                        {
                            roots.RemoveAt(i);
                            break;
                        }
                    }
                    await Task.Factory.StartNew(RefreshTreeView);
                    StopLoading();
                }
            }
        }

        private List<ResourceManagerTreeNode> FillDataToResourceTreeView(ResourceTreeNode node, int level = 1)
        {
            List<ResourceManagerTreeNode> result = new List<ResourceManagerTreeNode>();
            List<ResourceManagerTreeNode> list = null;
            foreach (var child in node.Children)
            {
                ResourceManagerTreeNode childNode = null;
                childNode = new ResourceManagerTreeNode(level);
                childNode.Path = child.Info;
                childNode.Title = child.Info.Name;
                //if (child.IsDir && child.Children.Count == 0)
                //    continue;
                if (!child.IsDir)
                {
                    if (child.Info.Extension.Equals(".dat", StringComparison.OrdinalIgnoreCase))
                    {
                        childNode.Icon = Application.Current.FindResource("GI.ResourceManager.Icons.Dat") as DrawingBrush;
                        if (childNode.ContextMenu == null)
                        {
                            ContextMenu menu = new ContextMenu();
                            MenuItem item = new MenuItem();
                            item.Header = "预览文件";
                            item.Click += delegate { Preview_File(childNode); };
                            menu.Items.Add(item);
                            childNode.ContextMenu = menu;
                        }
                        else
                        {
                            MenuItem item = new MenuItem();
                            item.Header = "预览文件";
                            item.Click += delegate { Preview_File(childNode); };
                            childNode.ContextMenu.Items.Add(item);
                        }
                    }
                    else if (child.Info.Extension.Equals(".txt", StringComparison.OrdinalIgnoreCase))
                    {
                        childNode.Icon = Application.Current.FindResource("GI.ResourceManager.Icons.Txt") as DrawingBrush;
                        if (childNode.ContextMenu == null)
                        {
                            ContextMenu menu = new ContextMenu();
                            MenuItem item = new MenuItem();
                            item.Header = "预览文件";
                            item.Click += delegate { Preview_File(childNode); };
                            menu.Items.Add(item);
                            childNode.ContextMenu = menu;
                        }
                        else
                        {
                            MenuItem item = new MenuItem();
                            item.Header = "预览文件";
                            item.Click += delegate { Preview_File(childNode); };
                            childNode.ContextMenu.Items.Add(item);
                        }
                    }
                    else if (child.Info.Extension.Equals(".grd", StringComparison.OrdinalIgnoreCase))
                    {
                        childNode.Icon = Application.Current.FindResource("GI.ResourceManager.Icons.Grd") as DrawingBrush;
                        if (childNode.ContextMenu == null)
                        {
                            ContextMenu menu = new ContextMenu();
                            MenuItem item = new MenuItem();
                            item.Header = "预览文件";
                            item.Click += delegate { Preview_GRD(childNode); };
                            menu.Items.Add(item);
                            childNode.ContextMenu = menu;
                        }
                        else
                        {
                            MenuItem item = new MenuItem();
                            item.Header = "预览文件";
                            item.Click += delegate { Preview_GRD(childNode); };
                            childNode.ContextMenu.Items.Add(item);
                        }
                    }
                    else
                    {
                        childNode.Icon = Application.Current.FindResource("GI.ResourceManager.Icons.Other") as DrawingBrush;
                    }
                    if (childNode.ContextMenu == null)
                    {
                        ContextMenu menu = new ContextMenu();
                        MenuItem item = new MenuItem();
                        item.Header = "打开文件";
                        item.Click += delegate { Open_File(childNode.Path.FullName); };
                        menu.Items.Add(item);
                        MenuItem item1 = new MenuItem();
                        item1.Header = "打开位置";
                        item1.Click += delegate { Open_FilePath(childNode.Path.FullName); };
                        menu.Items.Add(item1);
                        childNode.ContextMenu = menu;
                    }
                    else
                    {
                        MenuItem item = new MenuItem();
                        item.Header = "打开文件";
                        item.Click += delegate { Open_File(childNode.Path.FullName); };
                        childNode.ContextMenu.Items.Add(item);
                        MenuItem item1 = new MenuItem();
                        item1.Header = "打开位置";
                        item1.Click += delegate { Open_FilePath(childNode.Path.FullName); };
                        childNode.ContextMenu.Items.Add(item1);
                    }

                    childNode.PreviewMouseLeftButtonDown += delegate
                    {
                        DragDrop.DoDragDrop(childNode, childNode.Path.FullName, DragDropEffects.All);
                    };
                    childNode.MouseDoubleClick += delegate { Preview_File(childNode); };
                }
                else
                {
                    if (childNode.ContextMenu == null)
                    {
                        ContextMenu menu = new ContextMenu();
                        MenuItem item = new MenuItem();
                        item.Header = "打开目录";
                        item.Click += delegate { Open_File(childNode.Path.FullName); };
                        menu.Items.Add(item);
                        childNode.ContextMenu = menu;
                    }
                    else
                    {
                        MenuItem item = new MenuItem();
                        item.Header = "打开目录";
                        item.Click += delegate { Open_File(childNode.Path.FullName); };
                        childNode.ContextMenu.Items.Add(item);
                    }
                }
                list = FillDataToResourceTreeView(child, level + 1);
                foreach (var l in list)
                {
                    childNode.Items.Add(l);
                }
                result.Add(childNode);

            }

            list = null;
            return result;
        }


        /// <summary>
        /// 预览文件点击事件
        /// </summary>
        /// <param name="rmtn"></param>
        private void Preview_File(ResourceManagerTreeNode rmtn)
        {
            FilePreviewWindow.PreviewShow(rmtn);
        }

        /// <summary>
        /// 预览Grd点击事件
        /// </summary>
        /// <param name="rmtn"></param>
        private void Preview_GRD(ResourceManagerTreeNode rmtn)
        {
            GRDPreviewWindow.PreviewShow(rmtn,3,90);
        }

        private List<ResourceTreeNode> LoadResourceTree(List<DirectoryInfo> Roots)
        {
            List<ResourceTreeNode> list = new List<ResourceTreeNode>();
            ResourceTreeNode rootNode;
            List<DirectoryInfo> dirs;
            List<FileInfo> files;
            for (int i = 0; i < Roots.Count; i++)
            {
                DirectoryInfo rootDir = Roots[i];
                try
                {
                    if (rootDir.Attributes.HasFlag(FileAttributes.Hidden))
                    {
                        Roots.RemoveAt(i);
                        i--;
                        continue;
                    }
                    rootNode = new ResourceTreeNode(rootDir);
                    dirs = rootDir.GetDirectories().ToList();
                    rootNode.Children = LoadResourceTree(dirs);
                    files = rootDir.GetFiles().ToList();
                    FileInfo file;
                    for (int j=0; j<files.Count; j++)
                    {
                        file = files[j];
                        if (file.Attributes.HasFlag(FileAttributes.Hidden))
                        {
                            files.RemoveAt(j);
                            j--;
                            continue;
                        }
                        rootNode.Children.Add(new ResourceTreeNode(file));
                    }
                    list.Add(rootNode);
                }
                catch (UnauthorizedAccessException)
                {
                    Dispatcher.Invoke(delegate { MessageWindow.Show(Application.Current.MainWindow, string.Format("目录{0}拒绝访问！请检查是否拥有管理员权限\n", rootDir.FullName)); });
                    return list;
                }
                catch (DirectoryNotFoundException)
                {
                    Roots.RemoveAt(i);
                    i--;
                }
            }
            return list;
        }

        private void Grid_Unloaded(object sender, RoutedEventArgs e)
        {
            resourceTree.Items.Clear();
        }

        private async void ResourceManger_Addpath_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();

            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                StartLoading();
                string path = fbd.SelectedPath;
                DirectoryInfo dir = new DirectoryInfo(path);
                foreach (DirectoryInfo d in roots)
                {
                    if (d.FullName == dir.FullName)
                    {
                        MessageWindow.Show(Application.Current.MainWindow, "资源管理器中已存在该目录");
                        StopLoading();
                        return;
                    }
                }
                roots.Add(dir);
                await Task.Factory.StartNew(RefreshTreeView);
                StopLoading();
            }
        }

        private void resourceTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ResourceManagerTreeNode node = e.NewValue as ResourceManagerTreeNode;
            if (node != null)
            {
                if (node.Items.Count > 0)
                {
                    SelectFileControl.DefaultPath = node.Path.FullName;
                    e.Handled = true;
                }
                else if (node.Parent is ResourceManagerTreeNode)
                {
                    SelectFileControl.DefaultPath = (node.Parent as ResourceManagerTreeNode).Path.FullName;
                    e.Handled = true;
                }
            }
        }

        /// <summary>
        /// 右键选中节点
        /// </summary>
        private void resourceTree_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var node = VisualUpwardSearch<ResourceManagerTreeNode>(e.OriginalSource as DependencyObject) as ResourceManagerTreeNode;
            if (node != null)
            {
                node.Focus();
            }
        }

        /// <summary>
        /// 向上寻找目标节点
        /// </summary>
        private static DependencyObject VisualUpwardSearch<T>(DependencyObject source)
        {
            while (source != null && source.GetType() != typeof(T))
                source = VisualTreeHelper.GetParent(source);
            return source;
        }

    }

    /// <summary>
    /// 资源管理树节点类
    /// </summary>
    public class ResourceTreeNode
    {
        public bool IsDir { get; set; }

        private FileInfo _FileInfo;
        private DirectoryInfo _DirectoryInfo;
        public FileSystemInfo Info
        {
            get
            {
                if (IsDir)
                    return _DirectoryInfo;
                else
                    return _FileInfo;
            }
        }

        public List<ResourceTreeNode> Children { get; set; }

        public ResourceTreeNode(bool isDir, string path)
        {
            if (isDir)
            {
                _DirectoryInfo = new DirectoryInfo(path);
            }
            else
            {
                _FileInfo = new FileInfo(path);
            }
            Children = new List<ResourceTreeNode>();
        }

        public ResourceTreeNode(DirectoryInfo dirInfo)
        {
            IsDir = true;
            _DirectoryInfo = dirInfo;
            Children = new List<ResourceTreeNode>();
        }

        public ResourceTreeNode(FileInfo fileInfo)
        {
            IsDir = false;
            _FileInfo = fileInfo;
            Children = new List<ResourceTreeNode>();
        }

    }
}
