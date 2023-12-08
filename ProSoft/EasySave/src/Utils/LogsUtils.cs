using EasySave.src.Models.Data;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Spectre.Console;
using System;
using System.IO;


namespace EasySave.src.Utils {

    public static class LogsUtils {

        private static readonly string _path = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\EasySave\";

        private static readonly string _dateFormat = DateTime.Now.ToString("dd-MM-yyyy");

        public static void Init() {
            if (!DirectoryUtils.IsValidPath(_path)) {
                AnsiConsole.Clear();
                Directory.CreateDirectory(_path);
            }

            if (File.Exists($"{_path}saves.json")) {
                try {
                    Save.Init(JObject.Parse(File.ReadAllText($"{_path}saves.json")));
                }
                catch {
                    LogSaves();
                }
            }
        }

        #region Documentation
        /// <summary>
        /// Method to log saves to json file.
        /// </summary>
        #endregion
        public static void LogSaves() {
            File.WriteAllText($@"{_path}\saves.json", SavesToJson().ToString());
        }


        #region Documentation
        /// <summary>
        /// Converts all Saves to a JObject.
        /// </summary>
        /// <returns>The JObject representing the Saves.</returns>
        #endregion
        public static JObject SavesToJson() {
            JObject data = new JObject();
            foreach (Save s in Save.GetSaves()) {
                JObject saveData = SaveToJson(s);
                data.Add(s._uuid.ToString(), saveData);
            }
            return data;
        }


        public static JObject SaveToJson(Save s) {
            JobStatus status = s.GetStatus();
            dynamic data = new JObject();

            data.name =           s.GetName();
            data.src  =           s.sourceDirectory.Path;
            data.dst  =           s.destinationDirectory.Path;
            data.state=           status.ToString();
            data.type =           s.GetSaveType().ToString();
            data.totalFiles =     s.sourceDirectory.nbFiles;
            data.totalSize =      s.sourceDirectory.GetSize();

            if (status != JobStatus.WAITING) {
                string[] currentFile = DirectoryUtils.GetCurrentFile();
                data.filesLeft       = s.sourceDirectory.nbFiles - s.GetFilesCopied();
                data.sizeLeft        = s.sourceDirectory.GetSize() - s.GetSizeCopied();
                data.currentTransfertSourcePath = currentFile[0];
                data.currentTransfertDestinationPath = currentFile[1];
                data.progression     = s.CalculateProgress();
            }

            return data;
        }

        public static void LogTransfert(
            Save s,
            string sourcePath,
            string destinationPath,
            long fileSize,
            float fileTransfertTime
        ){
            dynamic transfertInfo = new JObject();
            transfertInfo.name = $"{s.GetName()} ({s._uuid})";
            transfertInfo.fileSource = sourcePath;
            transfertInfo.fileTarget = destinationPath;
            transfertInfo.fileSize = fileSize;
            transfertInfo.transfertTime = fileTransfertTime;
            transfertInfo.date = DateTime.Now;

            string json = JsonConvert.SerializeObject(transfertInfo);
            var arrayJson = JsonConvert.SerializeObject(new[] { transfertInfo }, Formatting.Indented);

            if (File.Exists($@"{_path}data-{_dateFormat}.json")) {
                JArray newJSON = ((JArray)JsonConvert.DeserializeObject(File.ReadAllText($@"{_path}data-{_dateFormat}.json")));
                newJSON.Add(JsonConvert.DeserializeObject(json));
                arrayJson = JsonConvert.SerializeObject(newJSON, Formatting.Indented);
            }
            File.WriteAllText($@"{_path}data-{_dateFormat}.json", arrayJson);
        }

    }
}