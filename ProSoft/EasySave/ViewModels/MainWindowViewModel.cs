using System.Reactive;
using ReactiveUI;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Threading.Tasks;
using System;

using EasySave.Models.Data;

namespace EasySave.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private ViewModelBase _currentViewModel;
    private ViewModelBase _insideViewModel;
    private IDialogService dialogService;
    private Window mainWindow;
    private Save __save;

    private readonly ViewModelBase[] Vues;
    private readonly ViewModelBase[] InsideVues;
    
    public ReactiveCommand<Unit, Unit> CreateSaveCommand { get; }
    public ReactiveCommand<Unit, Unit> HomeCommand { get; }
    public ReactiveCommand<Unit, Unit> SourceDestinationCommand { get; }
    public ReactiveCommand<Unit, Unit> CreateSaveFileCommand { get; }
    public ReactiveCommand<Unit, Unit> CreateSaveFolderCommand { get; }
    public ReactiveCommand<Unit, Unit> CreateSaveOsCommand { get; }
    public ReactiveCommand<Unit, Unit> CreateSaveDataAppCommand { get; }
    public ReactiveCommand<Unit, Unit> CreateSaveChoiceCommand { get; }
    public ReactiveCommand<object, Unit> CreateSaveExistCommand { get; }
    public ReactiveCommand<object, Unit> ChangeNameCommand { get; }
    public string saveName = "";
    public ViewModelBase CurrentViewModel
    {
        get => _currentViewModel;
        set => this.RaiseAndSetIfChanged(ref _currentViewModel, value);
    }
    public ViewModelBase InsideViewModel
    {
        get => _insideViewModel;
        set => this.RaiseAndSetIfChanged(ref _insideViewModel, value);
    }

    public MainWindowViewModel(IDialogService dialogService, Window mainWindow)
    {
        
        Vues = new ViewModelBase[]
        {
            new HomeViewModel(),
            new CreateSaveViewModel(dialogService, mainWindow),
        };

        InsideVues = new ViewModelBase[]
        {
            new CreateSaveChoiceViewModel(),
            new CreateSaveDataAppViewModel(dialogService, mainWindow),
            new CreateSaveFileViewModel(dialogService, mainWindow),
            new CreateSaveFolderViewModel(dialogService, mainWindow),
            new CreateSaveOsViewModel(dialogService, mainWindow),
            new CreateSaveExistViewModel(dialogService, mainWindow, saveName),
        };

        this.dialogService = dialogService;
        _currentViewModel = Vues[0];

        _insideViewModel = InsideVues[0];

        CreateSaveCommand = ReactiveCommand.Create(GoCreateSave);
        HomeCommand = ReactiveCommand.Create(GoHome);
        CreateSaveFileCommand = ReactiveCommand.Create(GoCreateSaveFile);
        CreateSaveFolderCommand = ReactiveCommand.Create(GoCreateSaveFolder);
        CreateSaveOsCommand = ReactiveCommand.Create(GoCreateSaveOs);
        CreateSaveDataAppCommand = ReactiveCommand.Create(GoCreateSaveDataApp);
        CreateSaveChoiceCommand = ReactiveCommand.Create(GoCreateSaveChoice);
        CreateSaveExistCommand = ReactiveCommand.Create<object>(GoCreateSaveExist);
        ChangeNameCommand = ReactiveCommand.Create<object>(ChangeName);
    }

    public void GoHome()
    {
        CurrentViewModel = Vues[0];
    }
    
    public void GoCreateSave()
    {
        CurrentViewModel = Vues[1];
    }

    public void GoCreateSaveChoice()
    {
        //Call function to delete temp saves
        try {Save.Delete(__save.uuid);} catch {}
        CurrentViewModel = new CreateSaveViewModel(dialogService, mainWindow);
        InsideViewModel = InsideVues[0];
    }
    
    public void GoCreateSaveDataApp()
    {
        __save = Save.CreateEmptySave();
        CurrentViewModel = new CreateSaveViewModel(dialogService, mainWindow);
        InsideViewModel = new CreateSaveDataAppViewModel(dialogService, mainWindow);
    }
    
    public void GoCreateSaveFile()
    {
        __save = Save.CreateEmptySave();
        CurrentViewModel = new CreateSaveViewModel(dialogService, mainWindow);
        InsideViewModel = new CreateSaveFileViewModel(dialogService, mainWindow);
    }

    public void GoCreateSaveFolder()
    {
        __save = Save.CreateEmptySave();
        CurrentViewModel = new CreateSaveViewModel(dialogService, mainWindow);
        InsideViewModel = new CreateSaveFolderViewModel(dialogService, mainWindow);
    }

    public void GoCreateSaveOs()
    {
        __save = Save.CreateEmptySave();
        CurrentViewModel = new CreateSaveViewModel(dialogService, mainWindow);
        InsideViewModel = new CreateSaveOsViewModel(dialogService, mainWindow);
    }
    public void GoCreateSaveExist(object parameter)
    {
        saveName = parameter as string;
        Console.WriteLine(saveName);
        InsideViewModel = new CreateSaveExistViewModel(dialogService, mainWindow, saveName);
        // InsideViewModel = InsideVues[5];
    }
    public void ChangeName(object parameter)
    {
        string parameters = parameter as string;
        string[] splitParameters = parameters.Split(',');

        string saveName = splitParameters[0];
        string name = splitParameters[1];

        Console.WriteLine(saveName);
        Secret_GetSaveByName(name).Rename(saveName);
        InsideViewModel = new CreateSaveExistViewModel(dialogService, mainWindow, saveName);
    }

    private static Save Secret_GetSaveByName(string name) {
        foreach (Save s in Save.GetSaves()) {
            if (s.GetName() == name) {
                return s;
            }
        }

        return null;
    }
}

public interface IDialogService
{
    Task<string[]> ShowOpenFileDialogAsync();
}

public class DialogService : IDialogService
{
    private readonly Window _window;

    public DialogService(Window window)
    {
        _window = window;
    }

    public async Task<string[]> ShowOpenFileDialogAsync()
    {
        var dialog = new OpenFileDialog();
        return await dialog.ShowAsync(_window);
    }
} 