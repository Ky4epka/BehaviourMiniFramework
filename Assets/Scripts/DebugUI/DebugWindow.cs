using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

namespace Main.Editors
{

    public class DebugWindow : EditorWindow
    {

        [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
        public class DebugClassAttribute : Attribute
        {
            /// <summary>
            /// If null then a group naming as class name
            /// </summary>
            public string GroupName = null;
        }

        [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
        public class DebugPropertyAttribute: Attribute
        {
            /// <summary>
            /// If null then a group naming as property name
            /// </summary>
            public string Label = null;
            /// <summary>
            /// If null then a group naming as class name
            /// </summary>
            public string GroupName = null;
        }

        protected class Element
        {
            public string Label = "";
            public object TargetInstance = null;
            public PropertyInfo BindedProp = null;
            public ElementGroup Container
            {
                get => iContainer;
                set
                {
                    if (iContainer == value)
                        return;

                    if (iContainer != null)
                        iContainer.Remove(this);

                    iContainer = value;

                    if (iContainer != null)
                        iContainer.Add(this);
                }
            }

            protected ElementGroup iContainer = null;

            public Element(ElementGroup container, string label, object target, PropertyInfo property)
            {
                Container = container;

                if (label == null)
                    Label = property.Name;
                else
                    Label = label;

                TargetInstance = target;
                BindedProp = property;
            }
        }

        protected class ElementGroup: List<Element>
        {
            public string Name = "";
            public bool GUIShowGroup = false;

            public ElementGroup(string name): base()
            {
                Name = name;
            }

        }

        protected class ElementRegisterLink : IDisposable
        {
            private Element iLinkedElement = null;
            protected bool iDisposed = false;

            public ElementRegisterLink(Element linked_element)
            {
                iLinkedElement = linked_element;
            }

            public void Dispose()
            {
                if (iDisposed)
                    throw new ObjectDisposedException(this.ToString());

                iDisposed = true;
                iLinkedElement.Container = null;
            }
        }

        protected Dictionary<string, ElementGroup> iGroups = new Dictionary<string, ElementGroup>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="group_name"></param>
        /// <param name="label"></param>
        /// <param name="property">Can not be a null</param>
        /// <returns>Unregistering link represented as IDisposable</returns>
        public IDisposable RegisterPropertyDisposable(string group_name, string label, object target, PropertyInfo property)
        {
            if (property == null)
                throw new ArgumentNullException("property");

            ElementGroup group;

            if (!iGroups.TryGetValue(group_name, out group))
            {
                group = new ElementGroup(group_name);
                iGroups.Add(group_name, group);
            }

            Element elem = new Element(group, label, target, property);
            return new ElementRegisterLink(elem);
        }

        public void RegisterProperty(string group_name, string label, object target, PropertyInfo property)
        {
            if (property == null)
                throw new ArgumentNullException("property");

            ElementGroup group;

            if (group_name == null)
                group_name = target.GetType().FullName;

            if (!iGroups.TryGetValue(group_name, out group))
            {
                group = new ElementGroup(group_name);
                iGroups.Add(group_name, group);
            }

            Element elem = new Element(group, label, target, property);
        }

        public void RegisterObject(object target)
        {
            Type t = target.GetType();

            DebugClassAttribute obj_attr = t.GetCustomAttribute<DebugClassAttribute>();

            if (obj_attr == null)
                return;

            foreach (PropertyInfo prop in t.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                DebugPropertyAttribute prop_attr = prop.GetCustomAttribute<DebugPropertyAttribute>();

                if (prop_attr == null)
                    continue;

                RegisterProperty(obj_attr.GroupName, prop_attr.Label, target, prop);
            }
        }

        public bool UnregisterProperty(string group_name, string label)
        {
            return iGroups[group_name].RemoveAll((Element elem) => { return elem.Label.Equals(label); }) > 0;
        }

        public void Clear()
        {
            iGroups.Clear();
        }

        [MenuItem("Window/Debug window")]
        public static void ShowWnd()
        {
            DebugWindow wnd = GetWindow<DebugWindow>(true, "Debug window", true);

            wnd.Clear();
            foreach (MonoBehaviour beh in FindObjectsOfType<MonoBehaviour>())
            {
                wnd.RegisterObject(beh);
            }
        }

        protected void DrawElement(Element element)
        {
            EditorGUILayout.LabelField(element.Label, element.BindedProp.GetValue(element.TargetInstance).ToString());            
        }

        protected void DrawGroup(ElementGroup group)
        {
            if (group.Count == 0)
                return;

            group.GUIShowGroup = EditorGUILayout.BeginFoldoutHeaderGroup(group.GUIShowGroup, group.Name);
            try
            {
                if (group.GUIShowGroup)
                {
                    foreach (Element element in group)
                    {
                        DrawElement(element);
                    }
                }

                if (!Selection.activeTransform)
                {
                    group.GUIShowGroup = false;
                }
            }
            finally
            {
                EditorGUILayout.EndFoldoutHeaderGroup();
            }
        }

        protected void DrawGroups()
        {
            foreach (KeyValuePair<string, ElementGroup> group in iGroups)
            {
                DrawGroup(group.Value);
            }
        }

        public void OnGUI()
        {
            DrawGroups();
        }

        public void OnInspectorUpdate()
        {
            this.Repaint();
        }

    }
}