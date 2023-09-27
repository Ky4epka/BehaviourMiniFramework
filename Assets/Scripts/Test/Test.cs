using Main;
using Main.Objects;
using Main.Objects.Behaviours;
using Main.Objects.Behaviours.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct TestS2
{
    public bool Bool;
    public int Int;
    public float Float;
    public string String;
    public Vector3 Vector3;
}

namespace Main.Aggregator.Events.Test.TestProp
{
    public class TestPropProperty : SharedPropertyEvent<TestS2>
    {
    }
}

public class Test : ObjectBehavioursBase
{
}
