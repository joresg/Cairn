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
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            //Items = loadDirs(GetSubdir(source_dir));
            collectionObject = new DirList();
            ListDir.ItemsSource = collectionObject;
            DirFull.Text = collectionObject.source_dir;
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

        private void SelectDir(object sender, EventArgs e)
        {
            dynamic Name = ListDir.SelectedItem as dynamic;
            string selected_dir = Name.DirName;
            collectionObject.loadDirs(selected_dir);
            DirFull.Text = collectionObject.source_dir;
        }

        private void dirGoBack(object sender, RoutedEventArgs e)
        {
            collectionObject.dirGoBack();
            DirFull.Text = collectionObject.source_dir;
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
            foreach (string dir_name in GetSubdir(source_dir))
            {
                string[] file_name_only = dir_name.Split('\\'); 
                Add(new Dir(file_name_only[file_name_only.Length-1]));
            }
            uiContext = SynchronizationContext.Current;
            threadWatcher = new Thread(new ThreadStart(fwRun));
            //uiContext.Send(x => _matchObsCollection.Add(match), null);
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
            foreach (string dir_name in GetSubdir(source_dir))
            {
                string[] file_name_only = dir_name.Split('\\'); 
                //Add(new Dir(file_name_only[file_name_only.Length-1]));
                //uiContext.Send(x => Add(new Dir(file_name_only[file_name_only.Length-1])), null);
                App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
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
                var get_subdir = GetSubdir(new_dir);
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

        public string[] GetSubdir(string source_dir)
        {
            string[] filesanddirs = null ;
            try
            {
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

        public Dir(string dir_name)
        {
            DirName = dir_name;
        }

    }
}