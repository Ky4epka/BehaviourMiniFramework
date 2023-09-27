using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.IO;
using System.Xml.Serialization;
using System.Collections;
using System.Runtime.InteropServices;
using Main.Managers.KeyboardEvents;

namespace Main.Events.KeyCodePresets
{
    [Serializable]
    public class KeyActionBase : IKeyAction
    {
        [SerializeField]
        [XmlElement("description")]
        public virtual string Description { get ; set ; }
        [SerializeField]
        [XmlElement("name")]
        public virtual string Name { get => GetName(); set { if (!SetName(value)) throw new Exception("Unable set name '" + value + "' to action for a reason: action with this name already exists"); } }
       
        [XmlIgnore]
        public virtual IEnumerable<KeyAction_KeyData> Keys 
        { 
            get => iBindedKeys;
            set
            {
                iBindedKeys.Clear();

                foreach (KeyAction_KeyData key_data in value)
                {
                    if (!BindKeyData(key_data))
                        throw new Exception("Can not bind key data '" + key_data.ToString() + " for a reason: key data already used");
                };
            }
        }

        [SerializeField]
        [XmlArray("KeyConfigs")]
        [XmlArrayItem("KeyConfig")]
        public virtual KeyAction_KeyData[] KeysSerialized
        {
            get
            {
                return iBindedKeys.ToArray();
            }
            set
            {
                Keys = value;
            }
        }


        [XmlIgnore]
        public virtual KeyActionCondition Condition { get; set; } = null;
        [XmlIgnore]
        public virtual Func<IKeyAction, KeyCode, KeyState, bool> Action { get; set; } = null;
        [XmlIgnore]
        public object Data { get; set; } = null;

        [NonSerialized]
        [XmlIgnore]
        protected IKeyActionHandler iHandler = null;
        [NonSerialized]
        [XmlIgnore]
        protected string iName = "";
        [NonSerialized]
        [XmlIgnore]
        protected List<KeyAction_KeyData> iBindedKeys = new List<KeyAction_KeyData>();

        public KeyActionBase()
        {
            Initialize();
        }

        public KeyActionBase(IKeyActionHandler handler)
        {
            Initialize();
            Handler = handler;
        }

        [XmlIgnore]
        public virtual IKeyActionHandler Handler 
        { 
            get => iHandler; 
            set
            {
                if (iHandler != null) iHandler.RemoveAction(this);

                iHandler = value;

                if (iHandler != null) iHandler.AddAction(this);
            }
        }

        public virtual bool BindKeyData(KeyAction_KeyData key_data)
        {
            if (((iHandler != null) && (iHandler.KeyAlreadyBinded(key_data))) || IsKeyDataBinded(key_data)) return false;

            iBindedKeys.Add(key_data);
            return true;
        }

        public virtual string GetName()
        {
            return iName;
        }

        public virtual bool IsKeyDataBinded(KeyAction_KeyData key_data)
        {
            return IndexOfKeyData(key_data) != -1;
        }

        public virtual bool SetName(string name)
        {
            if ((iHandler != null) && (iHandler.GetAction(name) != null)) return false; 

            iName = name;
            return true;
        }

        public virtual bool UnbindKeyData(KeyAction_KeyData key_data)
        {
            int index = IndexOfKeyData(key_data);

            if (index != -1)
            {
                iBindedKeys.RemoveAt(index);
                return true;
            }

            return false;
        }

        public virtual void ClearKeyData()
        {
            iBindedKeys.Clear();
        }

        public virtual bool Invoke(KeyCode key_code, KeyState key_state)
        {
            if (((Condition == null) || Condition(this, key_code, key_state)) && (Action != null))
            {
                return Action(this, key_code, key_state);
            }

            return false;
        }

        public virtual bool ActionCap(IKeyAction sender, KeyCode key_code, KeyState key_state)
        {
            return true;
        }

        public virtual bool ConditionCap(IKeyAction sender, KeyCode key_code, KeyState key_state)
        {
            return true;
        }

        protected virtual int IndexOfKeyData(KeyAction_KeyData key_data)
        {
            for (int i=0; i<iBindedKeys.Count; i++)
            {
                if (iBindedKeys[i].Equals(key_data))
                    return i;
            }

            return -1;
        }

        public virtual object Clone()
        {
            KeyActionBase obj = new KeyActionBase();
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
            if (source is KeyActionBase)
            {
                KeyActionBase cast_obj = source as KeyActionBase;
                Description = cast_obj.Description;
                Name = cast_obj.Name;
                Keys = cast_obj.Keys;
            }
            else
                throw new InvalidCastException("Can not assign source of type '" + source.GetType().FullName + "'");
        }

        public virtual void Dispose()
        {
            ClearKeyData();
            Handler = null;
        }

        protected virtual void Initialize()
        {
            Action = Action ?? ActionCap;
            Condition = Condition ?? ConditionCap;
        }
    }
}