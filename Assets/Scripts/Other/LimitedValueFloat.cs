using UnityEngine;
using System;
using System.Xml.Serialization;

[Serializable]
[XmlType("ValueContainer")]
public class LimitedValueFloat : ILimitedValue<float>, ILimitedValueExt<float>
{
    [XmlIgnore]
    public float Default_Value = 0;
    [XmlIgnore]
    public float Default_MinValue = -1;
    [XmlIgnore]
    public float Default_MaxValue = 1;

    /// <summary>
    /// Params: float1 - old_value, float2 - new_value
    /// </summary>
    [XmlIgnore]
    public NotifyEvent_2P<float, float> OnChanged { get; } = new NotifyEvent_2P<float, float>();
    [XmlIgnore]
    public NotifyEvent_NP OnMinBorder { get; } = new NotifyEvent_NP();
    [XmlIgnore]
    public NotifyEvent_NP OnMaxBorder { get; } = new NotifyEvent_NP();

    [XmlIgnore]
    [NonSerialized]
    protected float fValue = 0;
    [XmlIgnore]
    [NonSerialized]
    protected float fMinValue = -1;
    [XmlIgnore]
    [NonSerialized]
    protected float fMaxValue = 1;

    public LimitedValueFloat()
    {
        MinValue = Default_MinValue;
        MaxValue = Default_MaxValue;
        Value = Default_Value;
    }

    // Result: Принятое количество
    public virtual float Add(float amount, bool calc_only = false)
    {
        if (amount < 0)
            return 0;

        float value = fValue + amount;
        float result = amount;

        if (value > fMaxValue)
            value = fMaxValue;

        if (MathKit.NumbersEquals(value, fValue))
            return 0;

        float old = fValue;
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
        float value = Add(Percent2Value(percent), calc_only);
        return Value2Percent(value);
    }

    // Result: Принятое количество
    public virtual float Sub(float amount, bool calc_only = false)
    {
        if (amount < 0)
            return 0;

        float value = fValue - amount;
        float result = amount;

        if (value < fMinValue)
            value = fMinValue;

        if (MathKit.NumbersEquals(value, fValue))
            return 0;

        float old = fValue;

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
        float value = Sub(Percent2Value(percent), calc_only);
        return Value2Percent(value);
    }

    public virtual void DoOnChanged(float old_value, float new_value)
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

    public virtual void DoOnMinBorderChanged(float old_value, float _new_value)
    {

    }

    public virtual void DoOnMaxBorderChanged(float old_value, float _new_value)
    {

    }

    [SerializeField]
    [XmlElement("value")]
    public virtual float Value
    {
        get
        {
            return fValue;
        }

        set
        {
            if (MathKit.NumbersEquals(value, fValue))
                return;

            float delta = value - fValue;

            if (MathKit.NumbersEquals(delta, 0))
                return;
            else if (delta > 0)
                Add(delta);
            else
                Sub(-delta);
        }
    }

    [XmlIgnore]
    public virtual float MinValue
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

            float old_value = fMinValue;
            fMinValue = value;

            if (fValue <= fMinValue)
            {
                float old = fValue;
                fValue = fMinValue;
                DoOnChanged(old, fValue);
                DoOnMinBorder();
            }

            DoOnMinBorderChanged(old_value, fMinValue);
        }
    }

    [XmlIgnore]
    public virtual float MaxValue
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

            float old_value = fMaxValue;
            fMaxValue = value;

            if (fValue >= fMaxValue)
            {
                float old = fValue;
                fValue = fMaxValue;
                DoOnChanged(old, fValue);
                DoOnMaxBorder();
            }

            DoOnMaxBorderChanged(old_value, fMaxValue);
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

    public virtual float Length
    {
        get => Math.Abs(MaxValue - MinValue);
    }

    protected float Value2Percent(float value)
    {
        return (value / Length) * 100f;
    }

    protected float Percent2Value(float percent)
    {
        return (percent / 100f) * Length;
    }

}
