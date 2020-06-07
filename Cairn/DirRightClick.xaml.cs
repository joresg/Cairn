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
        public MainWindow mw { get; set; }
        public List<string> selected_Dirs {get;set; }
        public DirRightClick(MainWindow main_window, List<string> selected_dirs) {
            InitializeComponent();
            mw = main_window; 
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

        private childItem FindVisualChild<childItem>(DependencyObject obj)
        where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem)
                {
                    return (childItem)child;
                }
                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }


        private void FilesRename(object o, EventArgs e)
        {
            //go trough all selected files, change the type from TextBlock to something which takes input
            Console.WriteLine("you need to rename theese items:");
            //find the file which is flagged to be renamed

            foreach (Cairn.Dir idk in mw.ListDir.SelectedItems)
            {
                Console.WriteLine(idk.DirName);
                //EditableTextBlock item = (EditableTextBlock)mw.ListDir.ItemContainerGenerator.ContainerFromItem(idk);
                ListBoxItem item = (ListBoxItem)mw.ListDir.ItemContainerGenerator.ContainerFromItem(idk);
                var neki = item.DataContext;
                Console.WriteLine($"item:::: {item}");
                Console.WriteLine($"item:::: {neki}");

                // Getting the ContentPresenter of myListBoxItem
                ContentPresenter myContentPresenter = FindVisualChild<ContentPresenter>(item);

                // Finding textBlock from the DataTemplate that is set on that ContentPresenter
                DataTemplate myDataTemplate = myContentPresenter.ContentTemplate;
                EditableTextBlock myTextBlock = (EditableTextBlock)myDataTemplate.FindName("SubdirName", myContentPresenter);
                myTextBlock.State = !myTextBlock.State;
                mw.Window_Loaded(myTextBlock, e);
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
