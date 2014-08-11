using System.Collections.Generic;
using System.ComponentModel;
using FluentAssertions;
using JimBobBennett.JimLib.Mvvm;
using NUnit.Framework;

namespace JimBobBennett.JimLib.Test.Mvvm
{
    [TestFixture]
    public class ViewModelBaseTest
    {
        class MyModel : NotificationObject
        {
            private string _first;
            private int _second;

            public string First
            {
                get { return _first; }
                set
                {
                    if (value == _first) return;
                    _first = value;
                    RaisePropertyChanged();
                }
            }

            public int Second
            {
                get { return _second; }
                set
                {
                    if (value == _second) return;
                    _second = value;
                    RaisePropertyChanged();
                }
            }
        }

        class MyViewModel : ViewModelBase<MyModel>
        {
            public MyViewModel(MyModel model) : base(model)
            {
                Models = new List<MyModel>();
                RaisedModelPropertyChanges = new List<string>();
            }

            public MyViewModel()
            {
                Models = new List<MyModel>();
                RaisedModelPropertyChanges = new List<string>();
            }

            protected override void OnModelChanged(MyModel oldModel, MyModel newModel)
            {
                base.OnModelChanged(oldModel, newModel);

                if (Models != null)
                {
                    Models.Add(oldModel);
                    Models.Add(newModel);
                }
            }

            protected override void OnModelPropertyChanged(string propertyName)
            {
                base.OnModelPropertyChanged(propertyName);

                RaisedModelPropertyChanges.Add(propertyName);
            }

            public string First
            {
                get { return Model.First; }
                set { Model.First = value; }
            }

            public List<MyModel> Models { get; private set; }
            public List<string> RaisedModelPropertyChanges { get; set; } 
        }

        [Test]
        public void SettingModelCallsOnModelUpdated()
        {
            var model1 = new MyModel();
            var model2 = new MyModel();

// ReSharper disable once UseObjectOrCollectionInitializer
            var vm = new MyViewModel(model1);
            vm.Model = model2;

            vm.Models[0].Should().Be(model1);
            vm.Models[1].Should().Be(model2);
        }

        [Test]
        public void PropertyChangeForPassthroughIsRaised()
        {
            var model = new MyModel();
            var vm = new MyViewModel(model);

            model.MonitorEvents();
            vm.MonitorEvents();

            vm.First = "Hello";

            model.ShouldRaisePropertyChangeFor(m => m.First);
            vm.ShouldRaisePropertyChangeFor(v => v.First);
        }

        [Test]
        public void PropertyChangeOnModelForPassthroughIsRaised()
        {
            var model = new MyModel();
            var vm = new MyViewModel(model);

            model.MonitorEvents();
            vm.MonitorEvents();

            model.First = "Hello";

            model.ShouldRaisePropertyChangeFor(m => m.First);
            vm.ShouldRaisePropertyChangeFor(v => v.First);
        }

        [Test]
        public void PropertyChangeForNonPassthroughIsNotRaised()
        {
            var model = new MyModel();
            var vm = new MyViewModel(model);

            vm.MonitorEvents();
            //model.MonitorEvents();

            model.Second = 100;

            //model.ShouldRaisePropertyChangeFor(m => m.Second);
            vm.ShouldNotRaise("PropertyChanged");
        }

        [Test]
        public void ChangingTheModelRaisesAPropertyChangeForAll()
        {
            var model = new MyModel();
            var vm = new MyViewModel(model);
            vm.MonitorEvents();

            vm.Model = new MyModel();

            vm.ShouldRaise("PropertyChanged").WithArgs<PropertyChangedEventArgs>(e => e.PropertyName == string.Empty);
        }

        [Test]
        public void ChangingTheModelUnsubscribesFromPropertyChangesOnTheOldModel()
        {
            var model = new MyModel();
            var vm = new MyViewModel(model);

            vm.Model = new MyModel();

            vm.MonitorEvents();

            model.First = "Hello";

            vm.ShouldNotRaise("PropertyChanged");
        }

        [Test]
        public void OnModelPropertyChangeForPassthroughIsCalled()
        {
            var model = new MyModel();
            var vm = new MyViewModel(model);
            
            vm.First = "Hello";

            vm.RaisedModelPropertyChanges.Should().OnlyContain(s => s == "First");
        }
    }
}
