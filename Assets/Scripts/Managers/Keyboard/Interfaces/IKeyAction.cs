using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Main.Managers.KeyboardEvents;

namespace Main.Events.KeyCodePresets
{
    public delegate bool KeyActionCondition(IKeyAction sender, KeyCode key_code, KeyState key_state);

    public interface IKeyAction : INameable, ICloneable, IAssignable, IDisposable
    {
        IKeyActionHandler Handler { get; set; }
        string Description { get; set; }
        IEnumerable<KeyAction_KeyData> Keys { get; set; }
        KeyActionCondition Condition { get; set; }
        /// <summary>
        /// If you need stop a key state propagation return the false
        /// </summary>
        Func<IKeyAction, KeyCode, KeyState, bool> Action { get; set; }
        object Data { get; set; }

        bool BindKeyData(KeyAction_KeyData key_data);
        bool UnbindKeyData(KeyAction_KeyData key_data);
        bool IsKeyDataBinded(KeyAction_KeyData key_data);

        bool Invoke(KeyCode key_code, KeyState key_state);
    }

}