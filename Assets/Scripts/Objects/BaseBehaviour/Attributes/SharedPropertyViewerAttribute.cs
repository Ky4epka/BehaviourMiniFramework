using System;
using Main.Other;

namespace Main.Objects.Behaviours.Attributes
{

    public class SharedPropertyEventDelegateData: EventDelegateData
    {
        public override void PostActivate()
        {
            if (NotUseInEditMode)
                return;

            base.PostActivate();
            OwnerInstance.
                SharedProperty((Attribute as SharedPropertyViewerAttribute).SharedPropertyType).
                EventValueFor(MethodDelegate);
        }
    }

    /// <summary>
    /// Binds property value changes to enabled-state method and invokes it when behaviour has been enabled.
    /// Delegate has been equals <seealso cref="ISharedProperty.EventType"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class SharedPropertyViewerAttribute : ManagedDelegateOfEventAttribute
    {
        public Type SharedPropertyType { get; protected set; }


        /// <summary>
        /// Property listener must have one argument of type of shared property event
        /// </summary>
        /// <param name="sharedPropertyType"></param>
        public SharedPropertyViewerAttribute(Type sharedPropertyType) : base()
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

    }
}