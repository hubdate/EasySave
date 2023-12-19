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

public class CreateSaveDataAppViewModel : ViewModelBase
{
    private readonly IDialogService _dialogService;
    private readonly Window _mainWindow;
    public ReactiveCommand<Unit, Unit> OpenDataAppExplorerCommand { get;}

    public CreateSaveDataAppViewModel(IDialogService dialogService, Window mainWindow)
    {
        OpenDataAppExplorerCommand = ReactiveCommand.Create(OpenAppDataFolderExplorer);
        _dialogService = dialogService;
        _mainWindow = mainWindow;
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