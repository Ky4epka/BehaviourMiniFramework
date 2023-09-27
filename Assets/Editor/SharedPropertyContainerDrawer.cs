using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Main.Objects;
using System;
using Assets.Editor;
using System.Diagnostics;

namespace Main.Editors.Objects
{
    [CustomPropertyDrawer(typeof(SharedPropertiesContainer), true)]
    public class SharedPropertyContainerDrawer : PropertyDrawer
    {
        protected CustomPopupResultHandle iAddPropertyPopupResultHandle = null;
        protected CustomPopupResultHandle iDeletePropertyPopupResultHandle = null;

        protected bool iFoldState = true;
        protected Dictionary<string, bool> iGroupsFoldState = new Dictionary<string, bool>();

        private struct SerializedPropertyMeta
        {
            public SerializedProperty Property;
            public ISharedProperty PropertyValue;
            public SerializedProperty ValueSubProperty { get; private set; }

            public SerializedPropertyMeta(SerializedProperty prop, ISharedProperty value)
            {
                Property = prop;
                PropertyValue = value;
                ValueSubProperty = prop.FindPropertyRelative("iValue");
            }
        }

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return base.CreatePropertyGUI(property);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return (iFoldState) ? (HandleProps(property, Rect.zero, false) + 25 + EditorGUIUtility.singleLineHeight * 2f) : EditorGUIUtility.singleLineHeight * 2f;
        }

        public float HandleProps(SerializedProperty property, Rect position, bool drawGUI)
        {
            var propArray = property.FindPropertyRelative(nameof(SharedPropertiesContainer.SerializedPropValues));

            if (propArray == null)
                throw new InvalidOperationException($"Property {nameof(SharedPropertiesContainer.SerializedPropValues)} not found in {nameof(SharedPropertiesContainer)}");

            float height = EditorGUIUtility.singleLineHeight;

            position.x = 0;
            position.width = 1600;
            
            position = EditorGUI.IndentedRect(position);
            SortedDictionary<string, List<SerializedPropertyMeta>> PropertiesMeta = new SortedDictionary<string, List<SerializedPropertyMeta>>();
            IBehaviourContainer target = propArray.serializedObject.targetObject as IBehaviourContainer;

            Rect pos = position;
            for (int i = propArray.arraySize - 1; i >= 0 ; i--)
            {
                var arrayElement = propArray.GetArrayElementAtIndex(i);
                string propTypeFullName = arrayElement.managedReferenceFullTypename;

                propTypeFullName = propTypeFullName.Replace("Assembly-CSharp ", "").Trim() + ", " + System.Reflection.Assembly.GetAssembly(typeof(BehaviourContainer)).FullName;
                pos.y += 20;

                Type propType = Type.GetType(propTypeFullName);
                ISharedProperty prop = target.SharedProperty(propType);

                List<SerializedPropertyMeta> propMetaList = null;

                if (!PropertiesMeta.TryGetValue(prop.GroupTag, out propMetaList))
                {
                    propMetaList = new List<SerializedPropertyMeta>();
                    PropertiesMeta.Add(prop.GroupTag, propMetaList);
                }

                propMetaList.Add(new SerializedPropertyMeta(arrayElement, prop));
            }

            foreach (KeyValuePair<string, List<SerializedPropertyMeta>> keyValue in PropertiesMeta)
            {
                bool folded = true;

                if (!iGroupsFoldState.TryGetValue(keyValue.Key, out folded))
                    iGroupsFoldState.Add(keyValue.Key, false);


                if (drawGUI)
                {
                    GUIStyle labelStyle = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene).label;
                    labelStyle.fontStyle = FontStyle.Bold;
                    position.x = 20;
                    EditorGUI.indentLevel = 0;
                    position = EditorGUI.IndentedRect(position);
//                    EditorGUI.LabelField(new Rect(position.position, new Vector2(EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight)), $"[{keyValue.Key}]", labelStyle);
                    iGroupsFoldState[keyValue.Key] = EditorGUI.BeginFoldoutHeaderGroup(new Rect(position.position, new Vector2(EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight)), folded, $"{((folded) ? "[-]" : "[+]")} [{keyValue.Key}]", labelStyle);
                    EditorGUI.indentLevel = 1;
                    position = EditorGUI.IndentedRect(position);
                }

                position.y += EditorGUIUtility.singleLineHeight + 5;
                position.height += EditorGUIUtility.singleLineHeight + 5;
                height += EditorGUIUtility.singleLineHeight + 5;

                if (folded)
                {

                    foreach (SerializedPropertyMeta propMeta in keyValue.Value)
                    {
                        var sharedPropVal = propMeta.ValueSubProperty;//.FindPropertyRelative("iValue");

                        if (sharedPropVal == null)
                        {
                            EditorGUI.LabelField(position, new GUIContent("Value of property " + propMeta.PropertyValue.SharedName + " is inaccessible "));
                            continue;
                        }

                        float valHeight = 0f;
                        valHeight = EditorGUI.GetPropertyHeight(sharedPropVal, true);

                        position.height += valHeight + 5;
                        height += valHeight + 5;

                        if (drawGUI)
                        {
                            EditorGUI.BeginChangeCheck();

                            EditorGUI.BeginDisabledGroup(propMeta.PropertyValue.IsReadOnly);
                            Rect p = position;
                            p.y += 10;

                            propMeta.PropertyValue.SyncReadonlyValue();
                            EditorGUI.PropertyField(new Rect(position.position, new Vector2(EditorGUIUtility.currentViewWidth - position.x - 20, valHeight)), sharedPropVal, new GUIContent(propMeta.PropertyValue.SharedName), true);

                            EditorGUI.EndDisabledGroup();

                            if (EditorGUI.EndChangeCheck())
                            {
                                sharedPropVal.serializedObject.ApplyModifiedProperties();
                                propMeta.PropertyValue.DirtyValue();
                            }
                        }

                        position.y += valHeight + 5;
                    }
                }
                
                if (drawGUI)
                {
                    EditorGUI.EndFoldoutHeaderGroup();
                }                
            }

            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label = new GUIContent(((iFoldState) ? "[-]" : "[+]") + " Shared properties");
            GUIStyle style = new GUIStyle(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene).label);
            style.fontSize += 4;
            style.alignment = TextAnchor.UpperLeft;
            //iFoldState = EditorGUI.BeginFoldoutHeaderGroup(position, iFoldState, label, style);
            position = EditorGUI.IndentedRect(position);
            iFoldState = EditorGUI.Foldout(new Rect(position.position, new Vector2(EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight * 2f)), iFoldState, label, true, style);
            position.y += EditorGUIUtility.singleLineHeight * 2;

            if (iFoldState)
            {
                Rect elemRect = position;
                elemRect.x = 20;
                elemRect.y += 5;
                elemRect.width = 125;
                elemRect.height = 25;

                if (GUI.Button(elemRect, "Add property"))
                {
                    elemRect.width = 800;

                    UnityEditor.PopupWindow.Show(
                        elemRect,
                        new CustomListPopupContent(
                            Main.Other.TypeWrapper.SelectInheritanceClasses(typeof(ISharedProperty)),
                            (item) => { return (item as Type).FullName; },
                            out iAddPropertyPopupResultHandle));
                }

                if (iAddPropertyPopupResultHandle?.IsClosed ?? false)
                {
                    iAddPropertyPopupResultHandle.IsClosed = false;

                    if (iAddPropertyPopupResultHandle.SelectedIndex != -1)
                        (property.serializedObject.targetObject as IBehaviourContainer).SharedProperty(iAddPropertyPopupResultHandle.SelectedDataResult as Type);
                }

                elemRect.x += elemRect.width + 5;

                if (GUI.Button(elemRect, "Delete property"))
                {
                    elemRect.width = 800;
                    var propCol = (property.serializedObject.targetObject as IBehaviourContainer).PropertyCollection;
                    Type[] propTypes = new Type[propCol.Count];
                    int counter = 0;

                    foreach (var prop in propCol)
                        propTypes[counter++] = prop.GetType();

                    UnityEditor.PopupWindow.Show(
                        elemRect,
                        new CustomListPopupContent(
                            propTypes,
                            (item) => { return (item as Type).FullName; },
                            out iDeletePropertyPopupResultHandle));
                }

                elemRect.x += elemRect.width + 5;

                if (GUI.Button(elemRect, "Rebuild properties"))
                    (property.serializedObject.targetObject as BehaviourContainer).RebuildSharedProperties();

                position.y += 25;

                var indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 1;

                position.y += 20;
                position = EditorGUI.IndentedRect(position);
                HandleProps(property, position, true);

                EditorGUI.indentLevel = indent;

                if (iDeletePropertyPopupResultHandle?.IsClosed ?? false)
                {
                    iDeletePropertyPopupResultHandle.IsClosed = false;

                    if (iDeletePropertyPopupResultHandle.SelectedIndex != -1)
                    {
                        (property.serializedObject.targetObject as IBehaviourContainer).DeleteSharedProperty(iDeletePropertyPopupResultHandle.SelectedDataResult as Type);
                    }
                }
            }

            EditorGUI.EndFoldoutHeaderGroup();
        }

    }
}
