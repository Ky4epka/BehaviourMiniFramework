using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Main.UI;
using Main.Events;
using Main.Events.KeyCodePresets;
using Main.Managers.KeyboardEvents;
using Main.Objects;
using System;

namespace Main.Player.KeyPresets
{
    using Main.Aggregator.Enum.Behaviours.Movable.AxisMotionBehaviour;

    public class PlayerControlKeyActionMove : KeyActionBase
    {
        // LEFT + RIGHT = NOMOVE
        // TOP + DOWN = NOMOVE
        //

        protected static Dictionary<KeyCode, AxisDirectionMove> KeyCodeDirectionMap = new Dictionary<KeyCode, AxisDirectionMove>()
        { 
          { KeyCode.LeftArrow, AxisDirectionMove.MoveLeft},
          { KeyCode.RightArrow, AxisDirectionMove.MoveRight},
          { KeyCode.UpArrow, AxisDirectionMove.MoveTop},
          { KeyCode.DownArrow, AxisDirectionMove.MoveBottom}
        };

        protected List<KeyCode> KeysPressed = new List<KeyCode>();

        protected IPlayerBase iPlayer = null;

        public IPlayerBase Player
        {
            get => iPlayer;
            set => iPlayer = value;
        }

        public PlayerControlKeyActionMove() : base()
        {

        }

        public PlayerControlKeyActionMove(IPlayerBase player) : base()
        {
            Player = player;
        }

        public PlayerControlKeyActionMove(IKeyActionHandler handler, IPlayerBase player) : base(handler)
        {
            Player = player;
        }

        protected override void Initialize()
        {
            base.Initialize();
            Name = "Key action move";
            BindKeyData(new KeyAction_KeyData(KeyCode.LeftArrow, KeyState.Down, false));
            BindKeyData(new KeyAction_KeyData(KeyCode.LeftArrow, KeyState.Up, false));
            BindKeyData(new KeyAction_KeyData(KeyCode.UpArrow, KeyState.Down, false));
            BindKeyData(new KeyAction_KeyData(KeyCode.UpArrow, KeyState.Up, false));
            BindKeyData(new KeyAction_KeyData(KeyCode.RightArrow, KeyState.Down, false));
            BindKeyData(new KeyAction_KeyData(KeyCode.RightArrow, KeyState.Up, false));
            BindKeyData(new KeyAction_KeyData(KeyCode.DownArrow, KeyState.Down, false));
            BindKeyData(new KeyAction_KeyData(KeyCode.DownArrow, KeyState.Up, false));
            Action = (IKeyAction sender, KeyCode key_code, KeyState key_state) =>
            {
                if (!Player.enabled)
                    return true;

                if (Player != null)
                {
                    if (KeysPressed.Contains(key_code))
                    {
                        if (key_state == KeyState.Up)
                            KeysPressed.Remove(key_code);
                    }
                    else if (key_state == KeyState.Down)
                        KeysPressed.Add(key_code);

                    bool left_pressed = KeysPressed.Contains(KeyCode.LeftArrow);
                    bool right_pressed = KeysPressed.Contains(KeyCode.RightArrow);
                    bool up_pressed = KeysPressed.Contains(KeyCode.UpArrow);
                    bool down_pressed = KeysPressed.Contains(KeyCode.DownArrow);

                    Main.Aggregator.Enum.Behaviours.Movable.AxisMotionBehaviour.AxisDirectionMove direction = AxisDirectionMove.NoMove;

                    if ((left_pressed && right_pressed) ||
                        (up_pressed && down_pressed) ||
                        (KeysPressed.Count == 0))
                    {
                        direction = AxisDirectionMove.NoMove;
                    }
                    else
                        direction = KeyCodeDirectionMap[KeysPressed[KeysPressed.Count-1]];

                    foreach (var cur in Player.Data.SelectedObjects.Value)
                    {
                        cur.SharedProperty<Aggregator.Properties.Behaviours.Movable.AxisMotionBehaviour.AxisMovingDirectionProperty>().Value = direction;
                    }
                }

                return false;
            };
        }
    }


    public class PlayerControlKeyActionFire : KeyActionBase
    {
        protected IPlayerBase iPlayer = null;

        public IPlayerBase Player
        {
            get => iPlayer;
            set => iPlayer = value;
        }

        public PlayerControlKeyActionFire() : base()
        {

        }

        public PlayerControlKeyActionFire(IPlayerBase player) : base()
        {
            Player = player;
        }

        public PlayerControlKeyActionFire(IKeyActionHandler handler, IPlayerBase player) : base(handler)
        {
            Player = player;
        }

        protected override void Initialize()
        {
            base.Initialize();
            Name = "Key action fire";
            BindKeyData(new KeyAction_KeyData(KeyCode.Return, KeyState.Down, false));
            //BindKeyData(new KeyAction_KeyData(KeyCode.Return, KeyState.Up, false));
            Action = (IKeyAction sender, KeyCode key_code, KeyState key_state) =>
            {
                if (!Player.enabled)
                    return true;

                if (Player != null)
                {
                    foreach (var cur in Player.Data.SelectedObjects.Value)
                    {
                        cur.Event<Aggregator.Events.Behaviours.Tanks.DoFireEvent>(Player).Invoke();
                    }
                }

                return false;
            };
        }
    }

    public class PlayerKeyActionPreset: KeyActionPreset
    {
        public PlayerBase Player { get; private set; } = null;

        public PlayerKeyActionPreset(PlayerBase player) : base()
        {
            Configure(player);
        }

        public PlayerKeyActionPreset(PlayerBase player, int priority): base(priority)
        {
            Configure(player);
        }

        public PlayerKeyActionPreset(PlayerBase player, IPriorityObservable<IKeyHandler> observable, int priority) : base(observable, priority)
        {
            Configure(player);
        }


        protected void Configure(PlayerBase player)
        {
            Player = player;

            if (Player == null)
                return;

            AddAction(new PlayerControlKeyActionMove(player));
            AddAction(new PlayerControlKeyActionFire(player));
            Priority = Configuration.Instance.KeyBoardPriority[typeof(InputBehaviour)];
            Observable = Managers.KeyboardManager.Instance;
        }
    }

}
