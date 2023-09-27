using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Main.Events;
using Main.Managers.KeyboardEvents;
using System;

namespace Main.Managers
{

    public class KeyboardManager : CachedMonoBehaviour, IPriorityObservable<IKeyHandler>
    {
        public float KeyRepeatDelay { get; set; } = 1f;
        public float KeyRepeatInterval { get; set; } = 0.5f;

        public IKeyHandler[] INITIAL_Observables = null;
        public KeyCode[] INITIAL_MonitoredKeyCodes = null;

        protected PriorityObservable<IKeyHandler, int> iObservable = new PriorityObservable<IKeyHandler, int>();
        protected Dictionary<KeyCode, KeyCode_Data> iKeyData = new Dictionary<KeyCode, KeyCode_Data>();
        protected Dictionary<KeyCode, KeyCode_Data> iKeyDataChanges = new Dictionary<KeyCode, KeyCode_Data>(Enum.GetValues(typeof(KeyCode)).Length);

        protected static KeyboardManager iInstance = null;
        public static KeyboardManager Instance
        {
            get
            {
                if (iInstance == null)
                {
                    iInstance = FindObjectOfType<KeyboardManager>();
                }

                if (iInstance == null)
                    throw new System.NullReferenceException($"Could not find a {nameof(KeyboardManager)}");

                if (Application.isPlaying)
                    DontDestroyOnLoad(iInstance);

                return iInstance;
            }
        }

        public struct KeyCode_Data
        {
            public KeyCode KeyCode;
            public KeyState KeyState;
            public float TickBuffer;

            public KeyCode_Data(KeyCode key)
            {
                KeyCode = key;
                KeyState = KeyState.Unknown;
                TickBuffer = 0f;
            }
        }

        public PriorityObservable<IKeyHandler, int> Observables { get => iObservable; }
        public Dictionary<KeyCode, KeyCode_Data> KeyData { get => KeyData; }

        public bool AddObserver(IKeyHandler observer)
        {
            return iObservable.AddObserver(observer);
        }

        public bool RemoveObserver(IKeyHandler observer)
        {
            return iObservable.RemoveObserver(observer);
        }

        public bool AddMonitoredKey(KeyCode key)
        {
            if (iKeyData.ContainsKey(key)) return false;

            iKeyData.Add(key, new KeyCode_Data(key)); 
            return true;
        }

        public bool RemoveMonitoredKey(KeyCode key)
        {
            return iKeyData.Remove(key);
        }

        public void SendKey(KeyCode key_code, KeyState state)
        {
            LinkedListNode<IKeyHandler> node = iObservable.ObserverList.First;

            while (node != null)
            {
                try
                {
                    if (!node.Value.OnKeyState(key_code, state))
                        break;
                }
                catch (Exception e)
                {
                    GLog.LogException(e);
                }

                node = node.Next;
            }
        }

        protected void MonitorKeys()
        {
            iKeyDataChanges.Clear();
            foreach (KeyCode_Data value in iKeyData.Values)
            {
                bool mod_flag = false;
                KeyCode key_code = value.KeyCode;
                KeyCode_Data key_data = value;

                try
                {
                    if (Input.GetKeyDown(key_code))
                    {
                        key_data.KeyState = KeyState.Down;
                        key_data.TickBuffer = Time.time;
                        mod_flag = true;
                        SendKey(key_code, key_data.KeyState);
                    }
                }
                finally
                {
                }

                try
                {
                    if (Input.GetKeyUp(key_code))
                    {
                        key_data.KeyState = KeyState.Up;
                        mod_flag = true;
                        SendKey(key_code, key_data.KeyState);
                    }
                }
                finally
                {
                }

                try
                {

                    if ((key_data.KeyState & KeyState.Down) == KeyState.Down)
                    {
                        if ((((key_data.KeyState & KeyState.Repeat) == KeyState.Repeat) && (Time.time - key_data.TickBuffer >= KeyRepeatInterval)) ||
                             (Time.time - key_data.TickBuffer >= KeyRepeatDelay))
                        {
                            if ((key_data.KeyState & KeyState.Repeat) != KeyState.Repeat) key_data.KeyState = KeyState.Repeat;

                            key_data.TickBuffer = Time.time;
                            mod_flag = true;
                            SendKey(key_code, key_data.KeyState);
                        }
                    }
                }
                finally
                {
                }

                if (mod_flag)
                {
                    iKeyDataChanges.Add(key_code, key_data);
                }
            }

            foreach (KeyValuePair<KeyCode, KeyCode_Data> pair in iKeyDataChanges)
            {
                iKeyData[pair.Key] = pair.Value;
            }
        }

        public void Update()
        {
            MonitorKeys();
        }

        protected override void Start()
        {
            base.Start();
            if (INITIAL_Observables != null) foreach (IKeyHandler observable in INITIAL_Observables) { AddObserver(observable); }
            if (INITIAL_MonitoredKeyCodes != null) foreach (KeyCode key_code in INITIAL_MonitoredKeyCodes) { AddMonitoredKey(key_code); }
        }
    }
        
}