using System;

public interface ICustomFactory
{
    object CreateInstance();
    void DestroyInstance(object instance);
    Type ProducibleType { get; }
}
