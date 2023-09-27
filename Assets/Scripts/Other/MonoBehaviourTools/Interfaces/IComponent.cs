
using System;
using System.Collections.Generic;
//using System.ComponentModel;
using System.Security;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting;
using UnityEngineInternal;
using UnityEngine;

public interface IComponent
{
    Component animation { get; }
    Component audio { get; }
    Component camera { get; }
    Component collider { get; }
    Component collider2D { get; }
    Component constantForce { get; }
    GameObject gameObject { get; }
    Component hingeJoint { get; }
    Component light { get; }
    Component networkView { get; }
    Component particleSystem { get; }
    Component renderer { get; }
    Component rigidbody { get; }
    Component rigidbody2D { get; }
    string tag { get; set; }
    Transform transform { get; }

    void BroadcastMessage(string methodName);
    void BroadcastMessage(string methodName, object parameter);
    void BroadcastMessage(string methodName, object parameter, SendMessageOptions options);
    void BroadcastMessage(string methodName, SendMessageOptions options);
    bool CompareTag(string tag);
    Component GetComponent(string type);
    Component GetComponent(Type type);
    T GetComponent<T>();
    Component GetComponentInChildren(Type t);
    Component GetComponentInChildren(Type t, bool includeInactive);
    T GetComponentInChildren<T>();
    T GetComponentInChildren<T>(bool includeInactive);
    Component GetComponentInParent(Type t);
    T GetComponentInParent<T>();
    Component[] GetComponents(Type type);
    void GetComponents(Type type, List<Component> results);
    T[] GetComponents<T>();
    void GetComponents<T>(List<T> results);
    Component[] GetComponentsInChildren(Type t);
    Component[] GetComponentsInChildren(Type t, bool includeInactive);
    T[] GetComponentsInChildren<T>();
    T[] GetComponentsInChildren<T>(bool includeInactive);
    void GetComponentsInChildren<T>(bool includeInactive, List<T> result);
    void GetComponentsInChildren<T>(List<T> results);
    Component[] GetComponentsInParent(Type t);
    Component[] GetComponentsInParent(Type t, bool includeInactive);
    T[] GetComponentsInParent<T>();
    T[] GetComponentsInParent<T>(bool includeInactive);
    void GetComponentsInParent<T>(bool includeInactive, List<T> results);
    void SendMessage(string methodName);
    void SendMessage(string methodName, object value);
    void SendMessage(string methodName, object value, SendMessageOptions options);
    void SendMessage(string methodName, SendMessageOptions options);
    void SendMessageUpwards(string methodName);
    void SendMessageUpwards(string methodName, object value);
    void SendMessageUpwards(string methodName, object value, SendMessageOptions options);
    void SendMessageUpwards(string methodName, SendMessageOptions options);
    bool TryGetComponent(Type type, out Component component);
    bool TryGetComponent<T>(out T component);
}