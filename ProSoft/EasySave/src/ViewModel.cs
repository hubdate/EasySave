using EasySave.src.Models.Data;
using EasySave.src.Utils;


namespace EasySave.src {
    public class ViewModel {

        public Save CreateSave(string name, string src, string dst, SaveType type) {
            return Save.CreateSave(name, src, dst, type);
        }
    }
}