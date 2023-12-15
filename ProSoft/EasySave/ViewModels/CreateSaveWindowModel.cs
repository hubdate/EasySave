namespace EasySave.ViewModels;

public class CreateSaveWindowModel : ViewModelBase
{
    public MainWindowViewModel MainWindowViewModel { get; set; }
    public CreateSaveWindowModel(MainWindowViewModel mainWindowViewModel)
    {
        MainWindowViewModel = mainWindowViewModel;
    }
}  