using System;

namespace JimBobBennett.JimLib
{
    [AttributeUsage(AttributeTargets.All, Inherited = true)]
    public class PreserveAttribute : Attribute
    {
        public bool Conditional { get; set; }
    }
}
