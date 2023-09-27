using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main.Managers
{
    public class UIManager : ObjectManager
    {
        protected static UIManager iInstance = null;
        public static UIManager Instance { get => iInstance; set => iInstance = iInstance ?? (iInstance = new UIManager()); }
    }

}