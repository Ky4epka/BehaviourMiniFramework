using System;
using System.Collections.Generic;
using UnityEngine;

namespace Main.Managers.KeyboardEvents
{
    public interface IKeyHandler : IPriorityObserver<int>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key_code"></param>
        /// <param name="key_state"></param>
        /// <returns>False for stop key state propagation</returns>
        bool OnKeyState(KeyCode key_code, KeyState key_state);
    }
}
