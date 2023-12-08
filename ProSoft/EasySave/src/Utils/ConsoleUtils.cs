using EasySave.src.Models.Data;

using System.Collections.Generic;
using Spectre.Console;


namespace EasySave.src.Utils {
    class ConsoleUtils {


        /// <summary>
        ///     Display a menu with unique choice
        /// </summary>
        /// <param name="title">The question asked</param>         
        /// <param name="options">The list of options</param>
        /// <param name="lastOption">The last option of the prompt (e.g. back to menu)</param>
        /// <returns>The selected option</returns>
        public static string ChooseAction(string title, HashSet<string> options, string lastOption) {
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
        public static HashSet<string> ChooseMultipleActions(string title, HashSet<string> options, string lastOption) {
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