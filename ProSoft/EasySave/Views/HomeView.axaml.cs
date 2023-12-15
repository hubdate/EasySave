using Avalonia.Controls;
using EasySave.ViewModels;

namespace EasySave.Views;

public partial class HomeView : UserControl
{
    public HomeView(MainWindowViewModel mainWindowViewModel)
    {
        InitializeComponent();
        this.DataContext = mainWindowViewModel;
    }
}