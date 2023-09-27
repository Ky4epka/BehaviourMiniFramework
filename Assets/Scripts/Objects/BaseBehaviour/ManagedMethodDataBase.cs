using System;
using System.Reflection;

namespace Main.Objects.Behaviours.Attributes
{
    public abstract class ManagedMethodDataBase : IManagedMethodData
    {
        public IDelegateActivableAttribute Attribute { get; protected set; }
        public MethodInfo MethodInfo { get; protected set; }
        public bool IsActive { get; protected set; }
        public bool IsPrepared { get; protected set; }
        public Type OwnerType { get; protected set; }
        public IObjectBehavioursBase OwnerInstance { get; protected set; }
        public Type ExtractedEventType { get; protected set; }
        public Delegate MethodDelegate { get; protected set; } = null;
        public bool NotUseInEditMode { get; protected set; }

        public virtual bool Activate()
        {
            if (IsActive || NotUseInEditMode)
                return false;

            IsActive = true;
            OwnerInstance.AddEventListener(ExtractedEventType, MethodDelegate);
            return true;
        }

        public virtual bool Deactivate()
        {
            if (!IsActive || NotUseInEditMode)
                return false;

            IsActive = false;
            OwnerInstance.RemoveEventListener(ExtractedEventType, MethodDelegate);
            return true;
        }

        public virtual void PostActivate()
        {
        }

        public virtual bool Prepare(Type ownerType, IObjectBehavioursBase ownerInstance, IDelegateActivableAttribute attribute, MethodInfo methodInfo, bool _notUseInEditMode)
        {
            if (IsPrepared || NotUseInEditMode)
                return false;

            IsPrepared = true;
            OwnerType = ownerType;
            OwnerInstance = ownerInstance;
            Attribute = attribute;
            MethodInfo = methodInfo;
            ExtractedEventType = methodInfo.GetParameters()[0].ParameterType;
            NotUseInEditMode = _notUseInEditMode && !UnityEngine.Application.isPlaying;

            try
            {
                MethodDelegate = methodInfo.CreateDelegate(
                    System.Linq.Expressions.Expression.GetActionType(ExtractedEventType), OwnerInstance);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log($"Exception: {e.Message}; ownerType: {ownerType.FullName}; ownerInstance: {ownerInstance.GetType().FullName}; attribute: {attribute.GetType().FullName}; methodInfo: {methodInfo}");
            }

            return true;
        }
    }


}