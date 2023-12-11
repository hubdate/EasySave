using EasySave.src.Models.Data;
using EasySave.Resources;

using System;
using System.Threading;
using System.Collections.Generic;
using Spectre.Console;
using Spectre.Console.Json;
using Newtonsoft.Json;


namespace EasySave.src.Utils {
    class ConsoleUtils {


        /// <summary>
        ///     Display a menu with unique choice
        /// </summary>
        /// <param name="title">The question asked</param>         
        /// <param name="options">The list of options</param>
        /// <param name="lastOption">The last option of the prompt (e.g. back to menu)</param>
        /// <returns>The selected option</returns>
        public static string ChooseAction(string title, HashSet<string> options, string? lastOption = null) {
            if (lastOption != null) { options.Add(lastOption); }
            
            return (
                AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title(title)
                        .PageSize(10)
                        .AddChoices(options)
                )
            );
        }


        /// <summary>
        ///     Display a menu with multiple choices
        /// </summary>
        /// <param name="title">The question asked</param>
        /// <param name="options">The list of options</param>
        /// <param name="lastOption">The last option of the prompt (e.g. back to menu)</param>
        /// <returns>The selected options</returns>
        public static HashSet<string> ChooseMultipleActions(string title, HashSet<string> options, string? lastOption = null) {
            if (lastOption != null) { options.Add(lastOption); }
            
            return (
                new HashSet<string>(
                    AnsiConsole.Prompt(
                    new MultiSelectionPrompt<string>()
                        .Title(title)
                        .PageSize(10)
                        .AddChoices(options)
                    )
                )
            );
        }

        public static void WriteJson(string title, JsonText data, Color? color = null) {
            AnsiConsole.Write(
                new Panel(data)
                    .Header(title)
                    .Collapse()
                    .RoundedBorder()
                    .BorderColor(color ?? Color.Yellow)
            );
        }

        public static string Ask(string title, string? errorMessage = null) {
            return AnsiConsole.Prompt(
                new TextPrompt<string>(title)
                    .PromptStyle("blue")
                    .ValidationErrorMessage(errorMessage ?? "")
                    .Validate(prompt => !String.IsNullOrEmpty(prompt.Trim()))
            );
        }

        public static bool AskConfirm(bool withoutChoice = false)
        {
            if (withoutChoice)
                return ChooseAction("", new HashSet<string>() { Resource.Forms_Back }, null) == Resource.Confirm_Yes;
            else
                return ChooseAction(Resource.Confirm, new HashSet<string>() { Resource.Confirm_Yes, Resource.Confirm_No }, null) == Resource.Confirm_Yes;
        }

        public static void WriteError(string errorMessage) {
            AnsiConsole.MarkupLine($"[red]{errorMessage}[/]");
        }


        public static void CreateProgressBar(Save s) {
            AnsiConsole.Progress()
                .Columns(
                    new TaskDescriptionColumn(),
                    new ProgressBarColumn(),
                    new PercentageColumn(),
                    new RemainingTimeColumn(),
                    new SpinnerColumn()
                )
                .Start( context => {
                    var progress = context.AddTask($"Copying files...", maxValue: s.sourceDirectory.GetSize());
                    while (!context.IsFinished) {
                        Thread.Sleep(1000);
                        progress.Value = s.GetSizeCopied();
                    }
                });
        }
    }
}