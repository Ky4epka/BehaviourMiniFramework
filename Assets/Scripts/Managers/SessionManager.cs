using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Main.Managers
{

    public class SessionManager : MonoBehaviour
    {
        public static Guid SessionId { get; } = Guid.NewGuid();
        public static DateTime SessionStartTime { get; } = DateTime.Now;

    }

}