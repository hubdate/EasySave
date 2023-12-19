using System.Reactive;
using ReactiveUI;
using Avalonia.Controls;
using System.Collections.ObjectModel;
using Avalonia;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.ComponentModel;

using EasySave.Views;

namespace EasySave.ViewModels;

public class CreateSaveFileViewModel : ViewModelBase
{
    private readonly IDialogService _dialogService;
    private readonly Window _mainWindow;
    public ReactiveCommand<Unit, Unit> OpenFileExplorerCommand { get;}
    public ReactiveCommand<Unit, Unit> OpenFolderExplorerCommand { get;}

    private string _sourceFileName;
    public string SourceFileName
    {
        get { return _sourceFileName; }
        set => this.RaiseAndSetIfChanged(ref _sourceFileName, value);
    }

    private string _destinationFileName;
    public string DestinationFileName
    {
        get { return _destinationFileName; }
        set => this.RaiseAndSetIfChanged(ref _destinationFileName, value);
    }

    public CreateSaveFileViewModel(IDialogService dialogService, Window mainWindow)
    {
        OpenFileExplorerCommand = ReactiveCommand.Create(OpenFileExplorer);
        OpenFolderExplorerCommand = ReactiveCommand.Create(OpenFolderExplorer);
        _dialogService = dialogService;
        _mainWindow = mainWindow;
        SourceFileName = "Select a File";
        DestinationFileName = "Select a Folder";
    }

    public async void OpenFileExplorer()
    {
        var dialog = new OpenFileDialog
        {

        };
        var result = await dialog.ShowAsync(_mainWindow);

        if (result != null)
        {
            SourceFileName = result[0];
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
            DestinationFileName = result;
        }
    }
}