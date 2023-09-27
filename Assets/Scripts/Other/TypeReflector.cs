using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Main.Other
{

    public interface IPropertyInjectorAttribute : _Attribute
    {
        object Inject(object target, TypeReflector.PropertyReflector propReflector);
    }

    public class TypeReflector
    {
        public class FieldReflector
        {
            public FieldAttributeReference AttributeReference { get; private set; } = null;
            public FieldInfo ReflectedFieldInfo { get; private set; } = null;

            public FieldReflector(FieldInfo FieldInfo)
            {
                ReflectedFieldInfo = FieldInfo;
                AttributeReference = new FieldAttributeReference(FieldInfo);
            }
        }

        public class PropertyReflector
        {
            public PropertyAttributeReference AttributeReference { get; private set; } = null;
            public PropertyInfo ReflectedPropertyInfo { get; private set; } = null;

            public PropertyReflector(PropertyInfo propertyInfo)
            {
                ReflectedPropertyInfo = propertyInfo;
                AttributeReference = new PropertyAttributeReference(propertyInfo);
            }
        }

        public class MethodReflector
        {
            public MethodAttributeReference AttributeReference { get; private set; } = null;
            public MethodInfo ReflectedMethodInfo { get; private set; } = null;

            public MethodReflector(MethodInfo methodInfo)
            {
                ReflectedMethodInfo = methodInfo;
                AttributeReference = new MethodAttributeReference(methodInfo);
            }
        }

        public abstract class AttributeReferenceBase
        {
            protected Dictionary<AttributeKey, List<Attribute>> iAttributes =
                new Dictionary<AttributeKey, List<Attribute>>();

            public IEnumerable<Attribute> GetCustomAttributes(Type attributeType, bool inherit)
            {
                return ProvideCustomAttributesList(attributeType, inherit);
            }

            public IEnumerable<Attribute> GetCustomAttributes(bool inherit)
            {
                return ProvideCustomAttributesList(typeof(Attribute), inherit);
            }

            public Attribute GetCustomAttribute(Type attributeType, bool inherit)
            {
                IList<Attribute> col = ProvideCustomAttributesList(attributeType, inherit);
                return (col.Count > 0) ? col[0] : null;
            }

            protected virtual List<Attribute> ProvideCustomAttributesList(Type attributeType, bool inherit)
            {
                List<Attribute> result;
                AttributeKey key = new AttributeKey(attributeType, inherit);

                if (!iAttributes.TryGetValue(key, out result))
                {
                    IEnumerable<Attribute> e = GetAttributeList(attributeType, inherit);

                    if (e != null)
                        result = new List<Attribute>(e);
                    else
                        result = new List<Attribute>();
                    iAttributes.Add(key, result);
                }

                return result;
            }

            protected abstract IEnumerable<Attribute> GetAttributeList(Type attributeType, bool inherit);
        }


        public class TypeAttributeReference : AttributeReferenceBase
        {
            public Type ReflectedType { get; protected set; }

            public TypeAttributeReference(Type reflectedType) : base()
            {
                ReflectedType = reflectedType;
            }

            protected override IEnumerable<Attribute> GetAttributeList(Type attributeType, bool inherit)
            {
                return ReflectedType.GetCustomAttributes(attributeType, inherit) as IEnumerable<Attribute>;
            }
        }

        public class FieldAttributeReference : AttributeReferenceBase
        {
            public FieldInfo ReflectedField { get; private set; } = null;

            public FieldAttributeReference(FieldInfo reflectedField) : base()
            {
                ReflectedField = reflectedField;
            }

            protected override IEnumerable<Attribute> GetAttributeList(Type attributeType, bool inherit)
            {
                return ReflectedField.GetCustomAttributes(attributeType, inherit) as IEnumerable<Attribute>;
            }
        }

        public class MethodAttributeReference : AttributeReferenceBase
        {
            public MethodInfo ReflectedMethod { get; private set; } = null;

            public MethodAttributeReference(MethodInfo reflectedField) : base()
            {
                ReflectedMethod = reflectedField;
            }

            protected override IEnumerable<Attribute> GetAttributeList(Type attributeType, bool inherit)
            {
                return ReflectedMethod.GetCustomAttributes(attributeType, inherit) as IEnumerable<Attribute>;
            }
        }

        public class PropertyAttributeReference : AttributeReferenceBase
        {
            public PropertyInfo ReflectedProperty { get; private set; } = null;

            public PropertyAttributeReference(PropertyInfo reflectedField) : base()
            {
                ReflectedProperty = reflectedField;
            }

            protected override IEnumerable<Attribute> GetAttributeList(Type attributeType, bool inherit)
            {
                return ReflectedProperty.GetCustomAttributes(attributeType, inherit) as IEnumerable<Attribute>;
            }
        }

        public class AttributeKey
        {
            public Type AttributeType => iAttributeType;
            public bool Inherited => iInherited;

            protected Type iAttributeType = null;
            protected bool iInherited = false;

            public AttributeKey(Type attributeType, bool inherited)
            {
                if (attributeType == null)
                    throw new ArgumentNullException("attributeType");

                iAttributeType = attributeType;
                iInherited = inherited;
            }

            public override bool Equals(object obj)
            {
                AttributeKey cmpWith = obj as AttributeKey;

                return
                    cmpWith != null &&
                    AttributeType.Equals(cmpWith.AttributeType) &&
                    Inherited.Equals(cmpWith.Inherited);
            }

            public override int GetHashCode()
            {
                return AttributeType.GetHashCode() ^ ~(Inherited ? 1 : 0);
            }
        }

        public Type ReflectedType { get; private set; } = null;

        public TypeAttributeReference AttributeReference { get; private set; }
        protected Dictionary<BindingFlags, List<FieldReflector>> iFields = new Dictionary<BindingFlags, List<FieldReflector>>();
        protected Dictionary<BindingFlags, List<PropertyReflector>> iProperties = new Dictionary<BindingFlags, List<PropertyReflector>>();
        protected Dictionary<BindingFlags, List<MethodReflector>> iMethods = new Dictionary<BindingFlags, List<MethodReflector>>();


        public TypeReflector(Type reflectedType)
        {
            ReflectedType = reflectedType;
            AttributeReference = new TypeAttributeReference(reflectedType);
        }

        public Attribute GetCustomAttribute(Type attributeType, bool inherit)
        {
            return AttributeReference.GetCustomAttribute(attributeType, inherit);
        }

        public T GetCustomAttribute<T>(bool inherit) where T : Attribute => (T)GetCustomAttribute(typeof(T), inherit);

        public IEnumerable<Attribute> GetCustomAttributes(Type attributeType, bool inherit) =>
            AttributeReference.GetCustomAttributes(attributeType, inherit);

        public IEnumerable<Attribute> GetCustomAttributes(bool inherited)
        {
            return AttributeReference.GetCustomAttributes(inherited);
        }

        public IEnumerable<FieldReflector> GetFields(BindingFlags bindingAttr)
        {
            List<FieldReflector> result;

            if (!iFields.TryGetValue(bindingAttr, out result))
            {
                result = new List<FieldReflector>();

                foreach (FieldInfo fieldInfo in ReflectedType.GetFields(bindingAttr))
                {
                    result.Add(new FieldReflector(fieldInfo));
                }

                iFields.Add(bindingAttr, result);
            }

            return result;
        }

        public IEnumerable<MethodReflector> GetMethods(BindingFlags bindingAttr)
        {
            List<MethodReflector> result;

            if (!iMethods.TryGetValue(bindingAttr, out result))
            {
                result = new List<MethodReflector>();

                foreach (MethodInfo methodInfo in ReflectedType.GetMethods(bindingAttr))
                {
                    result.Add(new MethodReflector(methodInfo));
                }

                iMethods.Add(bindingAttr, result);
            }

            return result;
        }

        public IEnumerable<PropertyReflector> GetProperties(BindingFlags bindingAttr)
        {
            List<PropertyReflector> result;

            if (!iProperties.TryGetValue(bindingAttr, out result))
            {
                result = new List<PropertyReflector>();

                foreach (PropertyInfo propertyInfo in ReflectedType.GetProperties(bindingAttr))
                {
                    result.Add(new PropertyReflector(propertyInfo));
                }

                iProperties.Add(bindingAttr, result);
            }

            return result;
        }


        public void InjectProperties(object target, BindingFlags bindFlags, bool inherit)
        {
            foreach (PropertyReflector pr in GetProperties(bindFlags))
            {
                foreach (Attribute attr in pr.AttributeReference.GetCustomAttributes(inherit))
                {
                    IPropertyInjectorAttribute injector = attr as IPropertyInjectorAttribute;

                    if (injector == null)
                        continue;

                    if (pr.ReflectedPropertyInfo.CanRead)
                    {
                        pr.ReflectedPropertyInfo.SetValue(target, injector.Inject(target, pr));
                    }
                    else
                        throw new MemberAccessException(
                            string.Concat(
                                "Could not inject '",
                                nameof(pr.ReflectedPropertyInfo),
                                "' property for a reason: readonly property"));
                }
            }
        }


    }
}