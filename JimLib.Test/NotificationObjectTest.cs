using System.ComponentModel;
using FluentAssertions;
using NUnit.Framework;

namespace JimBobBennett.JimLib.Test
{
    [TestFixture]
    public class NotificationObjectTest
    {
        class NotificationTestClass : NotificationObject
        {
            private string _first;
            private string _second;

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

            [NotifyPropertyChangeDependency("SecondToUpper")]
            public string Second
            {
                get { return _second; }
                set
                {
                    if (value == _second) return;
                    _second = value;
                    RaisePropertyChanged();
                }
            }

            public string SecondToUpper { get { return Second.ToUpper(); } }

            public void RaiseForAll()
            {
                RaisePropertyChangeForAll();
            }
        }

        [Test]
        public void SettingPropertyRaisesPropertyChange()
        {
            var x = new NotificationTestClass();

            x.MonitorEvents();

            x.First = "FooBar";

            x.ShouldRaisePropertyChangeFor(v => v.First);
        }

        [Test]
        public void SettingPropertyWithDependentPropertyRaisesPropertyChangeForPropertyAndDependent()
        {
            var x = new NotificationTestClass();

            x.MonitorEvents();

            x.Second = "FooBar";

            x.ShouldRaisePropertyChangeFor(v => v.Second);
            x.ShouldRaisePropertyChangeFor(v => v.SecondToUpper);
        }

        [Test]
        public void RaisingPropertyChangeForAllUsesStringEmpty()
        {
            var x = new NotificationTestClass();

            x.MonitorEvents();

            x.RaiseForAll();

            x.ShouldRaise("PropertyChanged").WithArgs<PropertyChangedEventArgs>(a => a.PropertyName == string.Empty);
        }
    }
}
