using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Main.Objects;
using Main.Events;



namespace Main
{

    public class Map: BehaviourContainer
    {
        [BehaviourProperty]
        public Map_Common Common { get; private set; }
    }

}
