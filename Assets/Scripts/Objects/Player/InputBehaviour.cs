using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Main.Objects;
using Main.Objects.Behaviours;
using System.Runtime.InteropServices;

namespace Main.Player
{
    [RequireComponent(typeof(PlayerBase))]
    public class InputBehaviour : PlayerBehaviourBase
    {
        public KeyPresets.PlayerKeyActionPreset KeyPreset { get; private set; } = null;

        protected override bool DoEnable()
        {
            if (!base.DoEnable())
                return false;

            KeyPreset = new KeyPresets.PlayerKeyActionPreset(Container as PlayerBase);
            return true;
        }

        protected override bool DoDisable()
        {
            if (!base.DoDisable())
                return false;

            if (KeyPreset != null)
            {
                KeyPreset.Dispose();
                KeyPreset = null;
            }

            return true;
        }
    }
}