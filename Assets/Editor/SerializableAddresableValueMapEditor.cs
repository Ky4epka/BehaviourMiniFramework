using System.Collections.Generic.ValueMap;
using UnityEngine;
using UnityEditor;

namespace Assets.Editor
{
    [CustomPropertyDrawer(typeof(ISerializableAddressableValueMap), true)]
    public class SerializableAddresableValueMapEditor : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int lineCount = 1;
            var rowKeysList = property.FindPropertyRelative("SerializedRowKeys");

            if (rowKeysList != null)
                lineCount = rowKeysList.arraySize;
 
            return lineCount * (EditorGUIUtility.singleLineHeight + 5 ) + EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // position.yMin = -position.height / 2;
            position = EditorGUI.IndentedRect(position);

            EditorGUI.BeginProperty(position, label, property);
            Rect labelRect = position;
            labelRect.height = EditorGUIUtility.singleLineHeight;
            GUIStyle style = new GUIStyle(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene).label);
            style.fontStyle = FontStyle.Bold;
            EditorGUI.LabelField(labelRect, "["+label.text+"]", style);
            var columnKeysList = property.FindPropertyRelative("SerializedColumnKeys");
            var rowKeysList = property.FindPropertyRelative("SerializedRowKeys");
            var cellsList = property.FindPropertyRelative("SerializedCellsLinear");


            int cellCounter = 0;
            position.position += new Vector2(0, EditorGUIUtility.singleLineHeight);

            if ((columnKeysList == null) || (rowKeysList == null))
            {
                EditorGUI.indentLevel = 1;
                position = EditorGUI.IndentedRect(position);
                EditorGUI.LabelField(position, "--- Property value is null or table is empty ---");
                return;
            }

            for (int i=-1; i<rowKeysList.arraySize; i++)
            {
                Rect offset = position;
                offset.width = 100;
                offset.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.BeginDisabledGroup(true);
                if (i >= 0)
                    EditorGUI.PropertyField(offset, rowKeysList.GetArrayElementAtIndex(i), new GUIContent(""), true);
                EditorGUI.EndDisabledGroup();
                offset.width = 100;
                //EditorGUI.LabelField(offset, "Row");

                for (int j = -1; j < columnKeysList.arraySize; j++)
                {
                    offset.position += new Vector2(5, 0);

                    if ((i == -1) && (j >= 0))
                    {
                        EditorGUI.BeginDisabledGroup(true);
                        EditorGUI.PropertyField(offset, columnKeysList.GetArrayElementAtIndex(j), new GUIContent(""), true);
                        EditorGUI.EndDisabledGroup();
                    }
                    else if (j >= 0)
                    {
                        var Value = cellsList.GetArrayElementAtIndex(cellCounter++);


                        EditorGUI.PropertyField(offset, Value, new GUIContent(""), false);
                    }

                    offset.position += new Vector2(offset.width, 0);
                }

                position.position += new Vector2(0, EditorGUIUtility.singleLineHeight + 5);
            }

            EditorGUI.EndProperty();
        }
    }
}