using UnityEngine;
using System.Collections.Generic;
using Main.Objects;

namespace Main.UI
{

    public class UIElementBase : BehaviourContainer, IUIElementBase
    {
        public virtual IUINavigation Parent => ExploreParentComponentByTransform<IUIElementBase>(transform);
        public virtual IUINavigation[] Childs => GetComponentsInChildren<IUIElementBase>();
        public virtual IUINavigation PrevSibling => FindNavItem<IUIElementBase>(transform.parent, transform.GetSiblingIndex() - 1, false);
        public virtual IUINavigation NextSibling => FindNavItem<IUIElementBase>(transform.parent, transform.GetSiblingIndex() + 1, true);
        public virtual IUINavigation FirstSibling => FindNavItem<IUIElementBase>(transform.parent, 0, true);
        public virtual IUINavigation LastSibling => FindNavItem<IUIElementBase>(transform.parent, transform.parent?.childCount ?? 0 - 1, false);
        public virtual IUINavigation FirstChild => FindNavItem<IUIElementBase>(transform, 0, true);
        public virtual IUINavigation LastChild => FindNavItem<IUIElementBase>(transform, transform.childCount - 1, false);

        public T FindNavItem<T>(Transform node, int start_index, bool direct) where T: IUINavigation
        {
            return ComponentByTransform<T>(node, start_index, direct, null);
        }

        public T ComponentByTransform<T>(Transform node, int start_index, bool direct, System.Predicate<T> filter) where T : IUINavigation
        {
            if (node == null)
                return default(T);

            int index = start_index;
            int count = (direct) ? (node.childCount - start_index) : (start_index + 1);
            int direction_op = (direct) ? (1) : (-1);

            while (count > 0)
            {
                T item = default(T);

                if ((index >= 0) && (index < node.childCount))
                    item = node.GetChild(index).GetComponent<T>();

                if ((item != null) && ((filter == null) || (filter(item))))
                    return item;

                index += direction_op;
                count--;
            }

            return default(T);
        }

        public T ExploreParentComponentByTransform<T>(Transform pivot)
        {
            if (pivot.parent != null)
            {
                T result;
                result = pivot.parent.GetComponent<T>();

                if (result != null)
                    return result;
                else
                    return ExploreParentComponentByTransform<T>(pivot.parent);
            }

            return default(T);
        }


        protected override void Awake()
        {
            base.Awake();
            MasterManager = Managers.UIManager.Instance;
        }

        protected override void OnDestroy()
        {
            MasterManager = null;
            base.OnDestroy();
        }
    }

}

