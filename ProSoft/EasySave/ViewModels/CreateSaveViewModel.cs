using System.Reactive;
using ReactiveUI;
using Avalonia.Controls;
using System.Collections.ObjectModel;
using Avalonia;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

using EasySave.Models.Data;

namespace EasySave.ViewModels;

public class CreateSaveViewModel : ViewModelBase
{
    private readonly IDialogService _dialogService;
    private readonly Window _mainWindow;
    public ReactiveCommand<Unit, Unit> ToggleReadOnlyCommand { get;}
    public ObservableCollection<SaveModel> Saves { get; set; }
    // Définir les propriétés pour accéder aux attributs de l'objet X
    public string SaveName { get; set; }
    public DateTime LastSave { get; set; }
    public string DestFile { get; set; }
    public string SourceFile { get; set; }
    public string State { get; set; }

    private bool _isEditable = true;
    public bool IsEditable
    {
        get { return _isEditable; }
        set => this.RaiseAndSetIfChanged(ref _isEditable, value);
    }

    public CreateSaveViewModel(IDialogService dialogService, Window mainWindow)
    {
        ToggleReadOnlyCommand = ReactiveCommand.Create(ToggleReadOnly);
        _dialogService = dialogService;
        _mainWindow = mainWindow;
        Saves = new ObservableCollection<SaveModel>();

        foreach (Save s in Save.GetSaves()) {
            Saves.Add(
                new SaveModel {
                    Name = s.GetName(),
                    Dst = s.destinationDirectory.Path,
                    Src = s.sourceDirectory.Path,
                    State = s.GetStatus().ToString()
                }
            );
        }
    }
    public void ToggleReadOnly()
    {
        IsEditable = !IsEditable;
    }
}
public class SaveModel
{
    public string Name { get; set; }
    public string Src { get; set; }
    public string Dst { get; set; }
    public string State { get; set; }
    public string Type { get; set; }
    public int TotalFiles { get; set; }
    public double TotalSize { get; set; }
}