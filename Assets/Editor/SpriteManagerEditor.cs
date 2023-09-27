using System.Collections;
using UnityEngine;
using UnityEditor;
using Main.Managers;
using UnityEngine.UIElements;

[CustomEditor(typeof(SpriteManager))]
public class SpriteManagerEditor : Editor
{
    SpriteManager iManager;

    public override VisualElement CreateInspectorGUI()
    {
        iManager = target as SpriteManager;
        return base.CreateInspectorGUI();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }
}