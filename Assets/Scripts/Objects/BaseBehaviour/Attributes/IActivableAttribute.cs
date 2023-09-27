namespace Main.Objects.Behaviours.Attributes
{
    using System;
    using Main.Other;
    using System.Reflection;

    public interface IDelegateActivableAttribute
    {
        bool NotUseInEditMode { get; set; }
        IManagedMethodData CreateData();
    }
}