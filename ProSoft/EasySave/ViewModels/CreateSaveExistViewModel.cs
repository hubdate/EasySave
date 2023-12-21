using System.Reactive;
using ReactiveUI;
using Avalonia.Controls;
using System.Collections.ObjectModel;
using Avalonia;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

using System.ComponentModel;


using EasySave.Views;

using EasySave.Models.Data;
using System.Threading;
using EasySave.Utils;
using System.Windows.Input;

namespace EasySave.ViewModels;

public class CreateSaveExistViewModel : ViewModelBase
{
    private readonly IDialogService _dialogService;
    private readonly Window _mainWindow;

    private Save __save;

    public SaveModel saveModel = new SaveModel();
    public string Name { get; set; }

    private string __progress;
    public string Progress {
        get => __progress;
        set => this.RaiseAndSetIfChanged(ref __progress, value);
    }
    public string Dst { get; set; }
    public string Src { get; set; }
    private string __state;
    public string State {
        get => __state;
        set => this.RaiseAndSetIfChanged(ref __state, value);
    }
    private string __imgSrc = "src_disk.png";
    public string ImgSrc
    {
        get => __imgSrc;
        set => this.RaiseAndSetIfChanged(ref __imgSrc, value);
    }

    public ICommand PauseButton_Click { get; }
    public ICommand ResumeButton_Click { get; }
    public ICommand CancelButton_Click { get; }


    public ReactiveCommand<Unit, Unit> OpenOsExplorerCommand { get;}

    public CreateSaveExistViewModel(IDialogService dialogService, Window mainWindow, string saveName)
    {
        OpenOsExplorerCommand = ReactiveCommand.Create(OpenOsExplorer);
        _dialogService = dialogService;
        _mainWindow = mainWindow;
        Name = saveName;
        GetSaveByName(saveName);
        Dst = saveModel.Dst != null && saveModel.Dst.Length > 40 ? "..." + saveModel.Dst.Substring(saveModel.Dst.Length - 40) : saveModel.Dst;
        Src = saveModel.Src != null && saveModel.Src.Length > 40 ? "..." + saveModel.Src.Substring(saveModel.Src.Length - 40) : saveModel.Src;
        State = saveModel.State;
        Progress = "--";
        
        
        if (File.Exists(saveModel.Src)) { ImgSrc = "/Assets/src_file.png"; } 
        else                            { ImgSrc = "/Assets/src_disk.png"; }

        __save = Secret_GetSaveByName(saveName);
        __save.PropertyChanged += Save_PropertyChanged;

        PauseButton_Click = ReactiveCommand.Create(PauseSave);
        ResumeButton_Click = ReactiveCommand.Create(ResumeSave);
        CancelButton_Click = ReactiveCommand.Create(CancelSave);
    }

    public async void OpenOsExplorer() {
        var dialog = new OpenFolderDialog {
            Directory = Environment.GetFolderPath(Environment.SpecialFolder.Windows)
        };
        var result = await dialog.ShowAsync(_mainWindow);

        if (result != null)
        {
            // Do something with the selected folder
        }
    }

    public SaveModel GetSaveByName(string name)
    {
        foreach (Save s in Save.GetSaves())
        {
            if (s.GetName() == name)
            {
                saveModel = new SaveModel {
                    Name = s.GetName(),
                    Dst = s.destinationDirectory.Path,
                    Src = s.sourceDirectory.Path,
                    State = s.GetStatus().ToString()
                };
            }
        }

        return null; // Return null if no save with the provided name is found
    }

    public void RunSave() {
        new Thread(() => { __save.Run(); }).Start();
        __save.PropertyChanged += Save_PropertyChanged;
    }


    private static Save Secret_GetSaveByName(string name) {
        foreach (Save s in Save.GetSaves()) {
            if (s.GetName() == name) {
                return s;
            }
        }

        return null;
    }

    private void Save_PropertyChanged(object sender, PropertyChangedEventArgs e) {
        Save save = (Save)sender;
        switch (e.PropertyName) {
            case "SizeCopied":
                UpdateProgress(save.CalculateProgress());
                break;
        }
    }

    public void UpdateProgress(int value) {
        Progress = $"{value} %";
        Console.WriteLine(Progress);
    }






    private void PauseSave() {
        __save.Pause();
        DirectoryUtils.PauseTransfer();
        Console.WriteLine("Save has been paused");
    }

    private void ResumeSave() {
        __save.Resume();
        DirectoryUtils.ResumeTransfer();
        Console.WriteLine(__save.GetStatus().ToString());
        Console.WriteLine("Save has been resumed");
    }

    private void CancelSave() {
        Console.WriteLine(__save.GetStatus().ToString());
        __save.Cancel();
        Console.WriteLine(__save.GetStatus().ToString());
        Console.WriteLine("Save has been canceled");
    }


}