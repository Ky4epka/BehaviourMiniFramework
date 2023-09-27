//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace System.Collections
//{
//    public class ElementsTypePoolMap
//    {
//        protected class PoolInfo
//        {
//            public Type ElementType = null;
//            public IDataPool PoolInstance = null;
//            public IPoolFactory PoolFactory = null;

//            public PoolInfo(Type elementType, IDataPool poolInstance, IPoolFactory poolFactory)
//            {
//                ElementType = elementType;
//                PoolInstance = poolInstance;
//                PoolFactory = poolFactory;
//            }
//        }

//        protected Dictionary<Type, PoolInfo> iPools = new Dictionary<Type, PoolInfo>();

//        public void RegisterType(Type elementType, IPoolFactory poolFactory)
//        {
//            if (poolFactory == null)
//                throw new ArgumentNullException("poolFactory");

//            if (!elementType.IsAssignableFrom(typeof(IDataPool_Element)))
//                throw new TypeAccessException("Registered type must be inherited from a IDataPool_Element");

//            try
//            {
//                iPools.Add(elementType, new PoolInfo(elementType, null, poolFactory));
//            }
//            catch (ArgumentException)
//            {
//                throw new Exception(string.Concat("The pool of type '", elementType.FullName, "' already registered in this map"));
//            }
//        }

//        public void RegisterType<T>(IPoolFactory poolFactory) where T: IDataPool_Element
//        {
//            RegisterType(typeof(T), poolFactory);
//        }

//        public bool UnregisterType(Type elementType)
//        {
//            return iPools.Remove(elementType);
//        }

//        public virtual IDataPool Pool(Type elementType)
//        {
//            PoolInfo pinfo;
//            if (iPools.TryGetValue(elementType, out pinfo))
//            {
//                if (pinfo.PoolInstance == null)
//                {
//                    IDataPool pool = pinfo.PoolFactory.CreateInstance();
//                    if (!pinfo.PoolInstance.ElementType.Equals(elementType))
//                        throw new Exception(string.Concat(
//                            "Pool element type '",
//                            pinfo.PoolInstance.ElementType.FullName,
//                            "' not correspond to registered element type '",
//                            elementType.FullName,
//                            "'"));

//                    pinfo.PoolInstance = pool;
//                }

//                return pinfo.PoolInstance;
//            }

//            return null;
//        }

//        public virtual IDataPool Pool<T>() where T : IDataPool_Element
//        {
//            return Pool(typeof(T));
//        }

//        public virtual bool IsTypeRegistered(Type ElementType)
//        {
//            return Pool(ElementType) != null;
//        }

//        public virtual bool IsTypeRegistered<T>() where T : IDataPool_Element
//        {
//            return IsTypeRegistered(typeof(T));
//        }
//    }
//}

//namespace Main.Events
//{
//    using System;

//    public class EventDataPoolFactory<EventType> : IPoolFactory where EventType : IEventData, new()
//    {
//        public Type ProducibleType => typeof(EventType);

//        public IDataPool CreateInstance()
//        {
//            return null;//new EventDataPool<EventType>();
//        }

//        public void DestroyInstance(IDataPool instance)
//        {
//            instance.ClearPool();
//        }
//    }

//}
