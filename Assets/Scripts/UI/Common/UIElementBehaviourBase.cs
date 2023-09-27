using UnityEngine;
using Main.Objects;
using Main.Objects.Behaviours;
using Main.Objects.Behaviours.Attributes;
using Main;
using Main.Events;


namespace Main.UI
{
    [RequireComponent(typeof(UIElementBase))]
    public class UIElementBehaviourBase: ObjectBehavioursBase, IUIElementBehaviourBase
    {

        public override IBehaviourContainer Container
        {
            get
            {
                if (iContainer == null)
                {
                    iContainer = GetComponent<IUIElementBase>();
                }

                return iContainer;
            }
        }

        public virtual IUIElementBase ElementBase
        {
            get => Container as IUIElementBase;
        }

        public IUINavigation Parent => ElementBase.Parent;

        public IUINavigation[] Childs => ElementBase.Childs;

        public IUINavigation PrevSibling => ElementBase.PrevSibling;

        public IUINavigation NextSibling => ElementBase.NextSibling;
        public IUINavigation FirstSibling => ElementBase.FirstSibling;
        public IUINavigation LastSibling => ElementBase.LastSibling;

        public IUINavigation FirstChild => ElementBase.FirstChild;

        public IUINavigation LastChild => ElementBase.LastChild;

        public void OnBehaviourAdd(IObjectBehavioursBase behaviour) => ElementBase.OnBehaviourAdd(behaviour);

        public void OnBehaviourDestroy(IObjectBehavioursBase behaviour) => ElementBase.OnBehaviourDestroy(behaviour);

    }

}
