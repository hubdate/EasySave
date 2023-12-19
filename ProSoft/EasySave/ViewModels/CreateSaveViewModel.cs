using System.Reactive;
using ReactiveUI;
using Avalonia.Controls;
using System.Collections.ObjectModel;
using Avalonia;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

using EasySave.Models.Data;

namespace EasySave.ViewModels;

public class CreateSaveViewModel : ViewModelBase
{
    private readonly IDialogService _dialogService;
    private readonly Window _mainWindow;
    private bool _isTextReadOnly = true;
    public ReactiveCommand<Unit, Unit> ToggleReadOnlyCommand { get;}
    public ObservableCollection<SaveModel> Saves { get; set; }
    // Définir les propriétés pour accéder aux attributs de l'objet X
    public string SaveName { get; set; }
    public DateTime LastSave { get; set; }
    public string DestFile { get; set; }
    public string SourceFile { get; set; }
    public string State { get; set; }

    public bool IsTextReadOnly
    {
        get { return _isTextReadOnly; }
        set
        {
            _isTextReadOnly = value;
            this.RaisePropertyChanged(nameof(IsTextReadOnly));
        }
    }


    public CreateSaveViewModel(IDialogService dialogService, Window mainWindow)
    {
        ToggleReadOnlyCommand = ReactiveCommand.Create(ToggleReadOnly);
        _dialogService = dialogService;
        _mainWindow = mainWindow;
        Saves = new ObservableCollection<SaveModel>();

        // [Working] ]
        foreach (Save s in Save.GetSaves()) {
            Saves.Add(
                new SaveModel {
                    Name = s.GetName(),
                    Dst = s.destinationDirectory.Path,
                    Src = s.sourceDirectory.Path,
                    State = s.GetStatus().ToString()
                }

                // [ TO DO ] Need to add the last save date
            );
        }
    }

    private void ToggleReadOnly()
    {
        IsTextReadOnly = !IsTextReadOnly;
    }

    public async void OpenFileExplorer()
    {
        var result = await _dialogService.ShowOpenFileDialogAsync();

        if (result != null)
        {
            // Do something with the selected files
        }
    }

    public async void OpenFolderExplorer()
    {
        var dialog = new OpenFolderDialog
        {
            Directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
        };
        var result = await dialog.ShowAsync(_mainWindow);

        if (result != null)
        {
            // Do something with the selected folder
        }
    }

    public async void OpenOSFolderExplorer()
    {
        var dialog = new OpenFolderDialog
        {
            Directory = Environment.GetFolderPath(Environment.SpecialFolder.System)
        };
        var result = await dialog.ShowAsync(_mainWindow);

        if (result != null)
        {
            // Do something with the selected folder
        }
    }

    public async void OpenAppDataFolderExplorer()
    {
        var dialog = new OpenFolderDialog
        {
            Directory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
        };
        var result = await dialog.ShowAsync(_mainWindow);

        if (result != null)
        {
            // Do something with the selected folder
        }
    }
}

public class SaveModel
{
    public string Name { get; set; }
    public string Src { get; set; }
    public string Dst { get; set; }
    public string State { get; set; }
    public string Type { get; set; }
    public int TotalFiles { get; set; }
    public double TotalSize { get; set; }
}