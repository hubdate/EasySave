using EasySave.src.Utils;

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;   
using System.Diagnostics;
using System.Linq;


namespace EasySave.src.Models.Data {

    public abstract class Save {
        public const int MAX_SAVES = 5;

        private static readonly HashSet<Save> _saves = new HashSet<Save>();


        public readonly Guid _uuid;
        private string _name;
        private long _filesCopied;
        private long _sizeCopied;

        protected JobStatus status;
        public readonly SourceDirectory sourceDirectory;
        public readonly DestinationDirectory destinationDirectory;



        protected Save(string name, string src, string dst, Guid guid, JobStatus status = JobStatus.WAITING) {
            this._uuid = guid;
            this._name = name;
            this.sourceDirectory = new SourceDirectory(src);
            this.destinationDirectory = new DestinationDirectory(dst);
            this.status = status;
        }

        public static Save CreateSave(string name, string src, string dst, SaveType type){
            if (_saves.Count >= MAX_SAVES) {
                throw new Exception("Maximum number of saves reached.");
            }
            Save s = type switch {
                SaveType.DIFFERENTIAL => new DifferentialSave(name, src, dst, Guid.NewGuid()),
                SaveType.FULL => new FullSave(name, src, dst, Guid.NewGuid()),
                _ => new FullSave(name, src, dst, Guid.NewGuid())
            };
            _saves.Add(s);
            s.UpdateState();
            return s;
        }


        public int CalculateProgress() {
            return (int)(this._sizeCopied / this.sourceDirectory.GetSize() * 100);
        }


        public void Rename(string newName) {
            this._name = newName;
            this.UpdateState();
        }


        public void Pause()  { 
            this.status = JobStatus.PAUSED;
        }

        public void Resume() { 
            this.status = JobStatus.RUNNING;
        }
        
        public void Cancel() { 
            this.status = JobStatus.CANCELED;
        }

        public void Stop()   { 
            this.status = JobStatus.WAITING;
        }

        public static void Delete(Guid uuid) {
            Save save = _saves.First(save => save._uuid == uuid);
            _saves.Remove(save);
            LogsUtils.LogSaves();
        }

        public string Run() {
            this.status = JobStatus.RUNNING;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            DirectoryUtils.CopyFilesAndFolders(this);
            sw.Stop();
            this.status = JobStatus.FINNISHED;
            return ProcessResults(sw);
        }

        protected string ProcessResults(Stopwatch sw) {
            LogsUtils.LogSaves();
            dynamic result = new JObject();

            result.name =           this._name;
            result.status =         this.status.ToString();
            result.fileCopied =     this._filesCopied;
            result.sizeCopied =     $"{this._sizeCopied / (1024 * 1024)} MB";
            result.timeElapsed =    $"{(int)sw.Elapsed.TotalSeconds} seconds";

            return result.ToString();
        }

        private void UpdateState() {
            LogsUtils.LogSaves();
        }

        public void AddFileCopied() {
            this._filesCopied++;
        }

        public static void Init(JObject jObject) {
            foreach (var save in jObject) {
                if (!DirectoryUtils.IsValidPath(save.Value["src"].ToString())) { return; }
                switch (save.Value["state"].ToString()) {
                    case "DIFFERENTIAL":
                        _saves.Add(
                            new DifferentialSave(
                                save.Value["name"].ToString(),
                                save.Value["src"].ToString(),
                                save.Value["dst"].ToString(),
                                Guid.Parse(save.Key.ToString())
                            )
                        );
                        break;

                    case "FULL":
                    default:
                        _saves.Add(
                            new FullSave(
                                save.Value["name"].ToString(),
                                save.Value["src"].ToString(),
                                save.Value["dst"].ToString(),
                                Guid.Parse(save.Key.ToString()),
                                Save.GetStatus(save.Value["state"].ToString())
                            )
                        );
                        break;
                }
            }
        }

        public long GetSizeCopied() {
            return this._sizeCopied;
        }

        public void AddSizeCopied(long length) {
            this._sizeCopied += length;
        }


        #region Documentation
        /// <summary>
        /// Getter for the saves list.
        /// </summary>
        /// <returns>The saves list.</returns>
        #endregion
        public static HashSet<Save> GetSaves() {
            return _saves;
        }

        public string GetName() {
            return this._name;
        }

        public long GetFilesCopied() {
            return this._filesCopied;
        }

        public JobStatus GetStatus() {
            return this.status;
        }

        public static JobStatus GetStatus(string status) {
            return status switch {
                "RUNNING" => JobStatus.RUNNING,
                "PAUSED" => JobStatus.PAUSED,
                "WAITING" => JobStatus.WAITING,
                "FINNISHED" => JobStatus.FINNISHED,
                "CANCELED" => JobStatus.CANCELED,
                _ => JobStatus.WAITING
            };
        }

        public override abstract string ToString();

        public abstract SaveType GetSaveType();

    }
}