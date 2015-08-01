using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GI.Tools
{
    class FileNameFilter
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
}
