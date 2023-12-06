// using System;
// using System.Text;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

using Spectre.Console;

using EasySave.src.Utils;

namespace EasySave.src.Render {
    class View {


        private ViewModel _vm;

        // The actual view, represented by the console
        public View() {
            this._vm = new ViewModel();
        }

        public void Start() {
            Console.OutputEncoding = Encoding.UTF8;
            RenderHome();
        }

        private void RenderHome(string message = null) {
            AnsiConsole.Clear();
            AnsiConsole.Write( new FigletText("EasySave").Centered().Color(Color.Red) );
            
            string action = ConsoleUtils.ChooseAction(
                "Choisir une action :", 
                new System.Collections.Generic.HashSet<string>() {
                        "Créer une sauvegarde", 
                        "Charger une sauvegarde", 
                        "Editer une sauvegarde",
                        "Paramètres",
                        "Quitter"
                    }, 
                    null
                );
            switch (action) {
                case "1":
                default:
                    Exit();
                    break;
            }
        }

        internal void Exit(int errno = 0) {
            //this._vm.StopAllSaves();
            Environment.Exit(errno);
        }
    }
}