using System;
using UnityEngine;

namespace System.Runtime.InteropServices
{
    public interface INameable
    {
        string Name { get; set; }

        string GetName();
        bool SetName(string name);
    }
}
