using UnityEngine;
using Main.Objects.Behaviours;
using Main.Objects.Behaviours.Attributes;
using Main.Managers;

namespace Main
{
    [RequireComponent(typeof(MapCell))]
    public class Map_Cell_VisualBehaviour: Main.Objects.Behaviours.Tools.SpriteRendererBehaviourWrapper
    {
    }
}