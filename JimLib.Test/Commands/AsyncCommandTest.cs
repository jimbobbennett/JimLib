using System;
using System.Threading.Tasks;
using FluentAssertions;
using JimBobBennett.JimLib.Commands;
using NUnit.Framework;

namespace JimBobBennett.JimLib.Test.Commands
{
    [TestFixture]
    public class GenericAsyncCommandTest
    {
        private static async Task AsyncMethod(Action action)
        {
            await Task.Factory.StartNew(action);
        }

        [Test]
        public void CanExecuteIsTrueIfNotSet()
        {
            var command = new AsyncCommand<object>(async o => await AsyncMethod(() => { }));
            command.CanExecute(null).Should().BeTrue();
        }

        [Test]
        public void CanExecuteUsesPredicateIfSet()
        {
            var canExecute = false;

            // ReSharper disable once AccessToModifiedClosure
            var command = new AsyncCommand<object>(async o => await AsyncMethod(() => { }), o => canExecute);

            command.CanExecute(null).Should().BeFalse();

            canExecute = true;

            command.CanExecute(null).Should().BeTrue();
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ExecuteCannotBeNull()
        {
            // ReSharper disable once UnusedVariable
            var command = new AsyncCommand<object>(null);
        }

        [Test]
        public void RaisingCanExecuteChangedRaisesCanExecuteChanged()
        {
            var command = new AsyncCommand<object>(async o => await AsyncMethod(() => { }));
            command.MonitorEvents();

            command.RaiseCanExecuteChanged();

            command.ShouldRaise("CanExecuteChanged");
        }

        [Test]
        public async void ExecuteAsyncExecutesTheAction()
        {
            var run = false;

            var command = new AsyncCommand<object>(async o =>
                {
                    await AsyncMethod(() => run = true);
                });

            await command.ExecuteAsync(null);

            run.Should().BeTrue();
        }

        [Test]
        public void ExecuteExecutesTheAction()
        {
            var run = false;

            var command = new AsyncCommand<object>(async o =>
            {
                await AsyncMethod(() => run = true);
            });

            command.Execute(null);

            run.Should().BeTrue();
        }

        [Test]
        public void ExecuteDoesNotExecuteTheActionIfCanExecuteIsFalse()
        {
            var run = false;

            var command = new AsyncCommand<object>(async o =>
            {
                await AsyncMethod(() => run = true);
            }, o => false);

            command.Execute(null);

            run.Should().BeFalse();
        }

        [Test]
        public async void ExecuteAsyncDoesNotExecuteTheActionIfCanExecuteIsFalse()
        {
            var run = false;

            var command = new AsyncCommand<object>(async o =>
            {
                await AsyncMethod(() => run = true);
            }, o => false);

            await command.ExecuteAsync(null);

            run.Should().BeFalse();
        }
    }

    [TestFixture]
    public class AsyncCommandTest
    {
        private static async Task AsyncMethod(Action action)
        {
            await Task.Factory.StartNew(action);
        }

        [Test]
        public void CanExecuteIsTrueIfNotSet()
        {
            var command = new AsyncCommand(async () => await AsyncMethod(() => { }));
            command.CanExecute(null).Should().BeTrue();
        }

        [Test]
        public void CanExecuteUsesPredicateIfSet()
        {
            var canExecute = false;

            // ReSharper disable once AccessToModifiedClosure
            var command = new AsyncCommand(async () => await AsyncMethod(() => { }), o => canExecute);

            command.CanExecute(null).Should().BeFalse();

            canExecute = true;

            command.CanExecute(null).Should().BeTrue();
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ExecuteCannotBeNull()
        {
            // ReSharper disable once UnusedVariable
            var command = new AsyncCommand(null);
        }

        [Test]
        public void RaisingCanExecuteChangedRaisesCanExecuteChanged()
        {
            var command = new AsyncCommand(async () => await AsyncMethod(() => { }));
            command.MonitorEvents();

            command.RaiseCanExecuteChanged();

            command.ShouldRaise("CanExecuteChanged");
        }

        [Test]
        public async void ExecuteAsyncExecutesTheAction()
        {
            var run = false;

            var command = new AsyncCommand(async () =>
            {
                await AsyncMethod(() => run = true);
            });

            await command.ExecuteAsync(null);

            run.Should().BeTrue();
        }

        [Test]
        public void ExecuteExecutesTheAction()
        {
            var run = false;

            var command = new AsyncCommand(async () =>
            {
                await AsyncMethod(() => run = true);
            });

            command.Execute(null);

            run.Should().BeTrue();
        }

        [Test]
        public void ExecuteDoesNotExecuteTheActionIfCanExecuteIsFalse()
        {
            var run = false;

            var command = new AsyncCommand(async () =>
            {
                await AsyncMethod(() => run = true);
            }, o => false);

            command.Execute(null);

            run.Should().BeFalse();
        }

        [Test]
        public async void ExecuteAsyncDoesNotExecuteTheActionIfCanExecuteIsFalse()
        {
            var run = false;

            var command = new AsyncCommand(async () =>
            {
                await AsyncMethod(() => run = true);
            }, o => false);

            await command.ExecuteAsync(null);

            run.Should().BeFalse();
        }
    }
}
