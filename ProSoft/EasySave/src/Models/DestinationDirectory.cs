using EasySave.src.Utils;


namespace EasySave.src.Models {

    #region Documentation
    /// <summary>
    /// A model for a source directory.
    /// </summary>
    #endregion
    public class DestinationDirectory : IDirectory {
        public string Path { get; }

        #region Documentation
        /// <summary>
        /// Constructor of the SourceDirectory class.
        /// </summary>
        /// <param name="path">The path of the directory.</param>
        #endregion
        public DestinationDirectory(string path) {
            if (!DirectoryUtils.IsValidPath(path)) { DirectoryUtils.CreatePath(path); }
            this.Path = path;
        }
    }
}