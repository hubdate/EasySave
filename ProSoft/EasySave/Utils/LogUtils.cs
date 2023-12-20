using EasySave.Models.Data;

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;
using System.Linq;

namespace EasySave.Utils {
    public static class LogUtils {
        private static LogFormat __format;
        private static readonly string __date = DateTime.Now.ToString("dd-MM-yyyy");
        private static readonly Mutex __mutex = new Mutex(); // Mutex to prevent multiple threads from writing to the same file at the same time.

        public static readonly string path = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\ProSoft\EasySave\";
    
        public static void Init() {
            if (!DirectoryUtils.IsValidPath(path)) { Directory.CreateDirectory(path); }

            dynamic data;
            switch (true) {
                case var _ when File.Exists($"{path}saves.xml"):
                    __format = LogFormat.XML;
                    data = XDocument.Load($"{path}saves.xml");
                    try { Save.Init(data); } catch { }
                    LogSaves();
                    break;

                case var _ when File.Exists($"{path}saves.json"):
                    __format = LogFormat.JSON;
                    data = JObject.Parse(File.ReadAllText($"{path}saves.json"));
                    try { Save.Init(data); } catch { }
                    LogSaves();
                    break;
            }

            if (!File.Exists($"{path}config.json")) {
                HashSet<string> empty = new HashSet<string>();
                LogConfig(
                    "DEFAULTKEYNOTSECUREDNEEDTOCHANGEIT",
                    empty,
                    empty,
                    empty,
                    -1
                );
            }
        }

        public static void LogSaves() {
            __mutex.WaitOne();
            switch (__format) {
                case LogFormat.XML:
                    new XDocument(SavesToXML()).Save($"{path}saves.xml");
                    break;

                case LogFormat.JSON:
                    File.WriteAllText($"{path}saves.json", SavesToJSON().ToString());
                    break;
                
                default:
                    throw new Exception("Invalid log format");
            };
            __mutex.ReleaseMutex();
        }

        public static JObject SavesToJSON() {
            JObject data = new JObject();
            foreach (Save s in Save.GetSaves()) {
                JObject saveData = SaveToJSON(s);
                data.Add(s.uuid.ToString(), saveData);
            }
            return data;
        }

        public static JObject SaveToJSON(Save s) {
            JobStatus status = s.GetStatus();
            dynamic data = new JObject();

            data.name =         s.GetName();
            data.src =          s.sourceDirectory.Path;
            data.dst =          s.destinationDirectory.Path;
            data.state =        status.ToString();
            data.type =         s.GetSaveType().ToString();
            data.totalFiles =   s.sourceDirectory.nbFiles;
            data.totalSize =    s.sourceDirectory.GetSize();

            if (status != JobStatus.WAITING) {
                string[] currentFile =  DirectoryUtils.GetCurrentFile();
                data.filesLeft =         s.sourceDirectory.nbFiles - s.GetFilesCopied();
                data.sizeLeft =         s.sourceDirectory.GetSize() - s.GetSizeCopied();
                data.currentTransfertSourcePath = currentFile[0];
                data.currentTransfertDestinationPath = currentFile[1];
                data.progression     = s.CalculateProgress(); 
            }

            return data;
        }


        public static XElement SavesToXML() {
            XElement root = new XElement("root");
            foreach (Save s in Save.GetSaves()) {
                XElement saveData = SaveToXML(s);
                root.Add(saveData);
            }
            return root;
        }

        public static XElement SaveToXML(Save s) {
            JobStatus status = s.GetStatus();
            dynamic data = new XElement(
                "item",
                new XAttribute("uuid", s.uuid.ToString()),
                new XElement("name", s.GetName()),
                new XElement("src", s.sourceDirectory.Path),
                new XElement("dst", s.destinationDirectory.Path),
                new XElement("state", status.ToString()),
                new XElement("type", s.GetSaveType().ToString()),
                new XElement("totalFiles", s.sourceDirectory.nbFiles),
                new XElement("totalSize", s.sourceDirectory.GetSize())
            );

            if (status != JobStatus.WAITING) {
                string[] currentFile =  DirectoryUtils.GetCurrentFile();
                data.Add(new XElement("filesLeft", s.sourceDirectory.nbFiles - s.GetFilesCopied()));
                data.Add(new XElement("sizeLeft", s.sourceDirectory.GetSize() - s.GetSizeCopied()));
                data.Add(new XElement("currentTransfertSourcePath", currentFile[0]));
                data.Add(new XElement("currentTransfertDestinationPath", currentFile[1]));
                data.Add(new XElement("progression", s.CalculateProgress()));
            }

            return data;
        }


        public static void LogTransfer(
            Save s,
            string sourcePath,
            string destinationPath,
            long fileSize,
            float fileTransferTime,
            float encryptionTime
        ){
            dynamic transferInfo = new JObject();
            switch (__format) {
                case LogFormat.XML:
                    transferInfo = new XElement(
                        "transfer",
                        new XElement("name", $"{s.GetName()} ({s.uuid})"),
                        new XElement("fileSource", sourcePath),
                        new XElement("fileTarget", destinationPath),
                        new XElement("fileSize", fileSize),
                        new XElement("transferTime", fileTransferTime),
                        new XElement("encryptionTime", encryptionTime),
                        new XElement("date", DateTime.Now)
                    );

                    dynamic data;
                    if (File.Exists($@"{path}data-{__date}.xml")) { data = XDocument.Load($@"{path}data-{__date}.xml"); } 
                    else { data = new XDocument( new XElement("transfers")); }
                    data.Element("transfers").Add(transferInfo);
                    data.Save($@"{path}data-{__date}.xml");
                    break;

                case LogFormat.JSON:
                    transferInfo = new JObject();
                    transferInfo.name =             $"{s.GetName()} ({s.uuid})";
                    transferInfo.fileSource =       sourcePath;
                    transferInfo.fileTarget =       destinationPath;
                    transferInfo.fileSize =         fileSize;
                    transferInfo.transferTime =     fileTransferTime;
                    transferInfo.encryptionTime =   encryptionTime;
                    transferInfo.date =             DateTime.Now;

                    string json = JsonConvert.SerializeObject(transferInfo);
                    var arrayJson = JsonConvert.SerializeObject(new[] { transferInfo }, Formatting.Indented);

                    if (File.Exists($@"{path}data-{__date}.json")) {
                        JArray newJSON = ((JArray)JsonConvert.DeserializeObject(File.ReadAllText($@"{path}data-{__date}.json")));
                        newJSON.Add(JsonConvert.DeserializeObject(json));
                        arrayJson = JsonConvert.SerializeObject(newJSON, Formatting.Indented);
                    }
                    File.WriteAllText($@"{path}data-{__date}.json", arrayJson);
                    break;
                
                default:
                    throw new Exception("Invalid log format");
            };
        }

        public static void ChangeLogFormat(LogFormat format) {
            __format = format;
            LogSaves();
            switch (format) {
                case LogFormat.XML:
                    if (File.Exists($"{path}saves.json")) { File.Delete($"{path}saves.json"); }
                    break;

                case LogFormat.JSON:
                    if (File.Exists($"{path}saves.xml")) { File.Delete($"{path}saves.xml"); }
                    break;

                default:
                    throw new Exception("Invalid log format");
            }
        }

        public static LogFormat GetFormat() {
            return __format;
        }
    

        public static void LogConfig(
            string key,
            HashSet<string> extensionsToEncrypt,
            HashSet<string> process,
            HashSet<string> priorityExtensions,
            int limitSize
        ) {
            JObject data = new JObject(
                new JProperty("key", key),
                new JProperty("cryptosoftExtensions", new JArray(extensionsToEncrypt.Where(k => k.Length > 0))),
                new JProperty("process", new JArray(process.Where(k => k.Length > 0))),
                new JProperty("priorityExtensions", new JArray(priorityExtensions.Where(k => k.Length > 0))),
                new JProperty("limitSize", limitSize)
            );

            string json = JsonConvert.SerializeObject(data);
            File.WriteAllText($"{path}config.json", json);
        }
    }
}