using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace System.Reflection
{
    public class ReflectionAccelerator : ICustomAttributeProvider, IReflect
    {
        public Type UnderlyingSystemType => AccelerateReflection;

        protected Dictionary<Type, List<Attribute>> iClassAttributes = new Dictionary<Type, List<Attribute>>();
        protected Dictionary<string, List<PropertyInfo>> iProperties = new Dictionary<string, List<PropertyInfo>>();
        protected Dictionary<string, List<MethodInfo>> iMethods = new Dictionary<string, List<MethodInfo>>();
        protected Dictionary<string, List<FieldInfo>> iFields = new Dictionary<string, List<FieldInfo>>();

        protected Dictionary<PropertyInfo, Dictionary<Type, Attribute>> iPropertyAttributes = new Dictionary<PropertyInfo, Dictionary<Type, Attribute>>();
        protected Dictionary<PropertyInfo, Dictionary<Type, Attribute>> iMethodAttributes = new Dictionary<PropertyInfo, Dictionary<Type, Attribute>>();
        protected Dictionary<PropertyInfo, Dictionary<Type, Attribute>> iFieldsAttributes = new Dictionary<PropertyInfo, Dictionary<Type, Attribute>>();

        protected Type iAcceleratedType = null;

        public ReflectionAccelerator()
        {

        }

        public Type AccelerateReflection
        {
            get => iAcceleratedType;
            set
            {
                if (iAcceleratedType == value)
                    return;

                ClearCache();
                iAcceleratedType = value;
            }

        }

        public void ClearCache()
        {
        }

        protected void CacheAttributes()
        {

        }

        protected void CacheProperties()
        {

        }

        protected void CacheMethods()
        {

        }

        protected void CacheFields()
        {

        }

        public object[] GetCustomAttributes(bool inherit)
        {
            return null;
        }

        public object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            List<Attribute> result;

            if (iClassAttributes.TryGetValue(attributeType, out result))
            {
                return result.ToArray();
            }

            return new object[0];
        }

        public FieldInfo GetField(string name, BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        public FieldInfo[] GetFields(BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        public MemberInfo[] GetMember(string name, BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        public MemberInfo[] GetMembers(BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        public MethodInfo GetMethod(string name, BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        public MethodInfo GetMethod(string name, BindingFlags bindingAttr, Binder binder, Type[] types, ParameterModifier[] modifiers)
        {
            throw new NotImplementedException();
        }

        public MethodInfo[] GetMethods(BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        public PropertyInfo[] GetProperties(BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        public PropertyInfo GetProperty(string name, BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        public PropertyInfo GetProperty(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
        {
            throw new NotImplementedException();
        }

        public object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
        {
            throw new NotImplementedException();
        }

        public bool IsDefined(Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }
    }
}
