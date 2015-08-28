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

        /// <summary>
        /// 检查GRD文件格式
        /// </summary>
        /// <param name="filePath">文件名</param>
        /// <returns>格式正确返回{ Nx_input, Ny_input, Nx_output, Ny_output }, 错误返回null</returns>
        public static int[] CheckGRDFileFormat(string filePath)
        {
            try
            {
                using (var reader = new StreamReader(filePath))
                {
                    string firstLine = reader.ReadLine();
                    if (firstLine.Trim() != "DSAA")
                        return null;
                    string[] strData;
                    int Nx_input = 0, Ny_input = 0;
                    strData = reader.ReadLine().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    Nx_input = int.Parse(strData[0]);
                    Ny_input = int.Parse(strData[1]);
                    int Nx_output = (int)Math.Pow(2.0, ((int)(Math.Log(Nx_input) / Math.Log(2.0)) + 1));
                    int Ny_output = (int)Math.Pow(2.0, ((int)(Math.Log(Ny_input) / Math.Log(2.0)) + 1));
                    return new int[] { Nx_input, Ny_input, Nx_output, Ny_output };
                }
            }
            catch
            {
                return null;
            }
        }
    }

}
