using System.Collections.Generic.ValueMap;
using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.UIElements;

namespace Assets.Editor
{


    [CustomPropertyDrawer(typeof(Main.Other.TypeWrapper), true)]
    public class TypeDrawer : PropertyDrawer
    {
        protected CustomPopupResultHandle iPopupResult = null;

        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var baseClassNameProp = property.FindPropertyRelative("iBaseTypeClassName");
            var selectedClassNameProp = property.FindPropertyRelative("iSelectedTypeClassName");

            if (baseClassNameProp == null)
            {
                EditorGUI.LabelField(position, $"Serialized property '{property.propertyPath}' instance is null");
                return;
            }

            Rect elemRect = position;
            elemRect.width = EditorGUIUtility.labelWidth;
            elemRect.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.LabelField(elemRect, label);

            Type baseClass = Type.GetType(baseClassNameProp.stringValue);
            Type selectedClass = Type.GetType(selectedClassNameProp.stringValue);

            if (baseClass == null)
            {
                EditorGUI.LabelField(elemRect, $"Base class '{baseClassNameProp.stringValue}' not found.");
            }
            else
            {
                elemRect.x = EditorGUIUtility.labelWidth + 7;
                elemRect.width = position.width - elemRect.x - 30;
                elemRect = EditorGUI.IndentedRect(elemRect);
                EditorGUI.BeginDisabledGroup(true);
                EditorGUI.TextField(elemRect, selectedClassNameProp.stringValue);
                EditorGUI.EndDisabledGroup();
                elemRect.x += elemRect.width;
                elemRect.width = 30;

                if (GUI.Button(elemRect, "Set"))
                {
                    elemRect.width = 400;

                    UnityEditor.PopupWindow.Show(
                        elemRect, 
                        new CustomListPopupContent(
                            Main.Other.TypeWrapper.SelectInheritanceClasses(baseClass), 
                            (item) => { return (item as Type).FullName; }, 
                            out iPopupResult)) ;
                }

                if (iPopupResult?.IsClosed ?? false)
                {
                    iPopupResult.IsClosed = false;
                    selectedClassNameProp.stringValue = (iPopupResult.SelectedDataResult as Type)?.AssemblyQualifiedName ?? "";
                    iPopupResult = null;
                }
            }
        }
    }
    
}