using System;
using System.Reflection;
using Main.Other;

namespace Main.Objects.Behaviours.Attributes
{
    public class SharedPropertyHandlerEventDelegateData: EventDelegateData
    {
        public SharedPropertyHandlerAttribute CastAttribute => Attribute as SharedPropertyHandlerAttribute;

        public override bool Activate()
        {
            if (IsActive || NotUseInEditMode)
                return false;

            IsActive = true;


            ISharedProperty prop = (OwnerInstance).
                SharedProperty(CastAttribute.SharedPropertyType);

            if (MethodDelegate == null)
            {
                try
                {
                    MethodDelegate = MethodInfo.CreateDelegate(prop.HandlerDelegateType, OwnerInstance);
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.Log(e.Message);
                }
            }

            prop.AddPropertyHandler(MethodDelegate);

            if (CastAttribute.RehandleOnEnabled)
                prop.DirtyValue();

            return true;
        }

        public override bool Deactivate()
        {
            if (!IsActive || NotUseInEditMode)
                return false;

            IsActive = false;

            (OwnerInstance).SharedProperty(CastAttribute.SharedPropertyType).
                    RemovePropertyHandler(MethodDelegate);
            return true;
        }

        public override void PostActivate()
        {
            if (NotUseInEditMode)
                return;

            ISharedProperty prop = (OwnerInstance).
                SharedProperty(CastAttribute.SharedPropertyType);

            if (CastAttribute.RehandleOnEnabled)
                prop.DirtyValue();
        }

        public override bool Prepare(Type ownerType, IObjectBehavioursBase ownerInstance, IDelegateActivableAttribute attribute, MethodInfo methodInfo, bool _notUseInEditMode)
        {
            if (IsPrepared || NotUseInEditMode)
                return false;

            IsPrepared = true;
            OwnerType = ownerType;
            OwnerInstance = ownerInstance;
            Attribute = attribute;
            MethodInfo = methodInfo;
            ExtractedEventType = methodInfo.GetParameters()[0].ParameterType;
            NotUseInEditMode = _notUseInEditMode;

            return true;
        }
    }

    /// <summary>
    /// Property handler must be equals to delegate <seealso cref="SharedPropertyHandler{SharedProperty, ValueType}"/>.
    /// SharedProperty param must be a <seealso cref="ISharedProperty"/> type
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class SharedPropertyHandlerAttribute : MethodBaseAttribute, IDelegateActivableAttribute
    {
        public Type SharedPropertyType { get; protected set; }
        public bool RehandleOnEnabled { get; set; }
        public bool NotUseInEditMode { get; set; }

        public SharedPropertyHandlerAttribute(Type sharedPropertyType)
        {
            if (!typeof(ISharedProperty).IsAssignableFrom(sharedPropertyType))
                throw new InvalidCastException(
                                string.Concat(
                                    "Property type '",
                                    nameof(sharedPropertyType),
                                    "' is not assignable from ",
                                    nameof(ISharedProperty)
                                    )
                                );

            SharedPropertyType = sharedPropertyType;
        }


        public IManagedMethodData CreateData()
        {
            return new SharedPropertyHandlerEventDelegateData();
        }
    }
}