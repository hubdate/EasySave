using Avalonia.Controls;
using EasySave.ViewModels;

namespace EasySave.Views;

public partial class CreateSaveView : UserControl
{
    public CreateSaveView(MainWindowViewModel mainWindowViewModel)
    {
        InitializeComponent();
        DataContext = mainWindowViewModel;
    }
}