using EasySave.src.Models.Data;
using EasySave.Resources;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

using Spectre.Console.Json;
using Spectre.Console;

using EasySave.src.Utils;
using System.ComponentModel;
using Newtonsoft.Json;

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

        private void RenderHome(string? message = null) {
            AnsiConsole.Clear();
            AnsiConsole.Write( new FigletText("EasySave").Centered().Color(Color.Red) );
            if (message != null) { AnsiConsole.MarkupLine(message); }
            
            string action = ConsoleUtils.ChooseAction(
                $"{Resource.HomeMenu_Title}", 
                new HashSet<string>() {
                    $"{Resource.HomeMenu_Create}",
                    $"{Resource.HomeMenu_Load}",
                    $"{Resource.HomeMenu_Edit}",
                    $"{Resource.HomeMenu_Delete}",
                    $"{Resource.HomeMenu_ChangeLanguage}",
                }, 
                $"{Resource.Forms_Exit}"
            );
            switch (action) {
                case var value when value == Resource.HomeMenu_Create:
                    RenderCreateSave();
                    break;
                case var value when value == Resource.HomeMenu_Load:
                    RenderLoadSave(PromptSave(true));
                    break;

                case var value when value == Resource.HomeMenu_Delete:
                    RenderDeleteSave(PromptSave());
                    break;
                case var value when value == Resource.HomeMenu_Edit:
                    RenderEditSave(PromptSave());
                    break;
                case var value when value == Resource.HomeMenu_ChangeLanguage:
                    RenderChangeLanguage();
                    break;
                case var value when value == Resource.Forms_Exit:
                default:
                    Exit();
                    break;
            }
        }

        private void RenderEditSave(HashSet<Save> saves)
        {
            if (saves.Count != 0)
            {
                //Get selected save
                Save s = saves.Single();
                string oldName = s.GetName();
                //Ask for new name
                string name = ConsoleUtils.Ask(Resource.CreateSave_Name);
                _vm.EditSave(s, name);
                ConsoleUtils.WriteJson(Resource.Confirm, new JsonText(LogsUtils.SaveToJson(s).ToString()));
                //Ask to confirm
                if (ConsoleUtils.AskConfirm())
                {
                    RenderHome($"[yellow]{Resource.Save_Renamed} ({s._uuid})[/]");
                }
                else
                {
                    //Get back old name
                    _vm.EditSave(s, oldName);
                    RenderHome();
                }
            }
            else
                RenderHome();
        }

        private void RenderCreateSave() {
            if (Save.GetSaves().Count >= Save.MAX_SAVES) {
                RenderHome($"[red]{Resource.CreateSave_MaxSaves}[/]");
            }

            string name = ConsoleUtils.Ask($"{Resource.CreateSave_Name}");
            string src = ConsoleUtils.Ask($"{Resource.CreateSave_SourcePath}");
            while (!DirectoryUtils.IsValidPath(src)) {
                ConsoleUtils.WriteError(Resource.Path_Invalid);
                src = ConsoleUtils.Ask($"{Resource.CreateSave_SourcePath}");
            }

            string dst = ConsoleUtils.Ask($"{Resource.CreateSave_DestinationPath}");
            if (!DirectoryUtils.IsValidPath(dst)) {
                new DirectoryInfo(dst).Create();
            }

            Dictionary<string, SaveType> DictSaveTypes = new Dictionary<string, SaveType>() {
                {Resource.CreateSave_Type_Full, SaveType.FULL},
                {Resource.CreateSave_Type_Differential, SaveType.DIFFERENTIAL}
            };

            SaveType type = DictSaveTypes[
                ConsoleUtils.ChooseAction(
                    $"{Resource.CreateSave_Type}", 
                    new HashSet<string>(DictSaveTypes.Keys)
                )
            ];

            dynamic data = new JObject();
            data.name = name;
            data.src = src;
            data.dst = dst;
            data.type = type.ToString();
            ConsoleUtils.WriteJson(Resource.Confirm, new JsonText(data.ToString()));
            
            bool askConfirmation = ConsoleUtils.ChooseAction(
                Resource.Confirm, 
                new HashSet<string>() {
                    {Resource.Confirm_Yes},
                    {Resource.Confirm_No}
                }
            ) == Resource.Confirm_Yes;

            string message;
            if (askConfirmation) {
                Save s = this._vm.CreateSave(name, src, dst, type);
                message = $"[green]{Resource.CreateSave_Success} ({s._uuid})[/]";
            }
            else {
                message = $"[red]Save creation aborted![/]";
            }

            RenderHome(message);

        }

        private void RenderLoadSave(HashSet<Save> saves) {
            if (saves.Count == 0) { RenderHome(); }
            else {
                foreach (Save s in saves) {
                    try {
                        if (s.GetStatus() != JobStatus.WAITING && s.GetStatus() != JobStatus.FINNISHED) 
                        { RenderHome($"[red]{Resource.Save_AlreadyRunning}[/]"); }
                        
                        else
                        {
                            ConsoleUtils.WriteJson(Resource.Save_Execute, new JsonText(LogsUtils.SaveToJson(s).ToString()));
                            if (ConsoleUtils.AskConfirm()) {
                                ConsoleUtils.WriteJson(Resource.Save_Info, new JsonText(this._vm.RunSave(s)), Color.Green);
                            }
                            else {
                                RenderHome();
                            }
                        }
                    }
                    catch
                    {
                        ConsoleUtils.WriteError($"{Resource.Exception}");
                        Exit(-1);
                    }
                }
            }
            ConsoleUtils.AskConfirm(true);
            RenderHome();
        }


        private void RenderDeleteSave(HashSet<Save> saves)
        {
            if (saves.Count != 0)
            {
                //Get selected save
                Save s = saves.Single();
                ConsoleUtils.WriteJson(Resource.Confirm, new JsonText(LogsUtils.SaveToJson(s).ToString()));
                //Ask confirm
                string message;
                if (ConsoleUtils.AskConfirm())
                {
                    _vm.DeleteSave(s);
                    message = $"[green]{Resource.Save_Deleted}[/]";
                }
                else {
                    message = $"[red]{Resource.Save_Undeleted}[/]";
                }
                RenderHome(message);
            }
            else
                RenderHome();
        }

        private HashSet<Save> PromptSave(bool multi = false)
        {
            HashSet<string> saves = new HashSet<string>();
            //Ask for multi or single save
            if (multi)
                saves = ConsoleUtils.ChooseMultipleActions(Resource.SaveMenu_Title, _vm.GetSaves(), null);
            else
            {
                string save = ConsoleUtils.ChooseAction(Resource.SaveMenu_Title, _vm.GetSaves(), Resource.Forms_Back);
                if (save != Resource.Forms_Back && save != Resource.Forms_Exit)
                    saves.Add(save);
            }
            return _vm.GetSavesByUuid(saves);
        }


        private void RenderChangeLanguage() {
            string language = ConsoleUtils.ChooseAction(
                $"{Resource.ChangeLanguage}", 
                new HashSet<string>() {
                    $"{Resource.Language_en_US}",
                    $"{Resource.Language_fr_FR}",
                    $"{Resource.Language_it_IT}",
                    $"{Resource.Language_es_ES}"
                }, 
                $"{Resource.Forms_Back}"
            );
            string culture = CultureInfo.CurrentCulture.ToString();
            switch (language) {
                case var value when value == Resource.Language_fr_FR:
                    culture = "fr-FR";
                    break;
                case var value when value == Resource.Language_it_IT:
                    culture = "it-IT";
                    break;
                case var value when value == Resource.Language_es_ES:
                    culture = "es-ES";
                    break;
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