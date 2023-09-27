using System.Collections;
using UnityEngine;

namespace Main
{
    public interface IBehaviour
    {
        bool enabled { get; set; }
        bool isActiveAndEnabled { get; }
    }
}