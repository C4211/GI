using System;
using System.Collections.Generic;
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

namespace GI.Tools
{
    public class SelectUnitBox : ComboBox
    {
        public SelectUnitBox()
            : base()
        {
        }

        #region 获取设置依赖属性
        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public double ComboBoxWidth
        {
            get { return (double)GetValue(ComboBoxWidthProperty); }
            set { SetValue(ComboBoxWidthProperty, value); }
        }
        #endregion

        #region 注册依赖属性
        //public static readonly DependencyProperty ValueProperty =
        //    DependencyProperty.Register("Value", typeof(string), typeof(SelectUnitBox),
        //    new PropertyMetadata("0"));
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(string), typeof(SelectUnitBox),
            new PropertyMetadata("0", new PropertyChangedCallback((d, e) => ((SelectUnitBox)d).PropertyChanged())));
        public static readonly DependencyProperty ComboBoxWidthProperty =
            DependencyProperty.Register("ComboBoxWidth", typeof(double), typeof(SelectUnitBox));
        #endregion

        private string PreviousValue = "0";
        private void PropertyChanged()
        {
            double tmp;
            if (Value == string.Empty)
            {
                PreviousValue = "0";
                Value = "0";
            }
            else if (Value == "-")
            {
                PreviousValue = "0";
                Value = "0";
            }
            else if (Value.Contains(' ') || (!double.TryParse(Value, out tmp)))
            {
                Value = PreviousValue;
            }
            else
            {
                PreviousValue = Value;
            }
        }

        
    }
}
