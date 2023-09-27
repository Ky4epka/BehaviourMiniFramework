using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Main.Events;

namespace Main.Other
{
    public class RectangleSelector : CachedMonoBehaviour
    {
        public NotifyEvent_2P<RectangleSelector, Vector3> OnSelectionStart = new NotifyEvent_2P<RectangleSelector, Vector3>();
        public NotifyEvent_2P<RectangleSelector, Vector3> OnSelectionMove = new NotifyEvent_2P<RectangleSelector, Vector3>();
        public NotifyEvent<RectangleSelector> OnSelectionEnd = new NotifyEvent<RectangleSelector>();

        protected Vector3 iCursorStart = Vector3.zero;
        protected Vector3 iCursorCurrent = Vector3.zero;
        protected bool iSelecting = false;
        protected List<GameObject> iSelectedObjects = new List<GameObject>();
        protected Texture2D iWorkTexture = null;

        public Vector3 CursorStart
        {
            get => iCursorStart;
        }

        public Vector3 CursorCurrent
        {
            get => iCursorCurrent;
        }

        public bool IsSelecting
        {
            get => iSelecting;
        }

        public List<GameObject> SelectedObjects
        {
            get => iSelectedObjects;
        }

        public void StartSelect(Vector3 cursorStart)
        {
            ClearSelection();
            iSelecting = true;
            iCursorStart = cursorStart;
            iCursorCurrent = cursorStart;
            OnSelectionStart.Invoke(this, iCursorStart);
        }

        public void MoveSelect(Vector3 cursor)
        {
            if (!iSelecting) return;

            iCursorCurrent = cursor;
            OnSelectionMove.Invoke(this, iCursorCurrent);
        }

        public void EndSelect(System.Predicate<Collider2D> filter = null)
        {
            if (!IsSelecting) return;

            iSelecting = false;
            iSelectedObjects.Clear();

            foreach (Collider2D col in CastSelection())
            {
                if ((filter == null) || filter(col))
                {
                    iSelectedObjects.Add(col.gameObject);
                }
            }

            OnSelectionEnd.Invoke(this);
        }

        public void ClearSelection()
        {
            iSelecting = false;
            iCursorStart = Vector3.zero;
            iCursorCurrent = Vector3.zero;
            iSelectedObjects.Clear();
        }

        protected Texture2D WorkTexture
        {
            get
            {
                if (iWorkTexture == null)
                {
                    iWorkTexture = new Texture2D(1, 1);
                    iWorkTexture.SetPixel(0, 0, Color.white);
                    iWorkTexture.Apply();
                }

                return iWorkTexture;
            }
        }

        protected void DrawScreenRect(Rect rect, Color color)
        {
            GUI.color = color;
            GUI.DrawTexture(rect, WorkTexture);
            GUI.color = Color.white;
        }

        protected void DrawScreenRectBorder(Rect rect, float thickness, Color color)
        {
            DrawScreenRect(new Rect(rect.xMin, rect.yMin, rect.width, thickness), color);
            DrawScreenRect(new Rect(rect.xMin, rect.yMin, thickness, rect.height), color);
            DrawScreenRect(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), color);
            DrawScreenRect(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), color);
        }

        protected Rect GetScreenRect(Vector3 screenPositionAt, Vector3 screenPositionTo)
        {
            screenPositionAt.y = Screen.height - screenPositionAt.y;
            screenPositionTo.y = Screen.height - screenPositionTo.y;
            Vector3 topLeft = Vector3.Min(screenPositionAt, screenPositionTo);
            Vector3 bottomRight = Vector3.Max(screenPositionAt, screenPositionTo);
            return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
        }

        protected Rect ScreenRectToWorldRect(Rect screen_rect, Camera camera)
        {
            if (camera == null)
                return Rect.zero;

            Vector3 min = camera.ScreenToWorldPoint(screen_rect.min);
            Vector3 max = camera.ScreenToWorldPoint(screen_rect.max);

            return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
        }

        protected Collider2D[] CastSelection()
        {
            Rect sel_rect = ScreenRectToWorldRect(Rect.MinMaxRect(iCursorStart.x, iCursorStart.y, iCursorCurrent.x, iCursorCurrent.y), Managers.MapCameraManager.Instance.ControllingCamera.Value);
            return Physics2D.OverlapAreaAll(sel_rect.min, sel_rect.max);
        }

        protected void OnGUI()
        {
            if (!iSelecting) return;

            Rect rect = GetScreenRect(iCursorStart, iCursorCurrent);
            DrawScreenRectBorder(rect, 2, new Color(0.8f, 0.8f, 0.8f, 0.25f));
        }

    }

}