using UnityEngine;
using Main.Managers.KeyboardEvents;
using System;
using System.Runtime.InteropServices;

namespace Main.Events.KeyCodePresets
{
    public interface IKeyActionHandler: ICloneable, IAssignable, INameable
    {
        void AddAction(IKeyAction action);
        bool RemoveAction(IKeyAction action);
        bool RemoveAction(string name);

        IKeyAction GetAction(string name);
        IKeyAction GetAction(KeyAction_KeyData key_data);
        bool KeyAlreadyBindedWith(KeyAction_KeyData key_data, IKeyAction action);
        bool KeyAlreadyBinded(KeyAction_KeyData key_data);
        bool HandleKey(KeyCode key_code, KeyState key_state);
    }
}