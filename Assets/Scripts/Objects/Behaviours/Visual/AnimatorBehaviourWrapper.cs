using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Main;
using Main.Objects;
using Main.Objects.Behaviours;
using Main.Objects.Behaviours.Attributes;
using Main.Events;


namespace Main.Aggregator.Helpers.Animation
{
    [System.Serializable]
    public struct AnimationEventInfo: System.IEquatable<AnimationEventInfo>
    {
        [SerializeField]
        public string AnimationFullName;
        [SerializeField]
        public int LayerId;
        [SerializeField]
        public float normalizedStartTime;

        public bool Equals(AnimationEventInfo other)
        {
            return GetHashCode() == other.GetHashCode();
        }
    }
}

namespace Main.Aggregator.Events.Tools.AnimatorWrapper
{
    public class AnimatorProperty : SharedPropertyEvent<Animator>
    {
    }
}
namespace Main.Aggregator.Properties.Tools.AnimatorWrapper
{
    public class AnimatorProperty : SharedPropertyReference<Animator, Main.Aggregator.Events.Tools.AnimatorWrapper.AnimatorProperty>
    {
        public override string GroupTag => "Animation";
        public override string SharedName => "Animator";
    }
}


namespace Main.Aggregator.Events.Tools.AnimatorWrapper
{
    public class ControllerProperty : SharedPropertyEvent<RuntimeAnimatorController>
    {
    }
}
namespace Main.Aggregator.Properties.Tools.AnimatorWrapper
{
    public class ControllerProperty : SharedPropertyReference<RuntimeAnimatorController, Main.Aggregator.Events.Tools.AnimatorWrapper.ControllerProperty>
    {
        public override string GroupTag => "Animation";
        public override string SharedName => "Controller";
    }
}


namespace Main.Aggregator.Events.Tools.AnimatorWrapper
{
    public sealed class PlayEvent: EventDataBase
    {
        public string StateName { get; private set; } = "";
        public int LayerId { get; private set; } = 0;
        public float NormalizedTime { get; private set; } = 0f;

        public void Invoke(Aggregator.Helpers.Animation.AnimationEventInfo eventInfo)
        {
            Invoke(eventInfo.AnimationFullName, eventInfo.LayerId, eventInfo.normalizedStartTime);
        }

        public void Invoke(string stateName, int layerId = 0, float normalizedTime = 0f)
        {
            StateName = stateName;
            LayerId = layerId;
            NormalizedTime = normalizedTime;
            base.Invoke();
        }


    }

    public sealed class StopEvent : EventDataBase
    {
    }
}

namespace Main.Objects.Behaviours.Tools
{
    [RequireComponent(typeof(Animator))]
    [Unique]
    [DisallowMultipleComponent]
    public class AnimatorBehaviourWrapper : ObjectBehavioursBase
    {

        [SharedProperty(InjectComponentToValue = typeof(Animator))]
        public Main.Aggregator.Properties.Tools.AnimatorWrapper.AnimatorProperty Animator { get; protected set; }


        [SharedProperty]
        public Main.Aggregator.Properties.Tools.AnimatorWrapper.ControllerProperty Controller { get; protected set; }

        [SharedPropertyViewer(typeof(Main.Aggregator.Properties.Tools.AnimatorWrapper.ControllerProperty))]
        public void ControllerPropertyViewer(Main.Aggregator.Events.Tools.AnimatorWrapper.ControllerProperty eventData)
        {
            if (Animator.Value)
                Animator.Value.runtimeAnimatorController = eventData.PropertyValue;
        }

        [EnabledStateEvent]
        public void PlayEvent(Main.Aggregator.Events.Tools.AnimatorWrapper.PlayEvent eventData)
        {
            if (Animator.Value)
                Animator.Value.Play(eventData.StateName, eventData.LayerId, eventData.NormalizedTime);
        }

        [EnabledStateEvent]
        public void StopEvent(Main.Aggregator.Events.Tools.AnimatorWrapper.StopEvent eventData)
        {
            if (Animator.Value)
                Animator.Value.StopPlayback();
        }

        protected override bool DoEnable()
        {
            if (!base.DoEnable())
                return false;

            if (Animator.Value != null)
                Animator.Value.enabled = true;

            return true;
        }

        protected override bool DoDisable()
        {
            if (!base.DoDisable())
                return false;

            if (Animator.Value != null)
                Animator.Value.enabled = false;

            return true;
        }

        protected override void OnDestroy()
        {
            // Detects indirect destroying state of game object
            // This solutions needs for avoid exception: Destroying object multiple times. Don't use DestroyImmediate on the same object in OnDisable or OnDestroy.
            if (gameObject.activeInHierarchy &&
                Animator.Value)
#if UNITY_EDITOR
                GameObject.DestroyImmediate(Animator.Value);
#else
                GameObject.Destroy(Animator.Value);
#endif
            
            base.OnDestroy();
        }
    }
}
