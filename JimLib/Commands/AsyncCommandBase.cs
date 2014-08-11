using System;
using System.Threading.Tasks;

namespace JimBobBennett.JimLib.Commands
{
    public abstract class AsyncCommandBase<T> : IAsyncCommand<T>
    {
        public abstract bool CanExecute(object parameter);

        public abstract Task ExecuteAsync(T parameter);

        public async void Execute(object parameter)
        {
            await ExecuteAsync((T)parameter);
        }

        public event EventHandler CanExecuteChanged;
        
        public void RaiseCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}
