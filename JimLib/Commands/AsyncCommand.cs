using System;
using System.Threading.Tasks;

namespace JimBobBennett.JimLib.Commands
{
    public class AsyncCommand<T> : AsyncCommandBase<T>
    {
        private readonly Func<T, Task> _command;
        private readonly Func<T, bool> _canExecute;

        public AsyncCommand(Func<T, Task> command, Func<T, bool> canExecute = null)
        {
            _command = command;
            _canExecute = canExecute;
        }

        public override bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute((T)parameter);
        }

        public override Task ExecuteAsync(T parameter)
        {
            return _command(parameter);
        }
    }

    public class AsyncCommand : AsyncCommand<object>, IAsyncCommand
    {
        private readonly Func<Task> _command;
        private readonly Func<object, bool> _canExecute;

        public AsyncCommand(Func<Task> command, Func<object, bool> canExecute = null)
            : base(o => command(), canExecute)
        {
            _command = command;
            _canExecute = canExecute;
        }

        public override bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public override Task ExecuteAsync(object parameter)
        {
            return _command();
        }
    }
}
