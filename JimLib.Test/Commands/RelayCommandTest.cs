using System;
using FluentAssertions;
using JimBobBennett.JimLib.Commands;
using NUnit.Framework;

namespace JimBobBennett.JimLib.Test.Commands
{
    [TestFixture]
    public class RelayCommandTest
    {
        [Test]
        public void CanExecuteIsTrueIfNotSet()
        {
            var command = new RelayCommand(o => { });
            command.CanExecute(null).Should().BeTrue();
        }

        [Test]
        public void CanExecuteUsesPredicateIfSet()
        {
            var canExecute = false;

// ReSharper disable once AccessToModifiedClosure
            var command = new RelayCommand(o => { }, o => canExecute);

            command.CanExecute(null).Should().BeFalse();

            canExecute = true;

            command.CanExecute(null).Should().BeTrue();
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ExecuteCannotBeNull()
        {
// ReSharper disable once UnusedVariable
            var command = new RelayCommand(null);
        }

        [Test]
        public void RaisingCanExecuteChangedRaisesCanExecuteChanged()
        {
            var command = new RelayCommand(o => { });
            command.MonitorEvents();

            command.RaiseCanExecuteChanged();

            command.ShouldRaise("CanExecuteChanged");
        }

        [Test]
        public void ExecuteExecutesTheAction()
        {
            var run = false;

            var command = new RelayCommand(o => { run = true; });

            command.Execute(null);

            run.Should().BeTrue();
        }

        [Test]
        public void ExecuteDoesNotExecuteTheActionIfCanExecuteIsFalse()
        {
            var run = false;

            var command = new RelayCommand(o => { run = true; }, o => false);

            command.Execute(null);

            run.Should().BeFalse();
        }
    }
}
