using UnityEngine;
using System.Collections;

public struct DataPool_ElementData
{
    internal IDataPool fOwner;
    internal int fPoolIndex;
}

public interface IDataPool_Element: System.IDisposable
{
    void DataPool_Element_SetData(DataPool_ElementData data);
    DataPool_ElementData DataPool_Element_GetData();

    void OnRent();
    void OnReturn();

    void DoReturn();
}


public interface IDataPool
{
    System.Type ElementType { get; }
    void SetCapacity(int value);
    int GetCapacity();

    int GetUsedCount();
    int GetUnusedCount();

    void SetUseAutogrow(bool value);
    bool GetUseAutogrow();

    void SetGrowQuota(int value);
    int GetGrowQuota();

    void ReturnAllElements();
    void ClearPool();

    IDataPool_Element ElementAt(int index);

    IDataPool_Element RentElement();
    bool ReturnElement(IDataPool_Element element);
}

public interface IDataPoolElementFabric
{
    IDataPool_Element BuildElement();
    void DestroyElement(IDataPool_Element element);
}

public interface IDataPoolElementFabric<T> where T: IDataPool_Element
{
    T BuildElement();
    void DestroyElement(T element);
}

// Based on fast native-array
// O(1) selection
// O(1) take/return object operations
// NOTE: The active elements list may not correspond to adding order. Keep it in mind at selection from the list.
// No thread-safe
[System.Serializable]
public abstract class DataPool<T> : IDataPool where T: IDataPool_Element
{
    public System.Type ElementType { get; } = typeof(T);
    [SerializeField]
    protected IDataPool_Element[] fObjectPool = null;
    [SerializeField]
    protected int fPoolUsedCount = 0;
    [SerializeField]
    protected int fPoolCapacity = 0;

    [SerializeField]
    protected bool fUseAutoGrow = false;
    [SerializeField]
    protected int fGrowQuota = 1;

    protected IDataPoolElementFabric iElementFabric = null;
    
    public void SetCapacity(int value)
    {
        if (fPoolCapacity == value)
            return;

        

        // Если новый размер меньше старого, то удаляем лишние элементы
        for (int i = value; i < fPoolCapacity; i++)
        {
            iElementFabric.DestroyElement(fObjectPool[i]);
            fObjectPool[i] = null;
        }

        if (value > 0)
        {
            IDataPool_Element[] PoolCache = new IDataPool_Element[value];

            // Копирование данных старого массива
            for (int i = Mathf.Min(value, fPoolCapacity) - 1; i >= 0; i--)
            {
                PoolCache[i] = fObjectPool[i];
                fObjectPool[i] = null;
            }

            fObjectPool = PoolCache;
            PoolCache = null;

            // Если новый размер больше старого, то добавляем недостающие элементы
            for (int i = fPoolCapacity; i < value; i++)
            {
                fObjectPool[i] = iElementFabric.BuildElement();
            }
        }
        else
            value = 0;

        fPoolCapacity = value;

        if (fPoolUsedCount > fPoolCapacity)
            fPoolUsedCount = fPoolCapacity;
    }

    public int GetCapacity()
    {
        return fPoolCapacity;
    }

    public int GetUsedCount()
    {
        return fPoolUsedCount;
    }

    public int GetUnusedCount()
    {
        return fPoolCapacity - fPoolUsedCount;
    }

    public void SetUseAutogrow(bool value)
    {
        fUseAutoGrow = value;
    }

    public bool GetUseAutogrow()
    {
        return fUseAutoGrow;
    }

    public void SetGrowQuota(int value)
    {
        fGrowQuota = value;
    }

    public int GetGrowQuota()
    {
        return fGrowQuota;
    }

    public IDataPool_Element ElementAt(int index)
    {
        if ((index >= 0) &&
            (index < fPoolUsedCount))
            return fObjectPool[index];
        else
            Debug.LogError(string.Concat("The index ('", index, "') out of range (0, ", fPoolUsedCount, ")"));

        return null;
    }

    public IDataPool_Element RentElement()
    {
        IDataPool_Element element;
        
        if (fPoolUsedCount >= fPoolCapacity)
        {

            if (fUseAutoGrow && fGrowQuota > 0)
            {
                PoolCapacity += fGrowQuota;
            }
            else
            {
                Debug.LogError("Can't take a object from object-pool. Reason: The pool has no free elements");
                return null;
            }
        }

        DataPool_ElementData data;
        data.fOwner = this;
        data.fPoolIndex = fPoolUsedCount;

        element = fObjectPool[fPoolUsedCount];
        element.DataPool_Element_SetData(data);
        element.OnRent();
        fPoolUsedCount++;

        return element;
    }

    public bool ReturnElement(IDataPool_Element element)
    {
        DataPool_ElementData data = element.DataPool_Element_GetData();

        if (element == null)
        {
            Debug.LogError("Can't return object. Reason: object is null");
            return false;
        }
        else if (data.fOwner != this)
        {
            Debug.LogError(string.Concat("Can't return object '", element, "' to this pool ('", this, "'). Reason: The pool has no ownership this object. "));
            return false;
        }

        if (element.DataPool_Element_GetData().fPoolIndex == -1)
            return false;

        fPoolUsedCount--;
        fObjectPool[data.fPoolIndex] = fObjectPool[fPoolUsedCount];
        fObjectPool[data.fPoolIndex].DataPool_Element_SetData(data);
        fObjectPool[fPoolUsedCount] = element;
        data.fPoolIndex = -1;
        element.DataPool_Element_SetData(data);
        element.OnReturn();
        return true;
    }

    public void ReturnAllElements()
    {
        foreach (IDataPool_Element element in fObjectPool)
            if (element.DataPool_Element_GetData().fPoolIndex != -1)
                ReturnElement(element);

    }

    public void ClearPool()
    {
        SetCapacity(0);
    }

    public DataPool(IDataPoolElementFabric elementFabric)
    {
        if (elementFabric == null)
            throw new System.ArgumentNullException("elementFabric");

        iElementFabric = elementFabric;
    }
        
    public DataPool(IDataPoolElementFabric elementFabric, int capacity)
    {
        if (elementFabric == null)
            throw new System.ArgumentNullException("elementFabric");

        iElementFabric = elementFabric;

        PoolCapacity = capacity;
    }

    public int PoolCapacity
    {
        get
        {
            return GetCapacity();
        }

        set
        {
            SetCapacity(value);
        }
    }

    public int UsedCount
    {
        get
        {
            return GetUsedCount();
        }
    }

    public bool UseAutoGrow
    {
        get
        {
            return GetUseAutogrow();
        }

        set
        {
            SetUseAutogrow(value);
        }
    }

    public int GrowQuota
    {
        get
        {
            return GetGrowQuota();
        }

        set
        {
            SetGrowQuota(value);
        }
    }

    public IDataPool_Element this[int index]
    {
        get
        {
            return ElementAt(index);
        }
    }
    
}
