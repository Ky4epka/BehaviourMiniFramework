using UnityEngine;
using Main.Objects;
using Main.Objects.Behaviours;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public enum PropertyValueMixingMethod
{
    // Base value modification
    BaseModificator = 0,
    // Input value depend at base
    PivotByBase,
    // Input value depend at current total value (depends at buff position in common query)
    PivotByTotal,
    NoPivot
}

public enum PropertyValueOperation
{
    Assign,
    Addition,
    Subtraction,
    Multiplication,
    Division
}

public enum BuffType
{
    Buff,
    Debuff
}

public interface IBuffPropertyMeta: IAssignable
{
    PropertyValueOperation Operation { get; set; }
    PropertyValueMixingMethod MixingMethod { get; set; }
}

public interface IBuffPropertyData: IBuffPropertyMeta
{
    Type BuffType { get; set; }
    ISharedPropertyStorage propertyStorage { get; set; }
}


public interface IBuff: IBehaviourContainer
{
    string DisplayName { get; set; }
    bool IsPermanent { get; set; }
    BuffType BuffType { get; set; }

    IBuffPropertyMeta BuffPropertyData(Type sharedPropertyType);
    IBuffPropertyMeta BuffPropertyData<T>() where T: ISharedProperty;
}


public class BuffPropertyMeta : IBuffPropertyMeta
{
    public PropertyValueOperation Operation { get; set; }
    public PropertyValueMixingMethod MixingMethod { get; set; }

    public virtual void Assign(IAssignable source)
    {
        if (source is IBuffPropertyMeta)
        {
            Operation = (source as IBuffPropertyMeta).Operation;
            MixingMethod = (source as IBuffPropertyMeta).MixingMethod;
        }
    }
}

public class BuffPropertyData : BuffPropertyMeta, IBuffPropertyData
{
    public ISharedPropertyStorage propertyStorage { get; set; }
    public Type BuffType { get; set; }

}

public class BuffProcessorBehaviour: ObjectBehavioursBase
{
    protected class PropertyLine
    {
        public bool HasChanges { get; protected set; } = false;
        public LinkedList<IBuffPropertyData> PropertyList => iPropertyList;

        protected LinkedList<IBuffPropertyData> iPropertyList = new LinkedList<IBuffPropertyData>();

        public void AddBuffPropertyData(IBuffPropertyData propertyData)
        {
            iPropertyList.AddLast(propertyData);
            HasChanges = true;
        }

        public void DeleteBuffProperty(Type buffType)
        {
            LinkedListNode<IBuffPropertyData> node = iPropertyList.First;

            while (node != null)
            {
                if (node.Value.BuffType.Equals(buffType))
                {
                    iPropertyList.Remove(node);
                    HasChanges = true;
                    return;
                }

                node = node.Next;
            }
        }

        public void Clear()
        {
            iPropertyList.Clear();
            HasChanges = true;
        }

        public void ResetChanges()
        {
            HasChanges = false;
        }
    }

    /// <summary>
    /// Dictionary<
    ///     Type - a type of property,
    ///     List<
    ///         IBuffProperty - a buff property unit
    ///         >
    ///     >
    /// </summary>
    protected Dictionary<Type, PropertyLine> iPropertyMap = new Dictionary<Type, PropertyLine>();
    protected List<IBuff> iAppliedBuffs = new List<IBuff>();

    public void AttachBuff(IBuff buff)
    {
        CopyBuffData(buff);
    }

    public void DettachBuff(IBuff buff)
    {
        Type buffType = buff.GetType();

        foreach (var keyval in iPropertyMap)
        {
            keyval.Value.DeleteBuffProperty(buffType);
        }
    }

    protected void CopyBuffData(IBuff buff)
    {
        PropertyLine _PropertyLine;
        Type buffType = buff.GetType();
        Type propType;

        foreach (var property in buff.PropertyCollection)
        {
            if (property.IsReadOnly)
                continue;

            propType = property.GetType();
            if (!iPropertyMap.TryGetValue(propType, out _PropertyLine))
            {
                _PropertyLine = new PropertyLine();
                iPropertyMap.Add(propType, _PropertyLine);
            }

            IBuffPropertyData buffProperty = new BuffPropertyData();
            buffProperty.propertyStorage = Activator.CreateInstance(property.GetType()) as ISharedPropertyStorage;
            buffProperty.BuffType = buffType;
            buffProperty.propertyStorage.IsStorageMode = true;
            buffProperty.propertyStorage.Value = property.Value;
            buffProperty.Assign(buff.BuffPropertyData(propType));
            _PropertyLine.AddBuffPropertyData(buffProperty);
        }
    }
}