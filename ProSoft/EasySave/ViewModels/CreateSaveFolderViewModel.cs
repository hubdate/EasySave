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

namespace EasySave.ViewModels;

public class CreateSaveFolderViewModel : ViewModelBase
{
    private readonly IDialogService _dialogService;
    private readonly Window _mainWindow;
    public ReactiveCommand<Unit, Unit> OpenFolderExplorerCommand { get;}

    public CreateSaveFolderViewModel(IDialogService dialogService, Window mainWindow)
    {
        OpenFolderExplorerCommand = ReactiveCommand.Create(OpenFolderExplorer);
        _dialogService = dialogService;
        _mainWindow = mainWindow;
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
    
}