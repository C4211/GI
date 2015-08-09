using GI.Tools;
using System;
using System.Collections.Generic;
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
            if (roots.Count == 0)
                roots.Add(new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.Desktop)));
        }

        private static List<DirectoryInfo> roots = new List<DirectoryInfo>();

        #region 控制LoadingBar
        private void StartLoading()
        {
            Dispatcher.Invoke(delegate
            {
                resourceTree.Visibility = Visibility.Hidden;
                loadingBar.Visibility = Visibility.Visible;
            });
        }

        private void StopLoading()
        {
            Dispatcher.Invoke(delegate
            {
                loadingBar.Visibility = Visibility.Hidden;
                resourceTree.Visibility = Visibility.Visible;
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
            try
            {
                Thread.Sleep(600);
                result = LoadResourceTree(roots);
                Dispatcher.Invoke(delegate
                {
                    resourceTree.Items.Refresh();
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
                        resourceTree.Items.Add(parentNode);
                    }
                });
            }
            catch
            {
                Dispatcher.Invoke(delegate { MessageBox.Show(Application.Current.MainWindow, @"读取目录失败！"); });
            }
            finally
            {
                if (result != null)
                    result = null;
                if (list != null)
                    list = null;
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
                if (!child.IsDir)
                {
                    childNode.PreviewMouseLeftButtonDown += delegate
                    {
                        DragDrop.DoDragDrop(childNode, childNode.Path.FullName, DragDropEffects.All);
                    };
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

        private List<ResourceTreeNode> LoadResourceTree(List<DirectoryInfo> Roots)
        {
            List<ResourceTreeNode> list = new List<ResourceTreeNode>();
            ResourceTreeNode rootNode;
            List<DirectoryInfo> dirs;
            List<FileInfo> files;
            foreach (DirectoryInfo rootDir in Roots)
            {
                if (!rootDir.Exists)
                    throw new Exception();
                rootNode = new ResourceTreeNode(rootDir);
                dirs = rootDir.GetDirectories().ToList();
                rootNode.Children = LoadResourceTree(dirs);
                files = rootDir.GetFiles().ToList();
                foreach (var file in files)
                {
                    rootNode.Children.Add(new ResourceTreeNode(file));
                }
                list.Add(rootNode);
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
                string path = fbd.SelectedPath;
                DirectoryInfo dir = new DirectoryInfo(path);
                roots.Add(dir);
                StartLoading();
                await Task.Factory.StartNew(RefreshTreeView);
                StopLoading();
            }
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
