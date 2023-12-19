using System.Reactive;
using ReactiveUI;
using Avalonia.Controls;
using System.Collections.ObjectModel;
using Avalonia;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

using EasySave.Views;

using EasySave.Models.Data;

namespace EasySave.ViewModels;

public class CreateSaveExistViewModel : ViewModelBase
{
    private readonly IDialogService _dialogService;
    private readonly Window _mainWindow;
    public SaveModel saveModel = new SaveModel();
    public string Name { get; set; }
    public string Dst { get; set; }
    public string Src { get; set; }
    public string State { get; set; }
    public ReactiveCommand<Unit, Unit> OpenOsExplorerCommand { get;}

    public CreateSaveExistViewModel(IDialogService dialogService, Window mainWindow, string saveName)
    {
        OpenOsExplorerCommand = ReactiveCommand.Create(OpenOsExplorer);
        _dialogService = dialogService;
        _mainWindow = mainWindow;
        Name = saveName;
        GetSaveByName(saveName);
        Dst = saveModel.Dst;
        Src = saveModel.Src;
        State = saveModel.State;
    }

    public async void OpenOsExplorer()
    {
        var dialog = new OpenFolderDialog
        {
            Directory = Environment.GetFolderPath(Environment.SpecialFolder.Windows)
        };
        var result = await dialog.ShowAsync(_mainWindow);

        if (result != null)
        {
            // Do something with the selected folder
        }
    }

    public SaveModel GetSaveByName(string name)
    {
        foreach (Save s in Save.GetSaves())
        {
            if (s.GetName() == name)
            {
                saveModel = new SaveModel {
                    Name = s.GetName(),
                    Dst = s.destinationDirectory.Path,
                    Src = s.sourceDirectory.Path,
                    State = s.GetStatus().ToString()
                };
            }
        }

        return null; // Return null if no save with the provided name is found
    }
}