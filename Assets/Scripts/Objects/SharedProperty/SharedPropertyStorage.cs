using System;
using System.Runtime.InteropServices;

namespace Main.Objects
{
    /// <summary>
    /// This class provides only the full implementation of sharedProperty, but not could using in direct. 
    /// Shared property type must be unique, because his type used for self-identification in container
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="commandType"></typeparam>
    /// <typeparam name="eventType"></typeparam>
    [Serializable]
	public class SharedPropertyStorage<T> : ISharedPropertyStorage<T>
	{
		public virtual string GroupTag { get; }
		public virtual string SharedName { get; }
		public virtual T Value { get => iValue; set => iValue = value; }

        [UnityEngine.SerializeField]
        [UnityEngine.SerializeReference]
        protected T iValue = default;

        public Type ValueType => typeof(T);


        public SharedPropertyStorage()
        {

        }

        public SharedPropertyStorage(T value)
        {
            Value = value;
        }

        public bool HasMathematicOp
        {
            get
            {
                if (iHasMathematicOpInitFlag)
                {
                    iHasMathematicOp = typeof(ISharedPropertyStorageMathematic<T>).IsAssignableFrom(GetType());
                    iHasMathematicOpInitFlag = false;
                }

                return iHasMathematicOp;
            }
        }

        protected static bool iHasMathematicOpInitFlag = true;
        protected static bool iHasMathematicOp = false;

        object ISharedPropertyStorage.Value
		{ 
			get => (T)Value;
			// Condition with value != null to allow to avoid @NullReferenceException when value a non reference type takes abstract value (object) takes a "null" 
			set => Value = (value != null) ? (T)value : default(T);
		}

        public virtual bool IsStorageMode
        { 
            get => true;
            set => throw new Exception("Could not set storage mode for storage property.");
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public void Assign(IAssignable source)
        {
            if (source == null)
                return;

            if (!GetType().IsAssignableFrom(typeof(ISharedPropertyStorage<T>)))
                return;

            ISharedPropertyStorage<T> sourceStorage = source as ISharedPropertyStorage<T>;
            Value = sourceStorage.Value;
        }
    }
}