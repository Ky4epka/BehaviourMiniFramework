using System;
using System.Reflection;

namespace Main.Other.Tools.Attributes
{
    /// <summary>
    /// Use this attribute for class that need a static initialization. 
    /// Static method must be named as constant value '<see cref="InitializableStaticClassAttribute.INIT_METHOD_NAME"/>' and method type <see cref="Action"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class InitializableStaticClassAttribute: Attribute
    {
        public const string INIT_METHOD_NAME = "Init";

        public bool IgnoreMissingInitMethod = true;

        public void DoInitClass(Type classType)
        {
            if (classType == null)
                throw new System.ArgumentNullException(nameof(classType));

            MethodInfo mi = classType.GetMethod(INIT_METHOD_NAME, BindingFlags.Static);

            if (mi != null)
                mi.Invoke(null, null);
            else if (!IgnoreMissingInitMethod)
                throw new System.MethodAccessException($"Static method named as '{INIT_METHOD_NAME}' not found in class {classType.FullName}");
        }

        public static void InvokeInitialization(Func<Type, Exception, bool> exceptionHandler = null)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    try
                    {
                        type.GetCustomAttribute<InitializableStaticClassAttribute>(true)?.DoInitClass(type);
                    }
                    catch (Exception e)
                    {
                        if (exceptionHandler != null)
                            exceptionHandler(type, e);
                        else
                            throw e;
                    }
                }
            }
        }
    }
}
