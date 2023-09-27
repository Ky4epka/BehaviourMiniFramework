using UnityEngine;
using System;
using System.Xml.Serialization;

[Serializable]
[XmlType("ValueContainer")]
public class LimitedValueInt : ILimitedValue<int>, ILimitedValueExt<int>
{
    [XmlIgnore]
    public int Default_Value = 0;
    [XmlIgnore]
    public int Default_MinValue = -1;
    [XmlIgnore]
    public int Default_MaxValue = 1;

    /// <summary>
    /// Params: float1 - old_value, float2 - new_value
    /// </summary>
    [XmlIgnore]
    public NotifyEvent_2P<int, int> OnChanged { get; } = new NotifyEvent_2P<int, int>();
    [XmlIgnore]
    public NotifyEvent_NP OnMinBorder { get; } = new NotifyEvent_NP();
    [XmlIgnore]
    public NotifyEvent_NP OnMaxBorder { get; } = new NotifyEvent_NP();

    [XmlIgnore]
    [NonSerialized]
    protected int fValue = 0;
    [XmlIgnore]
    [NonSerialized]
    protected int fMinValue = -1;
    [XmlIgnore]
    [NonSerialized]
    protected int fMaxValue = 1;

    public LimitedValueInt()
    {
        MinValue = Default_MinValue;
        MaxValue = Default_MaxValue;
        Value = Default_Value;
    }

    // Result: Принятое количество
    public virtual int Add(int amount, bool calc_only = false)
    {
        if (amount < 0)
            return 0;

        int value = fValue + amount;
        int result = amount;

        if (value > fMaxValue)
            value = fMaxValue;

        if (MathKit.NumbersEquals(value, fValue))
            return 0;

        int old = fValue;
        if (!calc_only)
        {
            fValue = value;
            DoOnChanged(old, value);
        }

        if (MathKit.NumbersEquals(value, fMaxValue))
        {
            result -= (old + amount) - fMaxValue;

            if (!calc_only)
                DoOnMaxBorder();
        }

        return result;
    }

    public virtual float AddPercent(float percent, bool calc_only = false)
    {
        int value = Add(Percent2Value(percent), calc_only);
        return Value2Percent(value);
    }

    // Result: Принятое количество
    public virtual int Sub(int amount, bool calc_only = false)
    {
        if (amount < 0)
            return 0;

        int value = fValue - amount;
        int result = amount;

        if (value < fMinValue)
            value = fMinValue;

        if (MathKit.NumbersEquals(value, fValue))
            return 0;

        int old = fValue;

        if (!calc_only)
        {
            fValue = value;
            DoOnChanged(old, value);
        }

        if (MathKit.NumbersEquals(value, fMinValue))
        {
            result += old;

            if (!calc_only)
                DoOnMinBorder();
        }

        return result;
    }
    public virtual float SubPercent(float percent, bool calc_only = false)
    {
        int value = Sub(Percent2Value(percent), calc_only);
        return Value2Percent(value);
    }

    public virtual void DoOnChanged(int old_value, int new_value)
    {
        OnChanged.Invoke(old_value, new_value);
    }

    public virtual void DoOnMinBorder()
    {
        OnMinBorder.Invoke();
    }

    public virtual void DoOnMaxBorder()
    {
        OnMaxBorder.Invoke();
    }

    [SerializeField]
    [XmlElement("value")]
    public virtual int Value
    {
        get
        {
            return fValue;
        }

        set
        {
            if (MathKit.NumbersEquals(value, fValue))
                return;

            int delta = value - fValue;

            if (MathKit.NumbersEquals(delta, 0))
                return;
            else if (delta > 0)
                Add(delta);
            else
                Sub(-delta);
        }
    }

    [XmlIgnore]
    public virtual int MinValue
    {
        get
        {
            return fMinValue;
        }

        set
        {
            if ((MathKit.NumbersEquals(value, fMinValue)) ||
                (fMinValue >= fMaxValue))
                return;

            fMinValue = value;

            if (fValue <= fMinValue)
            {
                int old = fValue;
                fValue = fMinValue;
                DoOnChanged(old, fValue);
                DoOnMinBorder();
            }
        }
    }

    [XmlIgnore]
    public virtual int MaxValue
    {
        get
        {
            return fMaxValue;
        }

        set
        {
            if ((MathKit.NumbersEquals(value, fMaxValue)) ||
                (fMaxValue <= fMinValue))
                return;

            fMaxValue = value;

            if (fValue >= fMaxValue)
            {
                int old = fValue;
                fValue = fMaxValue;
                DoOnChanged(old, fValue);
                DoOnMaxBorder();
            }
        }
    }

    public virtual float ValuePercent
    {
        get => Value2Percent(Value);
        set
        {
            Value = Percent2Value(value) + MinValue;
        }
    }

    public virtual int Length
    {
        get => Math.Abs(MaxValue - MinValue);
    }

    protected float Value2Percent(int value)
    {
        return (value / Length) * 100f;
    }

    protected int Percent2Value(float percent)
    {
        return (int)((percent / 100f) * Length);
    }

}
