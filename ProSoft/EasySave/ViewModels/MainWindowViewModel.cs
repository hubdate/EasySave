using System.Reactive;
using ReactiveUI;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Threading.Tasks;

namespace EasySave.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private ViewModelBase _currentViewModel;
    private ViewModelBase _insideViewModel;
    private IDialogService dialogService;
    private Window mainWindow;

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
        InsideViewModel = InsideVues[0];
    }
    
    public void GoCreateSaveDataApp()
    {
        InsideViewModel = InsideVues[1];
    }
    
    public void GoCreateSaveFile()
    {
        InsideViewModel = InsideVues[2];
    }

    public void GoCreateSaveFolder()
    {
        InsideViewModel = InsideVues[3];
    }

    public void GoCreateSaveOs()
    {
        InsideViewModel = InsideVues[4];
    }
    public void GoCreateSaveExist(object parameter)
    {
        saveName = parameter as string;
        InsideViewModel = new CreateSaveExistViewModel(dialogService, mainWindow, saveName);
        // InsideViewModel = InsideVues[5];
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