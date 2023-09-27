using System;
using Main.Other;
using System.Reflection;

namespace Main.Objects.Behaviours.Attributes
{
    public interface IManagedMethodData
    {
        IDelegateActivableAttribute Attribute { get; }
        MethodInfo MethodInfo { get;}
        bool IsActive { get; }
        bool IsPrepared { get;}
        Type OwnerType { get;}
        IObjectBehavioursBase OwnerInstance { get;}
        Type ExtractedEventType { get;}
        Delegate MethodDelegate { get;}
        bool NotUseInEditMode { get; }

        bool Prepare(
            Type ownerType, 
            IObjectBehavioursBase ownerInstance,
            IDelegateActivableAttribute attribute,
            MethodInfo methodInfo, bool _useInEditMode);

        bool Activate();
        void PostActivate();
        bool Deactivate();
    }


}