using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GI.Tools
{
    public class FileNameFilter
    {
        public static bool CheckSuffix(string filePath)
        {
            filePath = filePath.TrimEnd();
            if (filePath.EndsWith(".txt", StringComparison.OrdinalIgnoreCase)
                || filePath.EndsWith(".grd", StringComparison.OrdinalIgnoreCase)
                || filePath.EndsWith(".dat", StringComparison.OrdinalIgnoreCase))
                return true;
            return false;
        }
    }

    public class ResourceDirectoryScanner
    {
        public static Task<List<ResourceTreeNode>> LoadResourceTreeAsync(List<DirectoryInfo> Roots)
        {
            return Task.Run(() =>
            {
                List<ResourceTreeNode> list = LoadResourceTree(Roots);
                return list;
            });
        }

        private static List<ResourceTreeNode> LoadResourceTree(List<DirectoryInfo> Roots)
        {
            List<ResourceTreeNode> list = new List<ResourceTreeNode>();
            ResourceTreeNode rootNode;
            List<DirectoryInfo> dirs;
            List<FileInfo> files;
            foreach (DirectoryInfo rootDir in Roots)
            {
                try
                {
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
                catch { }
            }
            return list;
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
