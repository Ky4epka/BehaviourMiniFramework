using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using UnityEngine;
using Main.Managers.KeyboardEvents;
using System.Xml.Serialization;
using System.Runtime.InteropServices;

namespace Main.Events.KeyCodePresets
{

    [Serializable]
    [XmlRoot("KeyCodePreset")]
    [XmlInclude(typeof(KeyActionHandlerBase))]
    public class KeyActionPreset : KeyActionHandlerBase, IKeyHandler
    {

        /// <summary>
        /// When higher a value then higher a priority
        /// </summary>
        [SerializeField]
        [XmlElement("HandlePriority")]
        public int Priority { get; set; } = 0;

        [NonSerialized]
        [XmlIgnore]
        protected IPriorityObservable<IKeyHandler> iObservable = null;

        public KeyActionPreset()
        {

        }

        public KeyActionPreset(int priority)
        {
            Priority = priority;
        }

        public KeyActionPreset(IPriorityObservable<IKeyHandler> observable, int priority)
        {
            Priority = priority;
            Observable = observable;
        }

        [XmlIgnore]
        public IPriorityObservable<IKeyHandler> Observable
        {
            get => iObservable;
            set
            {
                if (iObservable != null) iObservable.RemoveObserver(this);
                iObservable = value;
                if (iObservable != null) iObservable.AddObserver(this);
            }
        }

        public bool OnKeyState(KeyCode key_code, KeyState key_state)
        {
            return HandleKey(key_code, key_state);
        }

        public override void Dispose()
        {
            Observable = null;
            base.Dispose();
        }

    }
}