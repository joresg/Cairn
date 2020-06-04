using System;
using System.IO;
using System.Security.Permissions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cairn {
    class FileWatcher {
        public static DirList sourceWindow { get; set; }
        public Reloader reloader { get; set; }
        public FileWatcher() {
            reloader = new Reloader();
        }
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public void Run(DirList mw, string watch_path) {
            //sourceWindow = mw;
            Console.WriteLine($"watcher started for {watch_path}");
            // Create a new FileSystemWatcher and set its properties.
            using (FileSystemWatcher watcher = new FileSystemWatcher()) {
                watcher.Path = watch_path;

                // Watch for changes in LastAccess and LastWrite times, and
                // the renaming of files or directories.
                watcher.NotifyFilter = NotifyFilters.LastAccess
                                     | NotifyFilters.LastWrite
                                     | NotifyFilters.FileName
                                     | NotifyFilters.DirectoryName;

                // Add event handlers.
                watcher.Changed += OnChanged;
                watcher.Created += OnChanged;
                watcher.Deleted += OnChanged;
                watcher.Renamed += OnRenamed;

                // Begin watching.
                watcher.EnableRaisingEvents = true;
                while (true) {
                    System.Threading.Thread.Sleep(2000);
                }
            }
        }

        // Define the event handlers.
        //private void OnChanged(object source, FileSystemEventArgs e) =>
            // Specify what is done when a file is changed, created, or deleted.
            //Console.WriteLine($"File: {e.FullPath} {e.ChangeType}");
            //sourceWindow.reloadDirs();
        private void OnChanged(object source, FileSystemEventArgs e) {
            reloader.reload(source, e);
        }

        private void OnRenamed(object source, RenamedEventArgs e) =>
            // Specify what is done when a file is renamed.
            Console.WriteLine($"File: {e.OldFullPath} renamed to {e.FullPath}");
    }
    public class Reloader {
        public event EventHandler directoryChange;
        public void reload(object sender, EventArgs e) {
                directoryChange?.Invoke(this, null);
        }
    }
}
