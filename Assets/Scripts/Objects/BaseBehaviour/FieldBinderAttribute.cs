using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Main.Objects;
using System;
using System.Reflection;
using Main.Objects.Behaviours.Attributes;
using Main.Events;

namespace Main.Objects.Behaviours.Attributes
{

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class FieldBinderAttribute : Attribute
    {
        public string PropId;
        protected int iCachedId = Configuration.INVALID_ID;

        public FieldBinderAttribute(string _prop_id)
        {
            PropId = _prop_id;
        }


        public static void EnumThisAttribute(Type targetType, IBehaviourContainer target, Action<Type, IBehaviourContainer, FieldInfo, FieldBinderAttribute> callback)
        {
            foreach (FieldInfo prop in targetType.GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                FieldBinderAttribute attr = prop.GetCustomAttribute<FieldBinderAttribute>(true);

                if (attr != null)
                {
                    callback(targetType, target, prop, attr);
                }
            }
        }
    }
}