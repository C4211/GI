﻿using System;
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

        public enum States
        {
            计算结果未保存 = 1,
            计算正在进行中 = 2,
            正在保存 = 3
        }

        public enum Actions
        {
            退出 = 1,
            返回 = 2,
            取消 = 3
        }

        public static bool Start(CloseAndBackConfirm.Actions action)
        {
            if (IsNeedConfirm)
            {
                if (MessageWindow.Show(Application.Current.MainWindow, State + ",确认" + Enum.ToObject(typeof(Actions),action) + "？", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
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

        public static void Set(CloseAndBackConfirm.States state)
        {
            IsNeedConfirm = true;
            State = Enum.ToObject(typeof(States), state).ToString();
        }
    }
}
