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
        private static bool IsNeedConfirm = false;
        private static string State = null;
        public static bool Start(string action)
        {
            if (IsNeedConfirm)
            {
                if (MessageWindow.Show(Application.Current.MainWindow, State+",确认"+action+"？", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
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

        public static void Reset()
        {
            IsNeedConfirm = false;
            State = null;
        }

        public static void Set(string state)
        {
            IsNeedConfirm = true;
            State = state;
        }
    }
}
