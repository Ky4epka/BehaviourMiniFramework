using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IConfigurable 
{
    void DropConfig(System.IO.Stream stream);
    void PushConfig(System.IO.Stream stream);
}
