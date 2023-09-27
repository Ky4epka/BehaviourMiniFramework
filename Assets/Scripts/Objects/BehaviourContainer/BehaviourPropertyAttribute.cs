using System;
using Main.Objects.Behaviours;
using Main.Other;

namespace Main.Objects
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class BehaviourPropertyAttribute : Attribute, IPropertyInjectorAttribute
    {
        public object Inject(object target, TypeReflector.PropertyReflector propReflector)
        {
            if (!(typeof(IBehaviourContainer)).IsAssignableFrom(target.GetType()))
                throw new InvalidCastException($"Target type {target.GetType().FullName} must be inherited from {typeof(IBehaviourContainer).FullName}");

            if (!(typeof(IObjectBehavioursBase)).IsAssignableFrom(propReflector.ReflectedPropertyInfo.PropertyType))
                throw new InvalidCastException($"Property type {propReflector.ReflectedPropertyInfo.PropertyType.FullName} is not inherited from {typeof(IObjectBehavioursBase).FullName}");

            return (target as IBehaviourContainer).GetComponent(propReflector.ReflectedPropertyInfo.PropertyType);
        }
    }

}
