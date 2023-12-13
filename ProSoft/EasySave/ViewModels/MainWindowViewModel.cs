using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using EasySave.Views;
using Avalonia.Controls;

public class MainWindowViewModel : INotifyPropertyChanged
{
    private UserControl _currentView;
    public UserControl CurrentView
    {
        get { return _currentView; }
        set
        {
            _currentView = value;
            OnPropertyChanged();
        }
    }

    public ICommand ChangeViewCommand { get; set; }

    public MainWindowViewModel()
    {
        // Set the initial view
        CurrentView = new HomeView();

        // Initialize the command
        ChangeViewCommand = new RelayCommand<string>(ChangeView);
    }

    private void ChangeView(string viewName)
    {
        switch (viewName)
        {
            case "CreateSave":
                CurrentView = new CreateSaveView();
                break;
            // Add more cases as needed for other views
            default:
                CurrentView = new HomeView();
                break;
        }
    }

    // INotifyPropertyChanged implementation
    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public class RelayCommand<T> : ICommand
{
    private readonly Action<T> _execute;
    private readonly Predicate<T> _canExecute;

    public RelayCommand(Action<T> execute, Predicate<T> canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public bool CanExecute(object parameter)
    {
        return _canExecute == null || _canExecute((T)parameter);
    }

    public void Execute(object parameter)
    {
        _execute((T)parameter);
    }

    public event EventHandler CanExecuteChanged;

    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}