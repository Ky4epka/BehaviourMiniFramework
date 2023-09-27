using System;

public interface ICustomFactory<T>
{
    T CreateInstance();
    void DestroyInstance(T instance);
    Type ProducibleType { get; }
}
