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

namespace Cairn {
    /// <summary>
    /// Interaction logic for GridRightClick.xaml
    /// </summary>
    public partial class GridRightClick : Window {
        public GridRightClick() {
            InitializeComponent();
        }

        protected override void OnDeactivated(EventArgs e) {
            Console.WriteLine("GridRightClick: lost focus");
            base.OnDeactivated(e);
            Hide();
            Close();
        }

        private void CreateDirectory(object o, EventArgs e) {
            Console.WriteLine("create directory");
            Close();
        }
    }
}
