using EasySave.src.Models.Data;
using EasySave.src.Utils;


namespace EasySave.src {
    public class ViewModel {

        public Save CreateSave(string name, string src, string dst, SaveType type) {
            return Save.CreateSave(name, src, dst, type);
        }

        internal void StopAllSaves() {
            foreach (Save s in Save.GetSaves()) { s.Stop(); }
            LogsUtils.LogSaves();
        }

        public void DeleteSave(Save s)
        {
            Save.Delete(s._uuid);
        }

        public HashSet<Save> GetSavesByUuid(HashSet<string> names)
        {
            return new HashSet<Save>(Save.GetSaves().Where(save => names.Contains(save.ToString())).ToList());
        }

        /// <summary>
        /// Get all saves names
        /// </summary>
        /// <returns>saves names</returns>
        public HashSet<string> GetSaves()
        {
            HashSet<string> data = new HashSet<string>();
            foreach (Save s in Save.GetSaves())
                data.Add(s.ToString());
            return data;
        }
    }
}