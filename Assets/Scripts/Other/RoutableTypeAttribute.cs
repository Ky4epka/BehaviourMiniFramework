using System;

namespace Main
{
    /// <summary>
    /// Routable type must have an base constructor without args
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public class RoutableTypeAttribute : Attribute
    {
        public RoutableTypeAttribute()
        {

        }
    }
}
