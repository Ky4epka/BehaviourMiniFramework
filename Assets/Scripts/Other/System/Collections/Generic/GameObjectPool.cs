using UnityEngine;
using System.Collections;

public class GameObjectPool_Element: IDataPool_Element
{
    public GameObject LinkedObject { get => iLinkedObject; }
    protected GameObject iLinkedObject = null;
    protected DataPool_ElementData fDataPool_ElementData;

    public GameObjectPool_Element(GameObject linkedObject)
    {
        iLinkedObject = linkedObject;
    }

    public void DataPool_Element_SetData(DataPool_ElementData data)
    {
        fDataPool_ElementData = data;
    }

    public DataPool_ElementData DataPool_Element_GetData()
    {
        return fDataPool_ElementData;
    }

    public void Dispose()
    {
        DataPool_Element_GetData().fOwner?.ReturnElement(this);
    }

    public void OnRent()
    {
    }

    public void OnReturn()
    {
    }

    public void DoReturn()
    {
        DataPool_Element_GetData().fOwner.ReturnElement(this);
    }
}

// Based on fast native-array
// O(1) selection
// O(1) take/return object operations
// NOTE: The active elements list may not correspond to adding order. Keep it in mind at selection from the list.
// No thread-safe
[System.Serializable]
public class GameObjectPool: DataPool<GameObjectPool_Element>
{
    [SerializeField]
    protected GameObject fObjectPrototype = null;
    [SerializeField]
    protected Transform fPrototypeParent = null;
    [SerializeField]
    protected bool fObjectActivationState = false;

    public GameObjectPool(IDataPoolElementFabric elementFabric) : base(elementFabric)
    {
    }

    public GameObjectPool(IDataPoolElementFabric elementFabric, int capacity) : base(elementFabric, capacity)
    {
    }

    protected class GameObjectFabric : IDataPoolElementFabric
    {
        protected GameObject iPrototype = null;
        protected Transform iRoot = null;

        public GameObjectFabric(GameObject prototype, Transform root)
        {
            iPrototype = prototype;
            iRoot = root;
        }

        public IDataPool_Element BuildElement()
        {
            GameObjectPool_Element sample = new GameObjectPool_Element(GameObject.Instantiate(iPrototype, iRoot));
            return sample;
        }

        public void DestroyElement(IDataPool_Element element)
        {
            GameObject.Destroy((element as GameObjectPool_Element).LinkedObject);
        }

    }

    

    public GameObject ObjectPrototype
    {
        get
        {
            return fObjectPrototype;
        }

        set
        {
            fObjectPrototype = value;            
        }
    }

    public bool ObjectActivationState
    {
        get
        {
            return fObjectActivationState;
        }

        set
        {
            fObjectActivationState = value;
        }
    }
    
    public Transform ObjectParent
    {
        get
        {
            return fPrototypeParent;
        }

        set
        {
            fPrototypeParent = value;

            for (int i=0; i<fPoolCapacity; i++)
            {
                GameObjectPool_Element o = fObjectPool[i] as GameObjectPool_Element;
                o.LinkedObject.transform.parent = fPrototypeParent;
            }
        }
    }


}


