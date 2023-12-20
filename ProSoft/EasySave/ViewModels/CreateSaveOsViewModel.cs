using System.Reactive;
using ReactiveUI;
using Avalonia.Controls;
using System.Collections.ObjectModel;
using Avalonia;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

using EasySave.Views;

namespace EasySave.ViewModels;
public class CreateSaveOsViewModel : ViewModelBase
{
    private readonly IDialogService _dialogService;
    private readonly Window _mainWindow;
    public ReactiveCommand<Unit, Unit> OpenOsExplorerCommand { get;}
    private bool _tableVisibility;

    public bool TableVisibility
    {
        get { return _tableVisibility; }
        // set
        // {
            // if (_tableVisibility != value)
            // {
                set => this.RaiseAndSetIfChanged(ref _tableVisibility, value);
                // _tableVisibility = value;

                // OnPropertyChanged(nameof(TableVisibility));
            // }
        // }
    }
    // public event PropertyChangedEventHandler PropertyChanged;

    // protected virtual void OnPropertyChanged(string propertyName)
    // {
    //     PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    // }

    public CreateSaveOsViewModel(IDialogService dialogService, Window mainWindow)
    {
        OpenOsExplorerCommand = ReactiveCommand.Create(OpenOsExplorer);
        _dialogService = dialogService;
        _mainWindow = mainWindow;
        TableVisibility = false;
    }

    public async void OpenOsExplorer()
    {
        var dialog = new OpenFolderDialog
        {
            Directory = Environment.GetFolderPath(Environment.SpecialFolder.Windows)
        };
        var result = await dialog.ShowAsync(_mainWindow);

        if (result != null)
        {
            // Do something with the selected folder
        }
    }
}
public class TableItem
{
    public string Column1 { get; set; }
    public string Column2 { get; set; }
}

//     public class CreateSaveOsViewModel : ViewModelBase, INotifyPropertyChanged
//     {
//         private bool _tableVisibility;

//         public bool TableVisibility
//         {
//             get { return _tableVisibility; }
//             set
//             {
//                 if (_tableVisibility != value)
//                 {
//                     _tableVisibility = value;
//                     OnPropertyChanged(nameof(TableVisibility));
//                 }
//             }
//         }

//         public CreateSaveOsViewModel() => TableVisibility = false; // Vous pouvez définir la visibilité par défaut ici

//         public event PropertyChangedEventHandler PropertyChanged;

//         protected virtual void OnPropertyChanged(string propertyName)
//         {
//             PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
//         }
//     }
// }
