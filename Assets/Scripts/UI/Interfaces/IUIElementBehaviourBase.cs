using Main.Objects.Behaviours;

namespace Main.UI
{
    public interface IUIElementBehaviourBase: IObjectBehavioursBase, IUINavigation, IUIActionHandler
    {

    }
}


/*

    public class UIElementBase : BehaviourContainer, IUINavigation, IUIActivator, IUIActivable, IUIGraphicContainer, IUIActionHandler
    {
        public IUINavigation Parent => transform.parent?.GetComponent<IUINavigation>();
        public virtual IUINavigation[] Childs => GetComponentsInChildren<IUINavigation>();
        public virtual IUINavigation PrevSibling => FindNavItem<IUINavigation>(transform.GetSiblingIndex() - 1, false);
        public virtual IUINavigation NextSibling => FindNavItem<IUINavigation>(transform.GetSiblingIndex() + 1, true);
        public virtual IUINavigation FirstChild => FindNavItem<IUINavigation>(0, true);
        public virtual IUINavigation LastChild => FindNavItem<IUINavigation>(transform.childCount - 1, false);

        public virtual bool Active
        {
            get => iActive;
            set 
            {
                if (iActive == value)
                    return;

                iActive = value;
                UnityObject?.SetActive(value);
            }
        }

        public bool AllowMultiActivate
        {
            get => iAllowMultiActivations;
            set
            {
                if (iAllowMultiActivations == value)
                    return;

                iAllowMultiActivations = value;

                if (!iAllowMultiActivations &&
                    (iActivators.Count > 1))
                    iActivators.RemoveRange(1, iActivators.Count - 1);
            }
        }

        public virtual bool LoopActivation
        {
            get => iLoopActivation;
            set => iLoopActivation = value;
        }

        public IUIActivable ActivatedChild
        {
            get => iActivators[0];
            set
            {
                ActivatedChilds = new IUIActivable[1] { value };
            }
        }


        public IUIActivable[] ActivatedChilds
        {
            get => iActivators.ToArray();
            set
            {
                if (value == null)
                    return;

                if (!iAllowMultiActivations &&
                    value.Length > 1)
                    throw new System.InvalidOperationException("Trying an select multiple items then selector could not support multiselection.");

                ClearActivations();

                for (int i = 0; i < value.Length; i++)
                {
                    if (!iActivators.Contains(value[i]))
                    {
                        iActivators.Add(value[i]);
                        value[i].Active = true;
                    }
                }
            }
        }

        public int ActivatedCount => iActivators.Count;

        public GameObject UnityObject 
        {
            get => iUnityObject; 
            set
            {
                if (iUnityObject == value)
                    return;

                iUnityObject = value;
                iUnityObject?.SetActive(iActive);
                UpdateTransform();
            }
        }
        public int TransformChildIndex 
        { 
            get => iTransformChildIndex; 
            set
            {
                if (iTransformChildIndex == value)
                    return;

                iTransformChildIndex = value;
                UpdateTransformIndex();
            }
        }
        public Transform ChildsTransform 
        {
            get => iChildTransform;
            set
            {
                if (iChildTransform == value)
                    return;

                iChildTransform = value;
                UpdateTransform();
            }
        }

        protected void UpdateTransformIndex()
        {
            if (iTransformChildIndex != -1)
                UnityObject?.transform.SetSiblingIndex(iTransformChildIndex);
        }

        protected void UpdateTransform()
        {
            if (UnityObject != null)
                UnityObject.transform.SetParent(iChildTransform, false);

            UpdateTransformIndex();
        }

        protected bool iActive = false;
        protected bool iAllowMultiActivations = false;
        protected bool iLoopActivation = false;
        protected List<IUIActivable> iActivators = new List<IUIActivable>();
        protected GameObject iUnityObject = null;
        protected int iTransformChildIndex = -1;
        protected Transform iChildTransform = null;


        public void ActivateNext()
        {
            ClearActivations();
            IUIActivable activator = null;

            if (iActivators.Count == 0)
            {
                activator = FirstChild as IUIActivable;
            }
            else
            {
                activator = iActivators[iActivators.Count - 1].NextSibling as IUIActivable;

                if ((activator == null) &&
                    LoopActivation)
                    activator = FirstChild as IUIActivable;
            }

            ActivatedChild = activator;
        }

        public void ActivatePrev()
        {
            ClearActivations();
            IUIActivable activator = null;

            if (iActivators.Count == 0)
            {
                activator = LastChild as IUIActivable;
            }
            else
            {
                activator = iActivators[0].NextSibling as IUIActivable;

                if ((activator == null) &&
                    LoopActivation)
                    activator = LastChild as IUIActivable;
            }

            ActivatedChild = activator;
        }

        public void ClearActivations()
        {
            foreach (IUIActivable activator in iActivators)
                activator.Active = false;

            iActivators.Clear();
        }

        protected T FindNavItem<T>(int start_index, bool direct) where T: IUINavigation
        {
            if (transform.parent == null)
                return default(T);

            int index = start_index;
            int count = (direct) ? (transform.childCount - start_index) : (start_index + 1);
            int direction_op = (direct) ? (1) : (-1);

            while (count > 0)
            {
                T item = transform.parent.GetChild(index).GetComponent<T>();

                if (item != null)
                    return item;

                index += direction_op;
            }

            return default(T);
        }


        public virtual void PrepareUnityObject()
        {
        }

        public virtual void Action(string action_id)
        {
        }
    }
 */