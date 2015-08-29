using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GI.Tools
{
    class CloseAndBackConfirm
    {
        public static bool IsNeedConfirm = false;
        public static bool Start(string message)
        {
            if (IsNeedConfirm)
            {
                if (MessageWindow.Show(Application.Current.MainWindow, message, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    IsNeedConfirm = false;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
    }
}
