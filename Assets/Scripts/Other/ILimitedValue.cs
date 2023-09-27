public interface ILimitedValue<T>
{
    /// <summary>
    /// Param1: Old value, Param2: New value
    /// </summary>
    NotifyEvent_2P<T, T> OnChanged { get; }
    NotifyEvent_NP OnMinBorder { get; }
    NotifyEvent_NP OnMaxBorder { get; }


    /// <summary>
    /// Adds amount of units
    /// </summary>
    /// <param name="amount">Desired amount</param>
    /// <returns>Actual value change</returns>
    T Add(T amount, bool calc_only = false);
    float AddPercent(float percent, bool calc_only = false);
    /// <summary>
    /// Substracts amount of units
    /// </summary>
    /// <param name="amount">Desired amount</param>
    /// <returns>Actual value change</returns>
    T Sub(T amount, bool calc_only = false);
    float SubPercent(float percent, bool calc_only = false);
    T Value { get; set; }
    T MinValue { get; set; }
    T MaxValue { get; set; }
    float ValuePercent { get; set; }
    T Length { get; }
}

public interface ILimitedValueExt<T>
{
    void DoOnChanged(T old_value, T new_value);
    void DoOnMinBorder();
    void DoOnMaxBorder();
}