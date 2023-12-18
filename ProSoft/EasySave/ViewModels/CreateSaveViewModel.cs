using System.Reactive;
using ReactiveUI;

namespace EasySave.ViewModels;

public class CreateSaveViewModel : ViewModelBase
{
    private bool _isTextReadOnly = true;

    public bool IsTextReadOnly
    {
        get { return _isTextReadOnly; }
        set
        {
            _isTextReadOnly = value;
            this.RaisePropertyChanged(nameof(IsTextReadOnly));
        }
    }

    public ReactiveCommand<Unit, Unit> ToggleReadOnlyCommand { get;}

    public CreateSaveViewModel()
    {
        ToggleReadOnlyCommand = ReactiveCommand.Create(ToggleReadOnly);
    }

    private void ToggleReadOnly()
    {
        IsTextReadOnly = !IsTextReadOnly;
    }
}  