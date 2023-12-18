using System.Reactive;
using ReactiveUI;
using Avalonia.Controls;
using Avalonia;
using System;

namespace EasySave.ViewModels;

public class CreateSaveViewModel : ViewModelBase
{
    private readonly IDialogService _dialogService;
    private readonly Window _mainWindow;
    private bool _isTextReadOnly = true;

    public bool IsTextReadOnly
    {
        get { return _isTextReadOnly; }
        set
        {
            _isTextReadOnly = value;
            this.RaisePropertyChanged(nameof(IsTextReadOnly));
        }
    }

    public ReactiveCommand<Unit, Unit> ToggleReadOnlyCommand { get;}

    public CreateSaveViewModel(IDialogService dialogService, Window mainWindow)
    {
        ToggleReadOnlyCommand = ReactiveCommand.Create(ToggleReadOnly);
        _dialogService = dialogService;
        _mainWindow = mainWindow;
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