using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Main.UI;
using Main.Events;
using Main.Events.KeyCodePresets;
using Main.Managers.KeyboardEvents;
using System;

namespace Main.UI.KeyPresets
{
    
    public class UIKeyAction : KeyActionBase
    {
        protected IUIController iController = null;

        public IUIController Controller
        {
            get => iController;
            set => iController = value;
        }
        public virtual KeyCode Key { get; } = KeyCode.Escape;

        public UIKeyAction() : base()
        {

        }

        public UIKeyAction(IUIController controller) : base()
        {
            Controller = controller;
        }

        public UIKeyAction(IKeyActionHandler handler, IUIController controller) : base(handler)
        {
            Controller = controller;
        }

    }

    public class UIKeyActionNavigate: UIKeyAction
    {
        public virtual UINavigate Direction { get; } = UINavigate.Unknown;

        public UIKeyActionNavigate(IUIController controller): base(controller)
        {

        }

        protected override void Initialize()
        {
            base.Initialize();
            BindKeyData(new KeyAction_KeyData(Key, KeyState.Down, false));
            BindKeyData(new KeyAction_KeyData(Key, KeyState.Up, false));
            Action = (IKeyAction sender, KeyCode key_code, KeyState key_state) =>
            {
                if (!Controller.enabled)
                    return true;

                if ((Controller != null) &&
                    (key_state == KeyState.Down))
                    Controller.Event< Aggregator.Events.UI.UIController.DoNavigateEvent>(Controller.Container).Invoke(Direction);
                return false;
            };
        }
    }

    public class UIKeyActionNavigateToUp: UIKeyActionNavigate
    {
        public override KeyCode Key => KeyCode.UpArrow;
        public override UINavigate Direction { get; } = UINavigate.Up;
        public override string GetName() { return "NavigateUp"; }
        public override string Description { get => base.Description; set => base.Description = "Navigates to up"; }

        public UIKeyActionNavigateToUp(IUIController controller) : base(controller)
        {

        }
    }

    public class UIKeyActionNavigateToDown : UIKeyActionNavigate
    {
        public override KeyCode Key => KeyCode.DownArrow;
        public override UINavigate Direction { get; } = UINavigate.Down;
        public override string GetName() { return "NavigateDown"; }
        public override string Description { get => base.Description; set => base.Description = "Navigates to down"; }

        public UIKeyActionNavigateToDown(IUIController controller) : base(controller)
        {

        }
    }

    public class UIKeyActionNavigateToLeft : UIKeyActionNavigate
    {
        public override KeyCode Key => KeyCode.LeftArrow;
        public override UINavigate Direction { get; } = UINavigate.Left;
        public override string GetName() { return "NavigateLeft"; }
        public override string Description { get => base.Description; set => base.Description = "Navigates to left"; }

        public UIKeyActionNavigateToLeft(IUIController controller) : base(controller)
        {

        }
    }

    public class UIKeyActionNavigateToRight : UIKeyActionNavigate
    {
        public override KeyCode Key => KeyCode.RightArrow;
        public override UINavigate Direction { get; } = UINavigate.Right;
        public override string GetName() { return "NavigateRight"; }
        public override string Description { get => base.Description; set => base.Description = "Navigates to right"; }

        public UIKeyActionNavigateToRight(IUIController controller) : base(controller)
        {

        }
    }


    public class UIKeyActionClick: UIKeyAction
    {
        public override KeyCode Key => KeyCode.Return;
        public override string GetName() { return "UIClick"; }
        public override string Description { get => base.Description; set => base.Description = "Clicks to ui element"; }

        public UIKeyActionClick(IUIController controller) : base(controller)
        {

        }

        protected override void Initialize()
        {
            base.Initialize();
            BindKeyData(new KeyAction_KeyData(Key, KeyState.Down, false));
            BindKeyData(new KeyAction_KeyData(Key, KeyState.Up, false));
            Action = (IKeyAction sender, KeyCode key_code, KeyState key_state) =>
            {
                if (!Controller.enabled)
                    return true;

                if ((Controller != null) &&
                    (key_state == KeyState.Down))
                    Controller.Event<Aggregator.Events.UI.UIController.DoNavigateEvent>(Controller.Container).Invoke(UINavigate.Right);
                return false;
            };
        }
    }

    public class UIKeyActionBack : UIKeyAction
    {
        public override KeyCode Key => KeyCode.Escape;
        public override string GetName() { return "UIBack"; }
        public override string Description { get => base.Description; set => base.Description = "Backs to menu hierarchy"; }
        
        public UIKeyActionBack(IUIController controller) : base(controller)
        {

        }

        protected override void Initialize()
        {
            base.Initialize();
            BindKeyData(new KeyAction_KeyData(Key, KeyState.Down, false));
            BindKeyData(new KeyAction_KeyData(Key, KeyState.Up, false));
            Action = (IKeyAction sender, KeyCode key_code, KeyState key_state) =>
            {
                if (!Controller.enabled)
                    return true;

                if ((Controller != null) &&
                    (key_state == KeyState.Down))
                    Controller.Event<Aggregator.Events.UI.UIController.DoNavigateEvent>(Controller.Container).Invoke(UINavigate.Left);
                return false;
            };
        }
    }
    
}