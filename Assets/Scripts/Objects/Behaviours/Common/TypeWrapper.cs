using UnityEngine;
using System;
using System.Linq;
using System.Reflection;

namespace Main.Other
{

    [Serializable]
    public abstract class TypeWrapper: ITypeWrapper
    {
        public abstract Type BaseType { get; }
        public Type SelectedType { get; set; }

        [SerializeField]
        protected string iBaseTypeClassName;
        [SerializeField]
        protected string iSelectedTypeClassName;

        public TypeWrapper()
        {
            OnBeforeSerialize();
        }

        public void OnBeforeSerialize()
        {
            iSelectedTypeClassName = SelectedType?.AssemblyQualifiedName ?? "";
            iBaseTypeClassName = BaseType?.AssemblyQualifiedName ?? "";
        }

        public void OnAfterDeserialize()
        {
            SelectedType = Type.GetType(iSelectedTypeClassName);
        }

        public static Type[] SelectInheritanceClasses(Type baseClass)
        {
            return (
                from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                    // alternative: from domainAssembly in domainAssembly.GetExportedTypes()
                from type in domainAssembly.GetTypes()
                where baseClass.IsAssignableFrom(type) && !type.IsAbstract
                // alternative: && type != typeof(B)
                // alternative: && ! type.IsAbstract
                // alternative: where type.IsSubclassOf(typeof(B))
                select type).ToArray();
        }

        public override bool Equals(object obj)
        {
            TypeWrapper o = obj as TypeWrapper;
            return (o != null) && (o.BaseType?.Equals(BaseType) ?? false) && (o.SelectedType?.Equals(SelectedType) ?? false);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
