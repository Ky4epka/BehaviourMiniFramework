using System;
using UnityEngine;
using Main.Managers.KeyboardEvents;
using System.Xml.Serialization;

namespace Main.Events.KeyCodePresets
{
    [Serializable]
    public struct KeyAction_KeyData: IEquatable<KeyAction_KeyData>
    {
        [SerializeField]
        [XmlElement]
        public KeyCode KeyCode;
        [SerializeField]
        [XmlElement]
        public KeyState KeyState;
        [SerializeField]
        [XmlElement]
        public bool AllowContainsState;

        public KeyAction_KeyData(KeyCode key_code, KeyState key_state, bool allow_contains_state)
        {
            KeyCode = key_code;
            KeyState = key_state;
            AllowContainsState = allow_contains_state;
        }

        public bool Equals(KeyAction_KeyData other)
        {
            return (KeyCode == other.KeyCode) && ((AllowContainsState && ((other.KeyState & KeyState) == KeyState)) || (!AllowContainsState && (KeyState == other.KeyState)));
        }
    }
}