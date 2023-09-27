using System.Collections;
using System.Collections.Generic;
using Main.Managers;
using System;
using Main.Objects;
using Main.Objects.Behaviours;
using Main.Objects.Behaviours.Attributes;

namespace Main.Player
{
    using UnityEditor;
    using UnityEditor.UIElements;
    using UnityEngine.UIElements;

    public enum PlayerType
    {
        Unknown = 0x00,
        Player = 0x01,
        AI = 0x02
    }

    public interface IReadonlyPlayerObjectsRoot: IReadonlyContainerCollection
    {
        IPlayerBase Owner { get; }
    }

    [Serializable]
    public class PlayerObjectsRoot: ContainerCollection, IReadonlyPlayerObjectsRoot
    {
        public IPlayerBase Owner { get; protected set; }

        public PlayerObjectsRoot(IPlayerBase owner)
        {
            Owner = owner;
        }
    }



    [Serializable]
    public class PlayerBase : BehaviourContainer, IPlayerBase
    {

        public CommonPlayerBehaviour Data
        {
            get => (!iPlayerData) ? iPlayerData = GetComponent<Player.CommonPlayerBehaviour>() : iPlayerData;
        }

        protected CommonPlayerBehaviour iPlayerData = null;

        protected override void Awake()
        {
            base.Awake();
            MasterManager = Managers.PlayerManager.Instance;
        }

        protected override void OnDestroy()
        {
            MasterManager = null;
            base.OnDestroy();
        }
    }

}