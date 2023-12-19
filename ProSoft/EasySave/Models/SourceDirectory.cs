using EasySave.Utils;
using System.IO;

namespace EasySave.Models {
    public class SourceDirectory : IDirectory {
        private readonly double __size;

        public readonly long nbFiles;
        public string Path { get; }

        public SourceDirectory(string path) {
            this.Path = path;
            
            DirectoryInfo directory = new DirectoryInfo(path);
            this.__size = DirectoryUtils.GetDirectorySize(directory);
            this.nbFiles = DirectoryUtils.GetDirectoryNbFiles(directory);
        }

        public double GetSize() { return this.__size; }
    }
}