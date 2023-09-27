using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Main;
using Main.Player;
using Main.Objects;
using Main.Objects.Behaviours;

namespace Main.Other
{
    public class UnityEditorHelpers
    {
        /// <summary>
        /// Marks a type as Routable. This means that a type will be used in routing processes as events, props and etc. Also this affects to behaviour a shared properties in a editor.
        /// Note: Shared property type used as a pivot type, value instance used as a current type
        /// </summary>

        protected class EditorGUILayoutInfo
        {
            /// <summary>
            /// string - label text
            /// object - input value
            /// bool - is readonly
            /// object - output value
            /// </summary>
            public Func<string, object, bool, object> Result = null;

            public EditorGUILayoutInfo(Func<string, object, bool, object> result)
            {
                Result = result;
            }
        }

        protected static Dictionary<Type, bool> iFoldoutStore = new Dictionary<Type, bool>();

        protected static Dictionary<Type, EditorGUILayoutInfo> iEditorsTypeMap = new Dictionary<Type, EditorGUILayoutInfo> {
            { typeof(string), new EditorGUILayoutInfo((S, T, R) => { return EditorGUILayout.TextField(S, (string)T); }) },
            { typeof(byte), new EditorGUILayoutInfo((S, T, R) => { return EditorGUILayout.IntField(S, (byte)T); }) },
            { typeof(long), new EditorGUILayoutInfo((S, T, R) => { return EditorGUILayout.IntField(S, (int)T); }) },
            { typeof(int), new EditorGUILayoutInfo((S, T, R) => { return EditorGUILayout.IntField(S, (int)T); }) },
            { typeof(float), new EditorGUILayoutInfo((S, T, R) => { return EditorGUILayout.FloatField(S, (float)T); }) },
            { typeof(double), new EditorGUILayoutInfo((S, T, R) => { return EditorGUILayout.DoubleField(S, (double)T); }) },
            { typeof(Vector2Int), new EditorGUILayoutInfo((S, T, R) => { return EditorGUILayout.Vector2IntField(S, (Vector2Int)T); }) },
            { typeof(Vector2), new EditorGUILayoutInfo((S, T, R) => { return EditorGUILayout.Vector2Field(S, (Vector2)T); }) },
            { typeof(Vector3Int), new EditorGUILayoutInfo((S, T, R) => { return EditorGUILayout.Vector3IntField(S, (Vector3Int)T); }) },
            { typeof(Vector3), new EditorGUILayoutInfo((S, T, R) => { return EditorGUILayout.Vector3Field(S, (Vector3)T); }) },
            { typeof(RectInt), new EditorGUILayoutInfo((S, T, R) => { return EditorGUILayout.RectIntField(S, (RectInt)T); }) },
            { typeof(Rect), new EditorGUILayoutInfo((S, T, R) => { return EditorGUILayout.RectField(S, (Rect)T); }) },
            { typeof(Enum), new EditorGUILayoutInfo((S, T, R) => { return EditorGUILayout.EnumFlagsField(S, (Enum)T); }) },
            { typeof(bool), new EditorGUILayoutInfo((S, T, R) => { return EditorGUILayout.Toggle(S, (bool)T); }) },
            { typeof(Color), new EditorGUILayoutInfo((S, T, R) => { return EditorGUILayout.ColorField(S, (Color)T); }) },
            { typeof(Player.IReadonlyPlayerObjectsRoot), new EditorGUILayoutInfo((S, T, R) => {
                    Player.IReadonlyPlayerObjectsRoot container = (IReadonlyPlayerObjectsRoot)T;
                    bool foldout;

                    if (!iFoldoutStore.TryGetValue(typeof(Player.IReadonlyPlayerObjectsRoot), out foldout))
                        iFoldoutStore.Add(typeof(Player.IReadonlyPlayerObjectsRoot), true);

                    EditorGUI.EndDisabledGroup();
                    iFoldoutStore[typeof(Player.IReadonlyPlayerObjectsRoot)] = EditorGUILayout.Foldout(foldout, S, true);
                    EditorGUI.BeginDisabledGroup(true);

                    if (container == null)
                        return T;


                    int indentLevel = EditorGUI.indentLevel;
                    EditorGUI.indentLevel++;

                    if (foldout)
                    {
                        foreach (IBehaviourContainer behContainer in container)
                        {
                            GUIFieldByType(behContainer.GetType(), "", behContainer, R);
                        }
                    }

                    EditorGUI.indentLevel = indentLevel;

                    return T;
                }) 
            },
        };

        public static Type[] SelectInheritanceClasses(Type baseClass)
        {
            return (
                from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                    // alternative: from domainAssembly in domainAssembly.GetExportedTypes()
                from type in domainAssembly.GetTypes()
                where baseClass.IsAssignableFrom(type) && !type.IsAbstract
                // alternative: && type != typeof(B)
                // alternative: && ! type.IsAbstract
                // alternative: where type.IsSubclassOf(typeof(B))
                select type).ToArray();
        }

        public static Type TypePopup(string label, Type selectedType, Type baseType)
        {
            Type[] result = SelectInheritanceClasses(baseType);
            List<string> list = new List<string>(result.Length);
            int selectedIndex = -1;

            for (int i=0; i<result.Length; i++)
            {
                if (result[i].Equals(selectedType) && (selectedIndex == -1))
                {
                    selectedIndex = i;
                }

                list.Add(result[i].FullName);
            }

            selectedIndex = EditorGUILayout.Popup(label, selectedIndex, list.ToArray());

            return (selectedIndex != -1) ? result[selectedIndex] : null;
        }


        public static Type TypePopupList(string label, Type selectedType, Type [] typeList)
        {
            Type[] result = typeList;
            List<string> list = new List<string>(result.Length);
            int selectedIndex = -1;

            for (int i = 0; i < result.Length; i++)
            {
                if (result[i].Equals(selectedType) && (selectedIndex == -1))
                {
                    selectedIndex = i;
                }

                list.Add(result[i].FullName);
            }

            selectedIndex = EditorGUILayout.Popup(label, selectedIndex, list.ToArray());

            return (selectedIndex != -1) ? result[selectedIndex] : null;
        }

        public static object GUIFieldByType(Type valueType, string label, object value, bool isReadonly)
        {
            EditorGUILayoutInfo result;
            RoutableTypeAttribute routable = valueType.GetCustomAttribute<RoutableTypeAttribute>(true);

            if (routable != null)
            {
                Type selectedType = TypePopup(label, value?.GetType() ?? null, valueType);

                if (selectedType != null)
                    return Activator.CreateInstance(selectedType);
                else
                    return null;
            }

            if (typeof(UnityEngine.Object).IsAssignableFrom(valueType))
            {
                UnityEngine.Object ob = EditorGUILayout.ObjectField(label, (UnityEngine.Object)value, valueType, true);
                return ob;
            }

            if (typeof(Enum).IsAssignableFrom(valueType))
            {
                valueType = typeof(Enum);
            }

            if (!iEditorsTypeMap.TryGetValue(valueType, out result))
            {
                GLog.LogFormat(LogType.Warning, "Could not associate type {0} with known editor field", valueType.FullName);
                return value;
            }

            return result.Result(label, value, isReadonly);
        }
    }

}
