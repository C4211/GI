using GI.Tools;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// ResourceManager.xaml 的交互逻辑
    /// </summary>
    public partial class ResourceManager : Grid
    {
        public ResourceManager()
        {
            InitializeComponent();
        }

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

        private async void resourceTree_Loaded(object sender, RoutedEventArgs e)
        {
            StartLoading();
            await RefreshTree();
            StopLoading();
        }

        public async Task RefreshTree()
        {
            List<DirectoryInfo> roots = new List<DirectoryInfo>();
            roots.Add(new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)));
            Task<List<ResourceTreeNode>> task = ResourceDirectoryScanner.LoadResourceTreeAsync(roots);
            await task;
            List<ResourceTreeNode> result;
            result = task.Result;
            resourceTree.Items.Clear();
            foreach (var node in result)
            {
                ResourceManagerTreeNode parentNode;
                parentNode = new ResourceManagerTreeNode(0);
                parentNode.Path = node.Info;
                parentNode.Title = node.Info.Name;
                List<ResourceManagerTreeNode> list = FillDataToResourceTree(node);
                foreach (var l in list)
                {
                    parentNode.Items.Add(l);
                }
                resourceTree.Items.Add(parentNode);
            }
        }

        private List<ResourceManagerTreeNode> FillDataToResourceTree(ResourceTreeNode node, int level = 1)
        {
            List<ResourceManagerTreeNode> result = new List<ResourceManagerTreeNode>();
            foreach (var child in node.Children)
            {
                ResourceManagerTreeNode childNode = new ResourceManagerTreeNode(level);
                childNode.Path = child.Info;
                childNode.Title = child.Info.Name;
                if (!child.IsDir)
                {
                    childNode.PreviewMouseLeftButtonDown += delegate
                    {
                        DragDrop.DoDragDrop(childNode, childNode.Path.FullName, DragDropEffects.All);
                    };
                }
                List<ResourceManagerTreeNode> list = FillDataToResourceTree(child, level + 1);
                foreach (var l in list)
                {
                    childNode.Items.Add(l);
                }
                result.Add(childNode);
            }
            return result;
        }
    }
}
