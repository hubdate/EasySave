using EasySave.src.Render;
using Spectre.Console;

using EasySave.src.Models.Data;
using EasySave.src.Utils;
using EasySave.src.Render;

namespace EasySave.src {
    static class Program {
        static void Main(string[] args) {
            
            LogsUtils.Init();
            
            View view = new View();
            view.Start();


        }
    }
}