using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Threading;
using System.Collections.Specialized;
using System.Configuration;
using System.Runtime.Remoting.Channels;

namespace Cairn
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public List<Dir> Items { get; set; }
        public string dir_name { get; set; }
        public DirList collectionObject { get; set; }
        public DirRightClick dir_right_click { get; set; }
        public GridRightClick grid_right_click { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            //Items = loadDirs(GetSubdir(source_dir));
            collectionObject = new DirList();
            ListDir.ItemsSource = collectionObject;
            DirFull.Text = collectionObject.source_dir;

            //dir_right_click = new DirRightClick();
            //dir_right_click.LostFocus += CloseRightClickWin;
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            Console.WriteLine("PROPERTY CHANGED");
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }
        #endregion

        public void Window_Loaded(object sender, EventArgs e)
        {
            EditableTextBlock textBlock = (EditableTextBlock)sender;
            AdornerLayer.GetAdornerLayer(textBlock);
            if (null != textBlock)
            {
                //Get the adorner layer of the uielement (here TextBlock)
                AdornerLayer layer = AdornerLayer.GetAdornerLayer(textBlock);

                //If the IsInEditMode set to true means the user has enabled the edit mode then
                //add the adorner to the adorner layer of the TextBlock.
                if (textBlock.State)
                {
                    if (null == textBlock._adorner)
                    {
                        textBlock._adorner = new EditableTextBlockAdorner(textBlock);

                        //Events wired to exit edit mode when the user presses 
                        //Enter key or leaves the control.
                        /*
                        textBlock._adorner.TextBoxKeyUp += textBlock.TextBoxKeyUp;
                        textBlock._adorner.TextBoxLostFocus += textBlock.TextBoxLostFocus;
                        */
                    }
                    layer.Add(textBlock._adorner);
                }
                else
                {
                    //Remove the adorner from the adorner layer.
                    Adorner[] adorners = layer.GetAdorners(textBlock);
                    if (adorners != null)
                    {
                        foreach (Adorner adorner in adorners)
                        {
                            if (adorner is EditableTextBlockAdorner)
                            {
                                layer.Remove(adorner);
                            }
                        }
                    }

                    //Update the textblock's text binding.
                    /*
                    BindingExpression expression = textBlock.GetBindingExpression(TextProperty);
                    if (null != expression)
                    {
                        expression.UpdateTarget();
                    }
                    */
                }
            }
        }

        private void SelectDir(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine($"click count: {e.ClickCount}");
            //if(e.ClickCount == 2) {
            if (!Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.LeftCtrl)) {
            Console.WriteLine("MASK KEYS ARE NOT DOWN");
            dynamic Name = ListDir.SelectedItem as dynamic;
                string selected_dir = Name.DirName;
                collectionObject.loadDirs(selected_dir);
                DirFull.Text = collectionObject.source_dir;
            }
            /*
            EditableTextBlock pos = (EditableTextBlock)sender;
            pos.State = !pos.State;
            Window_Loaded(sender, e);
            */
        }

        private void dirGoBack(object sender, RoutedEventArgs e)
        {
            collectionObject.dirGoBack();
            DirFull.Text = collectionObject.source_dir;
        }
        private void GridRightClick(object sender, RoutedEventArgs e) {
            grid_right_click = new GridRightClick(collectionObject.source_dir);
            grid_right_click.WindowStartupLocation = WindowStartupLocation.Manual;
            grid_right_click.Left = PointToScreen(Mouse.GetPosition(null)).X;
            grid_right_click.Top = PointToScreen(Mouse.GetPosition(null)).Y;
            grid_right_click.Focus();
            grid_right_click.Show();

        }
        private void SubdirRightClick(object sender, MouseButtonEventArgs e) {
            Console.WriteLine("Right click on subdir element");
            e.Handled = true;
            /*
            Console.WriteLine($"type: {sender.GetType()}");
            //ignore parent event
            if (sender.GetType() == typeof(Grid))
            {
                Console.WriteLine($"jebisimater: {sender.GetType()}");
                return;
            }
            */
            List<string> selected_dirs = new List<string>();
            int x = 0;
            //give all files full path and pass them to newley instantiated DirRightClick object
            foreach(Dir file in ListDir.SelectedItems) {
                selected_dirs.Add(System.IO.Path.Combine(collectionObject.source_dir,file.DirName));
                x++;
            }
            dir_right_click = new DirRightClick(this, selected_dirs);
            dir_right_click.WindowStartupLocation = WindowStartupLocation.Manual;
            dir_right_click.Left = PointToScreen(Mouse.GetPosition(null)).X;
            dir_right_click.Top = PointToScreen(Mouse.GetPosition(null)).Y;
            dir_right_click.Focus();
            dir_right_click.Show();
        }
        public void RenameItemsGetInput()
        {

        }
    }
    public class DirList : ObservableCollection<Dir>, INotifyPropertyChanged
    {
        public string source_dir { get; set; }
        public Thread threadWatcher { get; set; }
        public Reloader _reloader { get; set; }
        public SynchronizationContext uiContext { get; set; }
        public DirList() : base()
        {
            //source_dir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            source_dir = Environment.GetEnvironmentVariable("userprofile");
            //Console.WriteLine($"starting dir: {source_dir}");
            foreach (string dir_name in GetSubdir(source_dir, DirOutput.Mode.All, DirOutput.Mode_Visibility.All))
            {
                string[] file_name_only = dir_name.Split('\\'); 
                Add(new Dir(file_name_only[file_name_only.Length-1]));
            }
            //uiContext = SynchronizationContext.Current;
            threadWatcher = new Thread(new ThreadStart(fwRun));
            threadWatcher.Start();
        }
        public void directoryChange(object sender, EventArgs e) {
            reloadDirs();
        }

        /*
        public override event NotifyCollectionChangedEventHandler CollectionChanged;
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e) {
            var eh = CollectionChanged;
            if (eh != null) {
                System.Windows.Threading.Dispatcher dispatcher = (from System.Collections.Specialized.NotifyCollectionChangedEventHandler nh in eh.GetInvocationList()
                                         let dpo = nh.Target as System.Windows.Threading.DispatcherObject
                                                                  where dpo != null
                                         select dpo.Dispatcher).FirstOrDefault();

                if (dispatcher != null && dispatcher.CheckAccess() == false) {
                    dispatcher.Invoke(DispatcherPriority.DataBind, (Action)(() => OnCollectionChanged(e)));
                } else {
                    foreach (NotifyCollectionChangedEventHandler nh in eh.GetInvocationList())
                        nh.Invoke(this, e);
                }
            }
        }
        */

        //wrapper for FileWatcher Run method
        public void fwRun() {
            Console.WriteLine($"starting watcher thread for: {source_dir}");
            FileWatcher fw = new FileWatcher();
            _reloader = fw.reloader;
            _reloader.directoryChange += directoryChange;
            fw.Run(source_dir);
        }
        public void reloadDirs() {
            App.Current.Dispatcher.Invoke((Action)delegate {
                Clear();
            });
            //Clear();
            foreach (string dir_name in GetSubdir(source_dir, DirOutput.Mode.All, DirOutput.Mode_Visibility.All))
            {
                string[] file_name_only = dir_name.Split('\\'); 
                //Add(new Dir(file_name_only[file_name_only.Length-1]));
                //uiContext.Send(x => Add(new Dir(file_name_only[file_name_only.Length-1])), null);
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    Add(new Dir(file_name_only[file_name_only.Length - 1]));
                });
            }
        }
        public void loadDirs(string selected_dir)
        {
            if(Directory.Exists(System.IO.Path.Combine(source_dir, selected_dir)))
            {
                //kill thread watcher thread for old dir
                threadWatcher.Abort();
                //Console.WriteLine($"selected dir: {selected_dir}");
                //Console.WriteLine($"old dir: {source_dir}");
                string[] sd_split = selected_dir.Split('\\');
                string new_dir = System.IO.Path.Combine(source_dir, sd_split[sd_split.Length - 1]);
                //Console.WriteLine($"new dir: {source_dir}");
                var get_subdir = GetSubdir(new_dir, DirOutput.Mode.All, DirOutput.Mode_Visibility.All);
                if(get_subdir != null)
                {
                    Clear();
                    foreach (string dir_name in get_subdir)
                    {
                        string[] file_name_only = dir_name.Split('\\'); 
                        Add(new Dir(file_name_only[file_name_only.Length-1]));
                    }
                    source_dir = new_dir;
                    //start threadwatcher for new dir
                    threadWatcher = new Thread(new ThreadStart(fwRun));
                    threadWatcher.Start();
                }
            }
            else
            {
                Console.WriteLine("NOT A PATH, OPEN FILE");
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                process.StartInfo.FileName = System.IO.Path.Combine(source_dir, selected_dir);
                //process.StartInfo.Arguments = 
                process.Start();
            }
        }

        public void dirGoBack()
        {
            Console.WriteLine($"preback dir: {source_dir}");
            if(source_dir.Split('\\').Length == 2)
            {
                source_dir = source_dir.Substring(0, source_dir.LastIndexOf('\\')+1);
            }
            else
            {
                source_dir = source_dir.Substring(0, source_dir.LastIndexOf('\\'));
            }
            Console.WriteLine($"back dir: {source_dir}");
            loadDirs("");
        }
        public static class DirOutput {
            public enum Mode {
                FilesOnly,
                DirsOnly,
                All

            }
            public enum Mode_Visibility {
                NonHidden,
                All
            }
        }

        public string[] GetSubdir(string source_dir, DirOutput.Mode mode, DirOutput.Mode_Visibility mode_visibility)
        {
            string[] filesanddirs = null ;
            try
            {
                /*
                string[] files = System.IO.Directory.GetFiles(source_dir);
                string[] directories = System.IO.Directory.GetDirectories(source_dir);
                filesanddirs = new string[files.Length + directories.Length];
                int x = 0;
                foreach (string file in files)
                {
                    filesanddirs[x] = file;
                    x++;
                }
                foreach (string dir in directories)
                {
                    filesanddirs[x] = dir;
                    x++;
                }
                */
                DirectoryInfo directory = new DirectoryInfo(source_dir);
                FileInfo[] files = directory.GetFiles();
                DirectoryInfo[] dirs = directory.GetDirectories();
                var files_filtered = files.Where(f => !f.Attributes.HasFlag(FileAttributes.Hidden));
                var dirs_filtered = dirs.Where(f => !f.Attributes.HasFlag(FileAttributes.ReadOnly) || !f.Attributes.HasFlag(FileAttributes.Hidden));
                Console.WriteLine(files_filtered.Count() + "," + dirs_filtered.Count());
                filesanddirs = new string[files_filtered.Count()+dirs_filtered.Count()];
                //filesanddirs = new string[files_filtered.Count()+dirs.Length];
                int x = 0;
                Console.WriteLine("izpisujem non hidden");
                foreach(var file in files_filtered) {
                    filesanddirs[x] = file.ToString();
                    Console.WriteLine(filesanddirs[x]);
                    x++;
                }
                foreach(var dir in dirs_filtered) {
                    filesanddirs[x] = dir.ToString();
                    Console.WriteLine(filesanddirs[x]);
                    x++;
                }
            }
            catch (UnauthorizedAccessException)
            {
                FileAttributes attr = (new FileInfo(source_dir)).Attributes;
                Console.Write("UnAuthorizedAccessException: Unable to access file. ");
                WindowError win = new WindowError("Access Denied.");
                win.Show();
                if ((attr & FileAttributes.ReadOnly) > 0)
                    Console.Write("The file is read-only.");
            }
            /*
            finally
            {
                //TODO
            }
            */
            //return files;
            return filesanddirs;
        }
    }

    public class Dir
    {
        public string DirName { get; set; }
        public bool Edit { get; set; }

        public Dir(string dir_name)
        {
            DirName = dir_name;
            Edit = true;
        }

    }
}