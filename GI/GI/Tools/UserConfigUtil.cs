using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace GI.Tools
{
    class UserConfigUtil
    {
        private const string CONFIG_PATH = @"user.config";

        private static XmlDocument getXmlDoc()
        {
            XmlDocument xmlDoc;
            try
            {
                xmlDoc = new XmlDocument();
                xmlDoc.Load(CONFIG_PATH);
            }
            catch
            {
                string dest = DateTime.Now.ToFileTime().ToString();
                if (File.Exists(CONFIG_PATH))
                {
                    File.Move(CONFIG_PATH, dest);
                    MessageBox.Show("配置已损坏！将使用默认配置\n原配置文件已被重命名为" + dest, "警告");
                }
                xmlDoc = new XmlDocument();
                xmlDoc.CreateXmlDeclaration("1.0", "utf-8", "yes");
                XmlNode root = xmlDoc.CreateElement("config");
                XmlNode defaultPath = xmlDoc.CreateElement("path");
                XmlCDataSection cdata = xmlDoc.CreateCDataSection(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
                defaultPath.AppendChild(cdata);
                root.AppendChild(defaultPath);
                xmlDoc.AppendChild(root);
            }
            return xmlDoc;
        }

        public static List<DirectoryInfo> GetPaths()
        {
            XmlDocument xmlDoc = getXmlDoc();
            XmlNodeList list = xmlDoc.SelectNodes("/config/path");
            List<DirectoryInfo> result = new List<DirectoryInfo>();
            if (list != null)
            {
                foreach (XmlNode pathNode in list)
                {
                    DirectoryInfo dir = new DirectoryInfo((pathNode.FirstChild as XmlCDataSection).InnerText.Trim());
                    result.Add(dir);
                }
            }
            return result;
        }

        public static void SavePaths(List<DirectoryInfo> paths)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.CreateXmlDeclaration("1.0", "utf-8", "yes");
            XmlNode root = xmlDoc.CreateElement("config");
            XmlNode node;
            XmlCDataSection cdata;
            foreach (var path in paths)
            {
                node = xmlDoc.CreateElement("path");
                cdata = xmlDoc.CreateCDataSection(path.FullName);
                node.AppendChild(cdata);
                root.AppendChild(node);
            }
            xmlDoc.AppendChild(root);
            try
            {
                xmlDoc.Save(CONFIG_PATH);
            }
            catch
            {
                // MessageBox.Show("保存配置文件出错！");
            }
        }
    }
}
