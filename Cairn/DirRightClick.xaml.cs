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
using System.IO;

namespace Cairn {
    /// <summary>
    /// Interaction logic for DirRightClick.xaml
    /// </summary>
    public partial class DirRightClick : Window {
        public List<string> selected_Dirs {get;set; }
        public DirRightClick(List<string> selected_dirs) {
            InitializeComponent();
            selected_Dirs = selected_dirs;
        }
        //can't have two right click windows at the same time
        protected override void OnDeactivated(EventArgs e) {
            Console.WriteLine("DirRightClick: lost focus");
            base.OnDeactivated(e);
            Hide();
            Close();
        }
        private void FilesCopy(object o, RoutedEventArgs e) {
            Console.WriteLine("copying files");
            foreach(string filename in selected_Dirs) {
                Console.WriteLine($"selected item: {filename}");
                //TODO copy files
                //make a structure to hold all copied files
            }
            //Close();
        }
        private void FilesDelete(object o, RoutedEventArgs e) {
            Console.WriteLine("deleting files");
            foreach(string filename in selected_Dirs) {
                Console.WriteLine($"selected item: {filename}");
                //TODO delete files
                FileAttributes attr = File.GetAttributes(filename);

                //detect whether its a directory or file
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory) {
                    Console.WriteLine("je dir");
                    Directory.Delete(filename, true);

                } else {
                    Console.WriteLine("je file");
                    File.Delete(filename);
                }
            }
            Hide(); 
            Close();

        }
        private void FilesPaste(object o, RoutedEventArgs e) {
            //get all files from the structure that holds copied files
            //paste them using File.Copy(src, dst)
        }
    }
}
