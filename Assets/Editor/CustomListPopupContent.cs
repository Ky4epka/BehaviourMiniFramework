using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Assets.Editor
{
    public class CustomPopupResultHandle
    {
        public bool IsClosed { get; set; } = false;
        public bool HasChanges { get; set; } = false;
        public int SelectedIndex { get; set; } = -1;
        public object SelectedDataResult { get; set; } = null;
    }

    public class CustomListPopupContent : PopupWindowContent
    {
        public CustomPopupResultHandle ResultHandle { get; private set; }

        protected Vector2 iScrollPosition = Vector2.zero;
        protected int iSelectedIndex = -1;
        protected string iPrevSearchFilter = null;
        protected string iSearchFilter = "";
        protected List<int> iFilteredItemIndexes = null;
        /// <summary>
        /// Cached for GUI.SelectionGrid
        /// </summary>
        protected List<string> iFilteredItemDisplayNames = null;

        protected List<string> iDisplayNames = null;
        protected List<object> iData = null;

        public CustomListPopupContent(object[] items, string[] displayNames, out CustomPopupResultHandle resultHandle) : base()
        {
            Initialize(items, displayNames, out resultHandle);
        }

        public CustomListPopupContent(object[] items, Func<object, string> displayNameHandler, out CustomPopupResultHandle resultHandle) : base()
        {
            string[] displayNames = new string[items.Length];

            for (int i=0; i<items.Length; i++)
            {
                displayNames[i] = displayNameHandler(items[i]);
            }

            Initialize(items, displayNames, out resultHandle);
        }

        protected void Initialize(object[] items, string[] displayNames, out CustomPopupResultHandle resultHandle)
        {
            iData = new List<object>(items.Length);
            iDisplayNames = new List<string>(items.Length);

            for (int i = 0; i < Math.Min(items.Length, displayNames.Length); i++)
            {
                iData.Add(items[i]);
                iDisplayNames.Add(displayNames[i]);
            }

            iFilteredItemDisplayNames = new List<string>(iData.Count);
            iFilteredItemIndexes = new List<int>(iData.Count);

            ResultHandle = new CustomPopupResultHandle();
            resultHandle = ResultHandle;
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(600, 300);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rect"></param>
        /// <returns>Height of header</returns>
        public virtual float DrawHeaderGUI(Rect rect)
        {
            return 0f;
        }

        public override void OnGUI(Rect rect)
        {
            Rect elementRect = rect;
            // Search input
            elementRect.x += 5;
            elementRect.width -= 10;
            elementRect.y += 5;
            elementRect.height = EditorGUIUtility.singleLineHeight;
            iSearchFilter = EditorGUI.TextField(elementRect, "Search", iSearchFilter).Trim();

            // Selection filter
            if (iSearchFilter != iPrevSearchFilter)
            {
                iPrevSearchFilter = iSearchFilter;
                iSelectedIndex = -1;
                iFilteredItemDisplayNames.Clear();
                iFilteredItemIndexes.Clear();
                string iLowerSearch = iSearchFilter.ToLower().Trim();

                for (int i = 0; i < iDisplayNames.Count; i++)
                {
                    if (iDisplayNames[i].ToLower().Contains(iLowerSearch) || (iLowerSearch.Length == 0))
                    {
                        iFilteredItemIndexes.Add(i);
                        iFilteredItemDisplayNames.Add(iDisplayNames[i]);
                    }
                }
            }

            // Header draw
            float headerHeight = DrawHeaderGUI(elementRect);

            if (!MathKit.NumbersEquals(headerHeight, float.Epsilon))
            {
                elementRect.x = 10;
                elementRect.y += elementRect.height + 10;
                elementRect.width = GetWindowSize().x - 20;
                elementRect.height = 2;

                EditorGUI.DrawRect(elementRect, Color.gray);
            }

            // Scroll view
            elementRect.y += 5;
            elementRect.y += EditorGUIUtility.singleLineHeight;
            elementRect.height = rect.height - elementRect.y - 5 - EditorGUIUtility.singleLineHeight - 25;
            Rect scrollRect = elementRect;
            scrollRect.height = (EditorGUIUtility.singleLineHeight * 1.2f) * (iFilteredItemIndexes.Count);
            scrollRect.width -= 20;
            iScrollPosition = GUI.BeginScrollView(elementRect, iScrollPosition, scrollRect, false, false);
            iSelectedIndex = GUI.SelectionGrid(scrollRect, iSelectedIndex, iFilteredItemDisplayNames.ToArray(), 1);
            GUI.EndScrollView(true);

            elementRect.x = 10;
            elementRect.y += elementRect.height + 10;
            elementRect.width = GetWindowSize().x - 20;
            elementRect.height = 2;

            EditorGUI.DrawRect(elementRect, Color.gray);

            // Ok button
            elementRect.x = 5;
            elementRect.y += 10;
            elementRect.width = 80;
            elementRect.height = EditorGUIUtility.singleLineHeight + 5;

            if (GUI.Button(elementRect, "Ok"))
            {
                ResultHandle.SelectedIndex = iSelectedIndex;
                ResultHandle.SelectedDataResult = (iSelectedIndex != -1) ? iData[iSelectedIndex] : null;

                this.editorWindow.Close();
            }

            // Cancel button
            elementRect.width = 80;
            elementRect.x = this.GetWindowSize().x - elementRect.width - 5;

            if (GUI.Button(elementRect, "Cancel"))
            {
                this.editorWindow.Close();
            }
        }

        public override void OnClose()
        {
            base.OnClose();
            ResultHandle.IsClosed = true;
            ResultHandle = null;
        }

        public override void OnOpen()
        {
            base.OnOpen();
        }
    }
    
}