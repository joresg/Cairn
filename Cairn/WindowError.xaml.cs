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
using System.Windows.Shapes;

namespace Cairn
{
    /// <summary>
    /// Interaction logic for WindowError.xaml
    /// </summary>
    public partial class WindowError : Window
    {
        public WindowError(string error_msg)
        {
            InitializeComponent();
            //ErrorMsg = error_msg;
            //okej.DataContext = ErrorMsg;
            DataContext = Error.GetErrorMsg(error_msg);

        }
    }
    public class Error
    {
        public string ErrorMsg { get; set; }

        public static Error GetErrorMsg(string error_msg)
        {
            var msg = new Error()
            {
                ErrorMsg = error_msg
            };
            return msg;
        }
    }
}
