using System;
using System.Windows.Input;
namespace EasySave.ViewModels;

public class HomeViewModel : ViewModelBase
{
    public MainWindowViewModel MainWindowViewModel { get; }

    public HomeViewModel(MainWindowViewModel mainWindowViewModel)
    {
        MainWindowViewModel = mainWindowViewModel;
    }
}