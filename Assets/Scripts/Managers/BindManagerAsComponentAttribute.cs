using System;
using System.Reflection;

namespace Main.Managers.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class BindManagerAsComponentAttribute : Attribute
    {
        public Type ManagerType { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="managerType">Manager will must inherits from IObjectManager</param>
        public BindManagerAsComponentAttribute(Type managerType)
        {
            if (!managerType.IsAssignableFrom(typeof(IObjectManager)))
                throw new InvalidCastException();

            ManagerType = managerType;
        }

        public void InjectSlaveManagerProperty(ObjectManagerMonoBehaviourWrapper topManager, PropertyInfo slaveManagerProperty)
        {
            IObjectManager slaveManager = ((IObjectManager)slaveManagerProperty.GetValue(this));

            if (slaveManager == null)
            {
                slaveManager = topManager.GetComponent(ManagerType) as IObjectManager;

                if (slaveManager == null)
                    throw new NullReferenceException(
                        $"Injecting component as manager type '{nameof(ManagerType)}' not found in MonoBehaviour {nameof(topManager)}");

                slaveManagerProperty.SetValue(topManager, slaveManager);
            }

            slaveManager.MasterManager = topManager;
        }
    }
}
