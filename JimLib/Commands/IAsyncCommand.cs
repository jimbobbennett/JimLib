using System.Threading.Tasks;
using System.Windows.Input;

namespace JimBobBennett.JimLib.Commands
{
    public interface IAsyncCommand<T> : ICommand
    {
        Task ExecuteAsync(T parameter);
        void RaiseCanExecuteChanged();
    }

    public interface IAsyncCommand : IAsyncCommand<object>
    {
    }
}
