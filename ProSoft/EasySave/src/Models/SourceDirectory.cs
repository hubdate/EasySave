using EasySave.src.Utils;
using System.IO;


namespace EasySave.src.Models {

    #region Documentation
    /// <summary>
    /// A model for a source directory.
    /// </summary>
    #endregion
    public class SourceDirectory : IDirectory {
        private readonly double _size;

        public string Path { get; }
        public readonly long nbFiles;


        #region Documentation
        /// <summary>
        /// Constructor of the SourceDirectory class.
        /// </summary>
        /// <param name="path">The path of the directory.</param>
        #endregion
        public SourceDirectory(string path) {
            this.Path = path;
            DirectoryInfo directory = new DirectoryInfo(path);
            this._size = DirectoryUtils.GetDirectorySize(directory);
            this.nbFiles = DirectoryUtils.GetDirectoryNbFiles(directory);
        }

        #region Documentation
        /// <summary>
        /// Returns the size of the directory.
        /// </summary>
        /// <returns>The size of the directory.</returns>
        #endregion
        public double GetSize() { return this._size; }
    }

}