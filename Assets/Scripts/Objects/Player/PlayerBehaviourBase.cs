using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Main.Objects;
using Main.Objects.Behaviours;
using System.Runtime.InteropServices;

namespace Main.Player
{
    [RequireComponent(typeof(PlayerBase))]
    public class PlayerBehaviourBase : ObjectBehavioursBase, IPlayerBase
    {
        public override IBehaviourContainer Container
        {
            get
            {
                if (iContainer == null)
                {
                    iContainer = GetComponent<IPlayerBase>();
                }

                return iContainer;
            }
        }

        public IPlayerBase PlayerBase
        {
            get => Container as IPlayerBase;
        }

        public CommonPlayerBehaviour Data
        {
            get => PlayerBase.Data;
        }


        public ISharedPropertiesContainer SharedPropertyContainer => Container.SharedPropertyContainer;

        public void Assign(IAssignable source) => Container.Assign(source);

        public void OnBehaviourAdd(IObjectBehavioursBase behaviour) => Container.OnBehaviourAdd(behaviour);
        public void OnBehaviourDestroy(IObjectBehavioursBase behaviour) => Container.OnBehaviourDestroy(behaviour);
    }

}