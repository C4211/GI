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
        public static bool CheckFileSuffix(string filePath)
        {
            filePath = filePath.TrimEnd();
            if (filePath.EndsWith(".txt", StringComparison.OrdinalIgnoreCase)
                || filePath.EndsWith(".grd", StringComparison.OrdinalIgnoreCase)
                || filePath.EndsWith(".dat", StringComparison.OrdinalIgnoreCase))
                return true;
            return false;
        }

        public static bool CheckFileExistence(string filePath)
        {
            FileInfo file = new FileInfo(filePath);
            if (file.Exists)
            {
                file = null;
                return true;
            }
            else
            {
                file = null;
                return false;
            }
        }
    }

}
