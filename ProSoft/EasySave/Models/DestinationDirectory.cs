using EasySave.Utils;

namespace EasySave.Models {
    public class DestinationDirectory : IDirectory {
        public string Path { get; }

        public DestinationDirectory(string path) {
            if (!DirectoryUtils.IsValidPath(path)) { DirectoryUtils.CreatePath(path); }
            this.Path = path;
        }
    }
}