using System.Collections.Generic;
using UnityEngine;
using Main.Managers.KeyboardEvents;
using System;
using System.Xml.Serialization;
using System.Runtime.InteropServices;

namespace Main.Events.KeyCodePresets
{
    [Serializable]
    [XmlRoot("ActionMap")]
    public class KeyActionHandlerBase : IKeyActionHandler, IDisposable
    {
        [XmlIgnore]
        public NotifyEvent_2P<IKeyActionHandler, IKeyAction> OnAddAction { get; } = new NotifyEvent_2P<IKeyActionHandler, IKeyAction>();
        [XmlIgnore]
        public NotifyEvent_2P<IKeyActionHandler, IKeyAction> OnRemoveAction { get; } = new NotifyEvent_2P<IKeyActionHandler, IKeyAction>();

        [SerializeField]
        [XmlElement("name")]
        public string Name { get => GetName(); set { if (!SetName(value)) throw new Exception("Could not change name to action preset to '" + value + "' for a reason: preset with this name already exists"); } }

        [NonSerialized]
        [XmlIgnore]
        protected string iName = "";

        [SerializeField]
        [XmlArray("actions")]
        [XmlArrayItem("action", typeof(KeyActionBase))]
        public KeyActionBase [] Actions 
        { 
            get
            {
                List<KeyActionBase> list = new List<KeyActionBase>();

                foreach (IKeyAction act in iActions)
                {
                    list.Add(act as KeyActionBase);
                }

                return list.ToArray();
            }
            set
            {
                Clear();

                foreach (IKeyAction action in value)
                {
                    AddAction(action);
                }
            }
        }

        [NonSerialized]
        [XmlIgnore]
        protected List<IKeyAction> iActions = new List<IKeyAction>();

        public virtual void AddAction(IKeyAction action)
        {
            if (action == null)
                throw new System.ArgumentNullException("action");

            if (GetAction(action.GetName()) != null)
            {
                throw new System.Exception("Action with '" + action.GetName() + "' name is already exists.");
            }

            foreach (KeyAction_KeyData key in action.Keys)
            {
                if (KeyAlreadyBinded(key))
                    throw new System.Exception("Can not add action with '" + key + "' key for a reason: key already binded.");
            }

            iActions.Add(action);
            OnAddAction.Invoke(this, action);
        }

        public virtual bool RemoveAction(IKeyAction action)
        {
            bool result = iActions.Remove(action);
            OnRemoveAction.Invoke(this, action);
            return result;
        }

        public virtual bool RemoveAction(string name)
        {
            for (int i=0; i< iActions.Count; i++)
                if (iActions[i].GetName().Equals(name))
                {
                    IKeyAction temp = iActions[i];
                    iActions.RemoveAt(i);
                    OnRemoveAction.Invoke(this, temp);
                    return true;
                }

            return false;
        }

        public virtual void Clear()
        {
            while (iActions.Count > 0)
            {
                RemoveAction(iActions[0]);
            }
        }

        public virtual bool KeyAlreadyBinded(KeyAction_KeyData key_data)
        {
            return GetAction(key_data) != null;
        }

        public virtual bool KeyAlreadyBindedWith(KeyAction_KeyData key_data, IKeyAction action)
        {
            return action.IsKeyDataBinded(key_data);
        }

        public virtual IKeyAction GetAction(string name)
        {
            for (int i = 0; i < iActions.Count; i++)
                if (iActions[i].GetName().Equals(name))
                    return iActions[i];

            return null;
        }

        public virtual IKeyAction GetAction(KeyAction_KeyData key_data)
        {
            for (int i = 0; i < iActions.Count; i++)
                if (iActions[i].IsKeyDataBinded(key_data))
                    return iActions[i];

            return null;
        }

        public virtual bool HandleKey(KeyCode key_code, KeyState key_state)
        {
            IKeyAction action = GetAction(new KeyAction_KeyData(key_code, key_state, false));

            if (action != null) return action.Invoke(key_code, key_state);

            return false;
        }

        public virtual string GetName()
        {
            return iName;
        }

        public virtual bool SetName(string name)
        {
            iName = name;
            return true;
        }

        public virtual void Dispose()
        {
            Clear();
        }

        public virtual object Clone()
        {
            KeyActionHandlerBase obj = new KeyActionHandlerBase();
            try
            {
                obj.Assign(this);
            }
            catch (Exception e)
            {
                obj = null;
                throw e;
            }

            return obj;
        }

        public virtual void Assign(IAssignable source)
        {
            if (source is KeyActionHandlerBase)
            {
                KeyActionHandlerBase cast_obj = source as KeyActionHandlerBase;
                Name = cast_obj.Name;
                Actions = cast_obj.Actions;
            }
            else if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            else
            {
                throw new InvalidCastException("Can not assign source of type '" + source.GetType().FullName + "'");
            }
        }
    }
}