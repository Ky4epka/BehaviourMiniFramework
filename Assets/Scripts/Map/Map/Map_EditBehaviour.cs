using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Main.Objects;
using Main.Objects.Behaviours;
using Main.Objects.Behaviours.Attributes;
using Main.Events;
using UnityEngine.EventSystems;
using UnityEditor;

namespace Main
{

    public class Map_EditBehaviour : ObjectBehavioursBase
    {
        protected PointerEventData.InputButton iPointerButton = PointerEventData.InputButton.Left;
        protected bool iPointerDown = false;
        protected bool iUnionSelection = false;
        protected bool iSubtractSelection = false;
        protected List<GameObject> iSelectedCells = new List<GameObject>();

        protected override bool DoEnable()
        {
            if (!base.DoEnable())
                return false;

            UnityEditor.Selection.selectionChanged += UpdateSelectionFromEditor;
            return true;
        }

        protected override bool DoDisable()
        {
            if (!base.DoDisable())
                return false;

            UnityEditor.Selection.selectionChanged -= UpdateSelectionFromEditor;
            return true;
        }

        protected void SelectCell(IBehaviourContainer cell, bool value, bool union, bool fromEditor = false)
        {
            if (!union)
                ClearSelection();

            ISharedProperty<bool> selected = cell?.SharedProperty<Aggregator.Properties.MapCell.SelectedProperty>();
            selected.Value = value;

            if (value)
                iSelectedCells.Add(cell.gameObject);
            else
                iSelectedCells.Remove(cell.gameObject);

            if (!fromEditor)
                UpdateSelected();
        }

        protected void ClearSelection(bool fromEditor = false)
        {
            foreach (GameObject cell in iSelectedCells)
            {
                ISharedProperty<bool> selected = cell.GetComponent<MapCell>()?.SharedProperty<Aggregator.Properties.MapCell.SelectedProperty>();
                selected.Value = false;
            }

            iSelectedCells.Clear();

            if (!fromEditor)
                UnityEditor.Selection.activeObject = null;
        }

        protected void UpdateSelectionFromEditor()
        {
            ClearSelection();

            foreach (MapCell cell in UnityEditor.Selection.GetFiltered<MapCell>(UnityEditor.SelectionMode.TopLevel))
            {
                SelectCell(cell, true, true, true);
            }
        }

        protected void UpdateSelected()
        {
            UnityEditor.Selection.objects = iSelectedCells.ToArray(); 
        }

        [EnabledStateEvent]
        public void PointerEnterEvent(Aggregator.Events.MapCell.PointerEnterEvent eventData)
        {
            ISharedProperty<bool> hovered = (eventData.Sender as IBehaviourContainer)?.SharedProperty<Aggregator.Properties.MapCell.HoveredProperty>();
            hovered.Value = true;

            if (iPointerDown)
            {
                switch (iPointerButton)
                {
                    case UnityEngine.EventSystems.PointerEventData.InputButton.Left:
                        SelectCell(eventData.Sender as IBehaviourContainer, !iSubtractSelection, iUnionSelection);
                        break;
                        
                    case UnityEngine.EventSystems.PointerEventData.InputButton.Right:
                        break;
                }
            }
        }

        [EnabledStateEvent]
        public void PointerLeaveEvent(Aggregator.Events.MapCell.PointerLeaveEvent eventData)
        {
            ISharedProperty<bool> hovered = (eventData.Sender as IBehaviourContainer)?.SharedProperty<Aggregator.Properties.MapCell.HoveredProperty>();
            hovered.Value = false;
        }

        [EnabledStateEvent]
        public void PointerDownEvent(Aggregator.Events.MapCell.PointerDownEvent eventData)
        {
            iPointerDown = true;
            iPointerButton = eventData.PointerEventData.button;
            iUnionSelection = Input.GetKey(KeyCode.LeftShift);
            iSubtractSelection = Input.GetKey(KeyCode.LeftAlt) && !iUnionSelection;

            SelectCell(eventData.Sender as IBehaviourContainer, !iSubtractSelection, iUnionSelection);
        }


        [EnabledStateEvent]
        public void PointerUpEvent(Aggregator.Events.MapCell.PointerUpEvent eventData)
        {
            iPointerDown = false;
        }

        protected void OnGUI()
        {
            Vector2Int position = new Vector2Int();
            Vector2Int size = new Vector2Int(150, 50);
            Vector2Int spacing = new Vector2Int(10, 10);

            position = new Vector2Int(10, 10);
            string path = "Assets/Prefabs/Map/Map.prefab";

            if (GUI.Button(new Rect(position, size), "Save"))
            {
                PrefabUtility.SaveAsPrefabAsset(gameObject, path);
            }

            position.y += size.y + spacing.y;

            if (GUI.Button(new Rect(position, size), "Load"))
            {
                GameObject map = PrefabUtility.LoadPrefabContents(path);
                Map_Common source_Common = map.GetComponent<Map_Common>();
                Map_Common dest_Common = Container.GetComponent<Map_Common>();
                Debug.Log(map.transform.childCount);

            }

            position.y += size.y + spacing.y;

            if (GUI.Button(new Rect(position, size), "Load v1"))
            {
                Map_Config config = new Map_Config(Container.GetComponent<Map_Common>());

                config.LoadFromFile("Assets/Resources/Map/test_map.map");
            }
        }
    }

}