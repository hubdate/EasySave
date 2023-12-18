using System.Reactive;
using ReactiveUI;

namespace EasySave.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private ViewModelBase _currentViewModel;

    private readonly ViewModelBase[] Vues;
    
    public ReactiveCommand<Unit, Unit> CreateSaveCommand { get; }
    public ReactiveCommand<Unit, Unit> HomeCommand { get; }

    public ViewModelBase CurrentViewModel
    {
        get => _currentViewModel;
        set => this.RaiseAndSetIfChanged(ref _currentViewModel, value);
    }

    public MainWindowViewModel()
    {
        Vues = new ViewModelBase[]
        {
            new HomeViewModel(),
            new CreateSaveViewModel()
        };
        
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