using Main.Managers;
using UnityEngine;

namespace Main.Managers
{
    public interface IPlayerManager: IObjectManager
    {

    }

    [DefaultExecutionOrder(KnownExecutionOrder.PlayerManagerOrder)]
    public class PlayerManager : ObjectManagerMonoBehaviourWrapper, IPlayerManager
    {
        protected static PlayerManager iInstance = null;
        public override IObjectManager ObjectManager
        {
            get => (base.ObjectManager == null) ? base.ObjectManager = new ObjectManager() : base.ObjectManager;
            protected set => base.ObjectManager = value;
        }

        public static PlayerManager Instance => (iInstance == null) ? ((iInstance = FindObjectOfType<PlayerManager>()) ? iInstance : throw new System.NullReferenceException($"Could not find a {nameof(PlayerManager)}")) : iInstance;

    }

}