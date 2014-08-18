namespace JimBobBennett.JimLib.Mvvm
{
    public interface IBusy
    {
        bool IsBusy { get; }

        string BusyMessage { get; set; }
        bool IsActive { get; }
    }
}