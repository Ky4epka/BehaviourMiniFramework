using System;

namespace Main.Objects.Behaviours.Attributes
{
    using Main.Other;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class SharedPropertyAttribute : Attribute, IPropertyInjectorAttribute
    {
        public Type InjectComponentToValue = null;
        /// <summary>
        /// Using Activator.CreateInstance(type) for default value of property
        /// </summary>
        public bool DefaultConstructorIfDefaultReferenceValue = false;

        protected Type iCertainPropType = null;

        public SharedPropertyAttribute()
        {
        }

        public SharedPropertyAttribute(Type certainPropType)
        {
            iCertainPropType = certainPropType;
        }

        public object Inject(object target, TypeReflector.PropertyReflector propReflector)
        {
            if (!(typeof(ISharedPropertiesContainer)).IsAssignableFrom(target.GetType()))
                throw new InvalidCastException($"Target type {target.GetType().FullName} must be inherited from {typeof(ISharedPropertiesContainer).FullName}");

            Type propType = iCertainPropType ?? propReflector.ReflectedPropertyInfo.PropertyType;

            if (!(typeof(ISharedProperty)).IsAssignableFrom(propType))
                throw new InvalidCastException($"Property type {propType.FullName} is not inherited from {typeof(ISharedProperty).FullName}");

            ISharedProperty result = (target as ISharedPropertiesContainer).SharedProperty(propType);

            if (result == null)
                throw new Exception("Couldnot provide shared property of type " + propType.FullName);

            if (InjectComponentToValue != null)
            {
                if (!(typeof(UnityEngine.Component)).IsAssignableFrom(InjectComponentToValue))
                    throw new InvalidCastException($"Injected component type {InjectComponentToValue} is not inherited from {typeof(UnityEngine.Component).FullName}");

                result.Value = ((UnityEngine.Component)target).GetComponent(InjectComponentToValue);
            }
            else if ((DefaultConstructorIfDefaultReferenceValue) && 
                     (!result.ValueType.IsValueType) &&
                     (result.Value == null))
                result.Value = Activator.CreateInstance(result.ValueType);

            return result;
        }
    }

}