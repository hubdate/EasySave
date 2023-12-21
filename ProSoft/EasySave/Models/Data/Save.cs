using EasySave.Utils;

using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;
using System.IO;
using System.Threading;
using System.Threading.Tasks;



namespace EasySave.Models.Data {
    public abstract class Save : INotifyPropertyChanged {
        private static readonly HashSet<Save> __saves = new HashSet<Save>();
        private string __name;
        private long __fileCopied;
        private long __sizeCopied;

        protected JobStatus _status;

        public readonly Guid uuid;
        public readonly SourceDirectory sourceDirectory;
        public readonly DestinationDirectory destinationDirectory;
        public event PropertyChangedEventHandler PropertyChanged;


        protected Save(string name, string src, string dst, Guid uuid, JobStatus status = JobStatus.WAITING) {
            this.uuid = uuid;
            this.__name = name;
            this.sourceDirectory = new SourceDirectory(src);
            this.destinationDirectory = new DestinationDirectory(dst);
            this._status = status;
        }

        public static Save CreateSave(string name, string src, string dst, SaveType type) {
            Save s = type switch {
                SaveType.DIFFERENTIAL => new DifferentialSave(name, src, dst, Guid.NewGuid()),
                SaveType.FULL => new FullSave(name, src, dst, Guid.NewGuid()),
                _ => new FullSave(name, src, dst, Guid.NewGuid())
            };
            __saves.Add(s);
            s.UpdateState();
            return s;
        }

        public static Save CreateEmptySave() {
            Save s = new FullSave(
                "New save",
                $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\ProSoft\EasySave",
                $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\ProSoft\EasySave",
                Guid.NewGuid());
            __saves.Add(s);
            s.OnPropertyChanged("Saves");

            return s;
        }

        public int CalculateProgress() {
            return Math.Min((int)(__sizeCopied / sourceDirectory.GetSize() * 100), 100);
        }

        public void Rename(string newName) {
            __name = newName;
            UpdateState();
        }

        public void Pause()  { 
            _status = JobStatus.PAUSED;
        }

        public void Resume() { 
            _status = JobStatus.RUNNING;
        }
        
        public void Cancel() { 
            _status = JobStatus.CANCELED;
        }

        public void Stop()   { 
            _status = JobStatus.WAITING;
            __fileCopied = 0;
            __sizeCopied = 0;
        }

        public static void Delete(Guid uuid) {
            Save save = __saves.First(save => save.uuid == uuid);
            __saves.Remove(save);
            LogUtils.LogSaves();
        }

        public void Run() {
            _status = JobStatus.RUNNING;
            
            SocketUtils.StartServer(this);
            DirectoryUtils.CopyFilesAndFolders(this);
        }

        private void UpdateState() {
            LogUtils.LogSaves();
        }

        public void AddFileCopied() {
            __fileCopied++;
        }


        public static void Init(dynamic data) {
            LogFormat format = LogUtils.GetFormat();
            switch (format) {
                case LogFormat.XML:
                    foreach (var save in data.Root.Elements()) {
                        if (!DirectoryUtils.IsValidPath(save.Element("src").Value.ToString())) { return; }
                        switch (save.Element("type").Value.ToString()) {
                            case "DIFFERENTIAL":
                                __saves.Add(
                                    new DifferentialSave(
                                        save.Element("name").Value.ToString(),
                                        save.Element("src").Value.ToString(),
                                        save.Element("dst").Value.ToString(),
                                        Guid.Parse(save.Attribute("uuid").Value.ToString())
                                    )
                                );
                                break;

                            case "FULL":
                            default:
                                __saves.Add(
                                    new FullSave(
                                        save.Element("name").Value.ToString(),
                                        save.Element("src").Value.ToString(),
                                        save.Element("dst").Value.ToString(),
                                        Guid.Parse(save.Attribute("uuid").Value.ToString()),
                                        Save.GetStatus(save.Element("state").Value.ToString())
                                    )
                                );
                                break;
                        }
                    }
                    break;

                case LogFormat.JSON:
                    foreach (var save in data) {
                        if (!DirectoryUtils.IsValidPath(save.Value["src"].ToString())) { return; }
                        switch (save.Value["type"].ToString()) {
                            case "DIFFERENTIAL":
                                __saves.Add(
                                    new DifferentialSave(
                                        save.Value["name"].ToString(),
                                        save.Value["src"].ToString(),
                                        save.Value["dst"].ToString(),
                                        Guid.Parse(save.Name.ToString())
                                    )
                                );
                                break;

                            case "FULL":
                            default:
                                __saves.Add(
                                    new FullSave(
                                        save.Value["name"].ToString(),
                                        save.Value["src"].ToString(),
                                        save.Value["dst"].ToString(),
                                        Guid.Parse(save.Name.ToString()),
                                        Save.GetStatus(save.Value["state"].ToString())
                                    )
                                );
                                break;
                        }
                    }
                    break;

                default:
                    throw new Exception("Invalid log format");
            };
            
            foreach (Save s in __saves) { s._status = JobStatus.WAITING; }
        }

        public long GetSizeCopied() {
            return __sizeCopied;
        }

        public void AddSizeCopied(long length) {
            __sizeCopied += length;
            OnPropertyChanged("SizeCopied");
        }

        public static HashSet<Save> GetSaves() {
            return __saves;
        }

        public string GetName() {
            return __name;
        }

        public long GetFilesCopied() {
            return __fileCopied;
        }

        public JobStatus GetStatus() {
            return _status;
        }

        public static JobStatus GetStatus(string status) {
            return status switch {
                "RUNNING" => JobStatus.RUNNING,
                "PAUSED" => JobStatus.PAUSED,
                "WAITING" => JobStatus.WAITING,
                "FINISHED" => JobStatus.FINISHED,
                "CANCELED" => JobStatus.CANCELED,
                _ => JobStatus.WAITING
            };
        }

        public override abstract string ToString();

        public abstract SaveType GetSaveType();

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null) {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public void MarkAsFinished() {
            _status = JobStatus.FINISHED;
        }
    }
}