namespace EasySave.ViewModels;

public class HomeViewModel : ViewModelBase
{
    public MainWindowViewModel MainWindowViewModel { get; set; }

    public HomeViewModel(MainWindowViewModel mainWindowViewModel)
    {
        MainWindowViewModel = mainWindowViewModel;
    }
}