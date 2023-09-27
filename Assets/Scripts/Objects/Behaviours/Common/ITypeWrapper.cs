using System;

namespace Main.Other
{
    public interface ITypeWrapper: UnityEngine.ISerializationCallbackReceiver
    {
        Type BaseType { get; }
        Type SelectedType { get; set; }
    }
}
