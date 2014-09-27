using System.Threading.Tasks;
using System.Windows.Input;

namespace JimBobBennett.JimLib.Commands
{
    public interface IAsyncCommand<T> : ICommand, IRaiseCanExecuteChanged
    {
        Task ExecuteAsync(T parameter);
    }

    public interface IAsyncCommand : IAsyncCommand<object>
    {
    }
}
