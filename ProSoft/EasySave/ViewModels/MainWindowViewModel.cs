using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using EasySave.Views;
using Avalonia.Controls;
using System.Diagnostics;

namespace EasySave.ViewModels;

public class MainWindowViewModel : INotifyPropertyChanged
{
    private UserControl _currentView;
    public UserControl CurrentView
    {
        get { return _currentView; }
        set
        {
            if (_currentView != value)
            {
                _currentView = value;
                //Console.WriteLine("CurrentView set from: " + new StackTrace());
                OnPropertyChanged();
            }
        }
    }

    public ICommand ChangeViewCommand { get; set; }

    public MainWindowViewModel()
    {
        // Set the initial view
        CurrentView = new HomeView(this);
        // Initialize the command
        ChangeViewCommand = new RelayCommand(ChangeView);
    }

    private void ChangeView(object viewNameObj)
    {
        string viewName = viewNameObj as string;
        Console.WriteLine("Changing view to " + viewName);

        if (viewName == null)
        {
            // Handle the case where viewNameObj is not a string
            return;
        }
        switch (viewName)
        {
            case "View1":
                CurrentView = new CreateSaveView(this);
                break;
            // Add more cases as needed for other views
            default:
                CurrentView = new HomeView(this);
                break;
        }
    }

    // INotifyPropertyChanged implementation
    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        // if (propertyName == nameof(CurrentView))
        // {
        //     Console.WriteLine("CurrentView property changed to " + CurrentView.GetType().Name);
        //     ChangeView(propertyName);
        // }
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }  
}

public class RelayCommand : ICommand
{
    private readonly Action<object> _execute;

    public RelayCommand(Action<object> execute)
    {
        _execute = execute;
    }

    public event EventHandler CanExecuteChanged;
    
    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    public bool CanExecute(object parameter)
    {
        return true;
    }

    public void Execute(object parameter)
    {
        _execute(parameter);
    }
}