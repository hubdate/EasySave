using System;
using System.IO;
using System.Linq;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Threading;

using ProSoft.Cryptosoft;
using EasySave.Models.Data;
using System.Diagnostics;


namespace EasySave.Utils {

    public static class DirectoryUtils {
        private static readonly string[] __currentFile = new string[2];
        private static readonly JObject __config = JObject.Parse(File.ReadAllText($"{LogUtils.path}config.json"));
        private static string __key = __config["key"].ToString();
        private static HashSet<string> __cyrptosoftExtensions = __config["cryptosoftExtensions"].Select(t => t.ToString()).ToHashSet();
        private static HashSet<string> __process = __config["process"].Select(t => t.ToString()).ToHashSet();
        private static HashSet<string> __priorityExtensions = __config["priorityExtensions"].Select(t => t.ToString()).ToHashSet();
        private static int __limitSize = int.Parse(__config["limitSize"].ToString());
        private static readonly Mutex __logMutex = new Mutex();
        private static readonly Mutex __filesMutex = new Mutex();
        private static readonly SemaphoreSlim prioritarySaveSemaphore = new SemaphoreSlim(1, 1);
        private static Cryptosoft __cs;
        private static readonly ManualResetEvent __mre = new ManualResetEvent(true);




        public static void CopyFilesAndFolders(Save s) {
            try   { __cs = Cryptosoft.Init(__key, __cyrptosoftExtensions.ToArray()); }
            catch { __cs = Cryptosoft.Init(__key); }

            DirectoryInfo sourceDirectory = new DirectoryInfo(s.sourceDirectory.Path);
            DirectoryInfo destinationDirectory = new DirectoryInfo(s.destinationDirectory.Path);

            List<KeyValuePair<FileInfo, FileInfo>> files = GetAllFiles(sourceDirectory, destinationDirectory, s);

            switch(CopyAll(s, files, __mre)) {
                case JobStatus.CANCELED:
                    // Notification: Save canceled
                    s.Cancel();
                    break;

                case JobStatus.FINISHED:
                    // Notification: Save finished
                    s.MarkAsFinished();
                    break;
            }

            LogUtils.LogSaves();
        }


        #region Documentation
        /// <summary>
        /// Method to get all files in a directory and its subdirectories, 
        /// by search-cascading through the source.
        /// </summary>
        /// <param name="sourceDirectory">The directory to search in.</param>
        /// <param name="destinationDirectory">The directory to copy the files to.</param>
        /// <param name="s">The save to copy the files from.</param>
        /// <returns>A list of key value pair with source files and destination files</returns>
        #endregion
        private static List<KeyValuePair<FileInfo, FileInfo>> GetAllFiles(DirectoryInfo sourceDirectory, DirectoryInfo destinationDirectory, Save s) {
            List<KeyValuePair<FileInfo, FileInfo>> files = new List<KeyValuePair<FileInfo, FileInfo>>();
            foreach (FileInfo file in sourceDirectory.GetFiles()) {
                if (__priorityExtensions.Contains(file.Extension)) {
                    files.Insert(
                        0, 
                        new KeyValuePair<FileInfo, FileInfo>(file, new FileInfo(Path.Combine(destinationDirectory.FullName, file.Name)))
                    );
                }
                else { files.Add(new KeyValuePair<FileInfo, FileInfo>(file, new FileInfo(Path.Combine(destinationDirectory.FullName, file.Name)))); }
            }

            foreach (DirectoryInfo subDirectory in sourceDirectory.GetDirectories()) {
                files.AddRange(GetAllFiles(subDirectory, destinationDirectory.CreateSubdirectory(subDirectory.Name), s));
            }

            return files;
        }


        private static JobStatus CopyAll(Save s, List<KeyValuePair<FileInfo, FileInfo>> files, ManualResetEvent mre) {
            foreach (KeyValuePair<FileInfo, FileInfo> fileInfo in files) {
                try {
                    LogUtils.LogSaves();
                    FileInfo sourceFile = fileInfo.Key;
                    FileInfo destinationFile = fileInfo.Value;

                    if (s.GetStatus() == JobStatus.CANCELED) { return JobStatus.CANCELED; }
                    foreach (var p in __process) {
                        Process[] processes = Process.GetProcessesByName(p.Split(".exe")[0].ToUpper());
                        if (processes.Length > 0 && s.GetStatus() == JobStatus.RUNNING) {
                            // [TO DO] send notification process is running
                            s.Pause();
                            LogUtils.LogSaves();
                            Process head = processes[0];
                            if (head != null) {
                                head.EnableRaisingEvents = true;
                                head.Exited += (sender, e) => {
                                    // [TO DO] send notification process is not running anymore
                                    s.Resume();
                                    // [TO DO] ResumeTransfer();
                                };
                            }
                        }
                    }
                    __mre.WaitOne();

                    bool fileCopied = true;
                    bool fileExists = File.Exists(Path.Combine(destinationFile.FullName));

                    //Process priority files first
                    if (__priorityExtensions.Contains(fileInfo.Key.Name)) { prioritarySaveSemaphore.Wait(); }
                    
                    // Proceed differential mode by comparing files data
                    if (
                        s.GetSaveType() == SaveType.FULL    ||
                        !fileExists ||
                        ( DateTime.Compare(
                            File.GetLastWriteTime(destinationFile.FullName), 
                            File.GetLastWriteTime(sourceFile.FullName)
                        ) < 0) ||
                        ( DateTime.Compare(
                            File.GetLastWriteTime($"{destinationFile.FullName}.cryptosoft"), 
                            File.GetLastWriteTime(sourceFile.FullName)
                        ) < 0)
                    ) {
                        // If size limit is reached, block other large files
                        if (__limitSize > 0 && (sourceFile.Length / 1024) > __limitSize) { __filesMutex.WaitOne(); }

                        __currentFile[0] = sourceFile.FullName;
                        __currentFile[1] = destinationFile.FullName;
                        long encryptionTime = -2;

                        // Create a Stopwatch to measure the transfer time
                        var watch = new Stopwatch();
                        watch.Start();
                        try {
                            if (__cyrptosoftExtensions.Contains(sourceFile.Extension)) {
                                encryptionTime = __cs.ProcessFile(sourceFile.FullName, $"{destinationFile.FullName}.cryptosoft");
                            }
                            else { sourceFile.CopyTo(destinationFile.FullName, true); }
                        }

                        catch {
                            fileCopied = false;
                            // [TO DO] Notify the user that the file could not be copied
                        }
                        watch.Stop();

                        __logMutex.WaitOne();
                        LogUtils.LogTransfer(
                            s,
                            sourceFile.FullName,
                            destinationFile.FullName,
                            sourceFile.Length,
                            watch.ElapsedMilliseconds,
                            encryptionTime
                        );
                        __logMutex.ReleaseMutex();

                        // Release the Semaphore, if it was acquired
                        if (__limitSize > 0 && (sourceFile.Length / 1024) > __limitSize) { __filesMutex.ReleaseMutex(); }
                    }

                    if (fileCopied) { s.AddFileCopied(); }
                    s.AddSizeCopied(sourceFile.Length);
                }

                finally {
                    if (__priorityExtensions.Contains(fileInfo.Key.Name)) { prioritarySaveSemaphore.Release(); }
                }
            }

            return JobStatus.FINISHED;
        }


        public static bool IsValidPath(string path) {
            return Directory.Exists(path);
        }


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
            return __currentFile;
        }


        public static void PauseTransfer() {
            __mre.Reset();
        }


        public static void ResumeTransfer() {
            __mre.Set();
        }


        public static void ChangeKey(string key) {
            __key = key;
            UpdateConfig();
        }


        public static void ChangeExtensionsToEncrypt(HashSet<string> extensions) {
            __cyrptosoftExtensions = extensions;
            UpdateConfig();
        }


        public static void ChangeProcess(HashSet<string> process) {
            __process = process;
            UpdateConfig();
        }


        public static void ChangePriorityExtensions(HashSet<string> extensions) {
            __priorityExtensions = extensions;
            UpdateConfig();
        }


        public static void ChangeLimitSize(int limitSize) {
            __limitSize = limitSize;
            UpdateConfig();
        }


        private static void UpdateConfig() {
            LogUtils.LogConfig(__key, __cyrptosoftExtensions, __process, __priorityExtensions, __limitSize);
        }


        public static string GetKey() {
            try   { return __key; }
            catch { return $"Please set a key in {LogUtils.path}config.json"; }
        }


        public static string GetExtensionsToEncrypt() {
           return string.Join("\r\n", __cyrptosoftExtensions);
        }


        public static string GetProcess() {
            return string.Join("\r\n", __process);
        }


        public static string GetPriorityExtensions() {
            return string.Join("\r\n", __priorityExtensions);
        }


        public static int GetLimitSize() {
            return __limitSize;
        }
    }   
}