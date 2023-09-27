using System.Collections.Generic;
using System;
using System.Reflection;

namespace Main.Other
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class MethodBaseAttribute : Attribute, ICloneable
    {
        public static MethodInfo[] GetMethods<AttributeClass>(Type T, BindingFlags selection_flags) where AttributeClass: MethodBaseAttribute
        {
            List<MethodInfo> methods = new List<MethodInfo>(10);

            foreach (MethodInfo minfo in T.GetMethods(selection_flags))
            {
                if (minfo.GetCustomAttribute(typeof(AttributeClass), true) != null)
                    methods.Add(minfo);
            }

            return methods.ToArray();
        }

        public static int GetMethods<AttributeClass>(Type T, BindingFlags selection_flags, ref MethodInfo[] methods_info) where AttributeClass : MethodBaseAttribute
        {
            int result = 0;

            foreach (MethodInfo minfo in T.GetMethods(selection_flags))
            {
                if (minfo.GetCustomAttribute(typeof(AttributeClass), true) != null)
                {
                    methods_info[result] = minfo;
                    result++;
                }
            }

            return result;
        }

        public static void GetMethods<AttributeClass>(Type T, BindingFlags selection_flags, List<MethodInfo> methods_info) where AttributeClass : MethodBaseAttribute
        {
            methods_info.Clear();

            foreach (MethodInfo minfo in T.GetMethods(BindingFlags.Public))
            {
                if (minfo.GetCustomAttribute(typeof(AttributeClass), true) != null)
                {
                    methods_info.Add(minfo);
                }
            }
        }

        public static void EnumMethods(Type attr_type, Type T, BindingFlags selection_flags, Action<MethodBaseAttribute, MethodInfo> callback) 
        {
            if (callback == null) return;
            MethodBaseAttribute mattr;

            foreach (MethodInfo minfo in T.GetMethods(selection_flags))
            {
                mattr = minfo.GetCustomAttribute(attr_type, true) as MethodBaseAttribute;
                if (mattr != null)
                {
                    callback(mattr, minfo);
                }
            }
        }

        public static void EnumMethods<AttributeClass>(Type T, BindingFlags selection_flags, Action<MethodBaseAttribute, MethodInfo> callback) where AttributeClass : MethodBaseAttribute
        {
            EnumMethods(typeof(AttributeClass), T, selection_flags, callback);
        }

        public static void EnumProperties<AttributeClass>(Type T, BindingFlags selection_flags, Action<MethodBaseAttribute, PropertyInfo> callback) where AttributeClass : MethodBaseAttribute
        {
            if (callback == null) return;
            MethodBaseAttribute mattr;

            foreach (PropertyInfo pinfo in T.GetProperties(selection_flags))
            {
                mattr = pinfo.GetCustomAttribute(typeof(AttributeClass), true) as MethodBaseAttribute;
                if (mattr != null)
                {
                    callback(mattr, pinfo);
                }
            }
        }

        public virtual object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
