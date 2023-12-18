using System.Reactive;
using ReactiveUI;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Threading.Tasks;

namespace EasySave.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private ViewModelBase _currentViewModel;
    private readonly IDialogService _dialogService;

    private readonly ViewModelBase[] Vues;
    
    public ReactiveCommand<Unit, Unit> CreateSaveCommand { get; }
    public ReactiveCommand<Unit, Unit> HomeCommand { get; }

    public ViewModelBase CurrentViewModel
    {
        get => _currentViewModel;
        set => this.RaiseAndSetIfChanged(ref _currentViewModel, value);
    }

    public MainWindowViewModel(IDialogService dialogService, Window mainWindow)
    {
        
        Vues = new ViewModelBase[]
        {
            new HomeViewModel(),
            new CreateSaveViewModel(dialogService, mainWindow)
        };

        _dialogService = dialogService;
        _currentViewModel = Vues[0];

        CreateSaveCommand = ReactiveCommand.Create(GoCreateSave);
        HomeCommand = ReactiveCommand.Create(GoHome);
    }

    public void GoCreateSave()
    {
        CurrentViewModel = Vues[1];
    }

    public void GoHome()
    {
        CurrentViewModel = Vues[0];
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