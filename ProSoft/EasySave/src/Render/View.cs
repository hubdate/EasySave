using EasySave.src.Models.Data;
using EasySave.Resources;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

using Spectre.Console;

using EasySave.src.Utils;
using System.ComponentModel;

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
            if (message != null) { AnsiConsole.MarkupLine(message); }
            
            string action = ConsoleUtils.ChooseAction(
                $"{Resource.HomeMenu_Title}", 
                new HashSet<string>() {
                    $"{Resource.HomeMenu_Create}", 
                    $"{Resource.HomeMenu_Edit}",
                    $"{Resource.HomeMenu_Delete}",
                    $"{Resource.HomeMenu_ChangeLanguage}",
                }, 
                $"{Resource.Forms_Exit}"
            );
            switch (action) {
                case var value when value == Resource.HomeMenu_ChangeLanguage:
                    RenderChangeLanguage();
                    break;
                
                case var value when value == Resource.Forms_Exit:
                default:
                    Exit();
                    break;
            }
        }



        private void RenderCreateSave() {
            // if (Save.GetSaves().Count >= Save.MAX_SAVES) {
            //     AnsiConsole.MarkupLine("[red]Le nombre maximum de sauvegardes a été atteint.[/]");
            //     RenderHome();
            // }

            // Save s = this._vm.CreateSave(
            //     "test", 
            //     @"D:\GitHub\EasySave\ProSoft\EasySave\", 
            //     @"D:\GitHub\EasySave\ProSoft\EasySave\", 
            //     SaveType.FULL
            // );
            // RenderHome($"[green]{Resource.CreateSave_Success} ({s._uuid})[/]");
        }

        private void RenderChangeLanguage() {
            string language = ConsoleUtils.ChooseAction(
                $"{Resource.ChangeLanguage}", 
                new HashSet<string>() {
                    $"{Resource.Language_en_US}"
                }, 
                $"{Resource.Forms_Back}"
            );
            string culture = CultureInfo.CurrentCulture.ToString();
            switch (language) {
                case var value when value == Resource.Language_en_US:
                    culture = "en-US";
                    break;
            }

            CultureInfo cultureInfo = new CultureInfo(culture);
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
            RenderHome();
        }

        internal void Exit(int errno = 0) {
            this._vm.StopAllSaves();
            Environment.Exit(errno);
        }
    }
}