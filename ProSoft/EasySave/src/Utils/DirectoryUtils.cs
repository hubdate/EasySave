using EasySave.src.Models.Data;

using System;
using System.IO;

namespace EasySave.src.Utils {

    #region Documentation
    /// <summary>
    /// A static class, that contains methods to manipulate and manage directories.
    /// </summary>
    #endregion
    public static class DirectoryUtils {
        
        #region Documentation
        /// <summary>
        /// The name of the file currently being copied.
        /// _currentFile[0] is the name of the file.
        /// _currentFile[1] is the name of the save.
        /// </summary>
        #endregion
        private static readonly string[] _currentFile = new string[2];


        #region Documentation
        /// <summary>
        /// Copy all files and folders from a source directory to a destination directory
        /// </summary>
        /// <param name="">The save concerned</param>
        /// <returns></returns>
        #endregion
        public static void CopyFilesAndFolders(Save s) {
            DirectoryInfo sourceDirectory = new DirectoryInfo(s.sourceDirectory.Path);
            DirectoryInfo destinationDirectory = new DirectoryInfo(s.destinationDirectory.Path);

            Parallel.Invoke(
                () => ConsoleUtils.CreateProgressBar(s),
                () => CopyAll(s, sourceDirectory, destinationDirectory, s.GetSaveType())
            );
        }



        private static void CopyAll(Save s, DirectoryInfo src, DirectoryInfo dst, SaveType type) {
            foreach (FileInfo file in src.GetFiles()) {

                // Update the json data
                LogsUtils.LogSaves();
                bool fileCopied = true;
                bool fileExists = File.Exists(Path.Combine(dst.FullName, file.FullName));

                // Proceed differential mode by comparing files data
                if (
                    type == SaveType.FULL   ||
                    !fileExists             ||
                    ( DateTime.Compare(
                        File.GetLastWriteTime(Path.Combine(dst.FullName, file.FullName)), 
                        File.GetLastWriteTime(Path.Combine(src.FullName, file.Name))
                    ) < 0)
                ) {
                    _currentFile[0] = src.FullName;
                    _currentFile[1] = dst.FullName;

                    // Create a Stopwatch to measure the transfer time
                    var watch = new System.Diagnostics.Stopwatch();
                    watch.Start();
                    try   { file.CopyTo(Path.Combine(dst.FullName, file.Name), true); }
                    catch {
                        fileCopied = false;
                        Console.WriteLine($"{Path.Combine(dst.FullName, file.Name)} | Access denied");
                    }
                    watch.Stop();

                    LogsUtils.LogTransfert(
                        s,
                        Path.Combine(src.FullName, file.Name),
                        Path.Combine(dst.FullName, file.Name),
                        file.Length,
                        watch.ElapsedMilliseconds
                    );
                }

                if (fileCopied) { s.AddFileCopied(); }
                s.AddSizeCopied(file.Length);
            }


            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo subDirectory in src.GetDirectories()) {
                DirectoryInfo nextTarget = dst.CreateSubdirectory(subDirectory.Name);
                CopyAll(s, subDirectory, nextTarget, type);
            }
        }



        #region Documentation
        /// <summary>
        /// Checks if the given path is a valid directory path.
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <returns>True if the path is a valid directory path, false otherwise.</returns>
        #endregion
        public static bool IsValidPath(string path) {
            return Directory.Exists(path);
        }


        #region Documentation
        /// <summary>
        /// Creates a directory at the given path.
        /// </summary>
        /// <param name="path">The path of the directory to create.</param>
        #endregion
        public static void CreatePath(string path) {
            Directory.CreateDirectory(path);
        }


        #region Documentation
        /// <summary>
        /// Calculates the total size of a directory recursively, including all its files and subdirectories.
        /// </summary>
        /// <param name="path">The directory for which to calculate the size.</param>
        /// <returns>The total size of the directory in bytes.</returns>
        #endregion
        public static double GetDirectorySize(DirectoryInfo path) {
            double size = 0;
            foreach (FileInfo file in path.GetFiles()) { size += file.Length; }
            foreach (DirectoryInfo subDirectory in path.GetDirectories()) { size += GetDirectorySize(subDirectory);}
            
            return size;
        }


        #region Documentation
        /// <summary>
        /// Calculates the total number of files in a directory and its subdirectories.
        /// </summary>
        /// <param name="path">The directory to count the files in.</param>
        /// <returns>The total number of files in the directory and its subdirectories.</returns>
        #endregion
        public static long GetDirectoryNbFiles(DirectoryInfo path) {
            long nbFiles = 0;
            foreach (FileInfo file in path.GetFiles()) { nbFiles++; }
            foreach (DirectoryInfo subDirectory in path.GetDirectories()) { nbFiles += GetDirectoryNbFiles(subDirectory);}
            
            return nbFiles;
        }


        #region Documentation
        /// <summary>
        /// Returns the name of the file currently being copied.
        /// </summary>
        /// <returns>The name of the file currently being copied.</returns>
        #endregion
        public static string[] GetCurrentFile() {
            return _currentFile;
        }
    }

}