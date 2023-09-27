using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Main;
using Main.Objects;
using Main.Objects.Behaviours;
using Main.Objects.Behaviours.Attributes;
using Main.Events;



namespace Main.Aggregator.Events.Behaviours.Tanks
{
    public class GunObjectProperty : SharedPropertyEvent<GameObject>
    {
    }
}
namespace Main.Aggregator.Properties.Behaviours.Tanks
{
    public class GunObjectProperty : SharedPropertyReference<GameObject, Main.Aggregator.Events.Behaviours.Tanks.GunObjectProperty>
    {
        public override string GroupTag => "Tank";
        public override string SharedName => "GunObject";
    }
}


namespace Main.Aggregator.Events.Behaviours.Tanks
{
    public class TracksObjectProperty : SharedPropertyEvent<GameObject>
    {
    }
}
namespace Main.Aggregator.Properties.Behaviours.Tanks
{
    public class TracksObjectProperty : SharedPropertyReference<GameObject, Main.Aggregator.Events.Behaviours.Tanks.TracksObjectProperty>
    {
        public override string GroupTag => "Tank";
        public override string SharedName => "TracksObject";
    }
}


namespace Main.Aggregator.Events.Behaviours.Tanks
{
    public class BodySpriteProperty : SharedPropertyEvent<Sprite>
    {
    }
}
namespace Main.Aggregator.Properties.Behaviours.Tanks
{
    public class BodySpriteProperty : SharedPropertyReference<Sprite, Main.Aggregator.Events.Behaviours.Tanks.BodySpriteProperty>
    {
        public override string GroupTag => "Tank";
        public override string SharedName => "BodySprite";
    }
}


namespace Main.Aggregator.Events.Behaviours.Tanks
{
    public class GunSpriteProperty : SharedPropertyEvent<Sprite>
    {
    }
}
namespace Main.Aggregator.Properties.Behaviours.Tanks
{
    public class GunSpriteProperty : SharedPropertyReference<Sprite, Main.Aggregator.Events.Behaviours.Tanks.GunSpriteProperty>
    {
        public override string GroupTag => "Tank";
        public override string SharedName => "GunSprite";
    }
}


namespace Main.Aggregator.Events.Behaviours.Tanks
{
    public class TracksSpriteProperty : SharedPropertyEvent<Sprite>
    {
    }
}
namespace Main.Aggregator.Properties.Behaviours.Tanks
{
    public class TracksSpriteProperty : SharedPropertyReference<Sprite, Main.Aggregator.Events.Behaviours.Tanks.TracksSpriteProperty>
    {
        public override string GroupTag => "Tank";
        public override string SharedName => "TracksSprite";
    }
}


namespace Main.Aggregator.Events.Behaviours.Tanks
{
    public class MoveAnimationProperty : SharedPropertyEvent<Helpers.Animation.AnimationEventInfo>
    {
    }
}
namespace Main.Aggregator.Properties.Behaviours.Tanks
{
    public class MoveAnimationProperty : SharedProperty<Helpers.Animation.AnimationEventInfo, Main.Aggregator.Events.Behaviours.Tanks.MoveAnimationProperty>
    {
        public override string GroupTag => "Tank";
        public override string SharedName => "MoveAnimation";
    }
}


namespace Main.Aggregator.Events.Behaviours.Tanks
{
    public class IdleAnimationProperty : SharedPropertyEvent<Helpers.Animation.AnimationEventInfo>
    {
    }
}
namespace Main.Aggregator.Properties.Behaviours.Tanks
{
    public class IdleAnimationProperty : SharedProperty<Helpers.Animation.AnimationEventInfo, Main.Aggregator.Events.Behaviours.Tanks.IdleAnimationProperty>
    {
        public override string GroupTag => "Tank";
        public override string SharedName => "IdleAnimation";
    }
}

namespace Main.Objects.Behaviours.Tanks
{
    public class TankBaseBehaviour : ObjectBehavioursBase
    {

        [SharedProperty]
        public Main.Aggregator.Properties.Behaviours.Tanks.GunObjectProperty GunObject { get; protected set; }

        [SharedProperty]
        public Main.Aggregator.Properties.Behaviours.Tanks.TracksObjectProperty TracksObject { get; protected set; }

        [SharedProperty]
        public Main.Aggregator.Properties.Tools.SpriteRendererWrapper.SpriteProperty Sprite { get; protected set; }
        [SharedProperty]
        public Main.Aggregator.Properties.Tools.AnimatorWrapper.AnimatorProperty Animator { get; protected set; }

        [SharedProperty]
        public Main.Aggregator.Properties.Behaviours.Tanks.BodySpriteProperty BodySprite { get; protected set; }

        [SharedProperty]
        public Main.Aggregator.Properties.Behaviours.Tanks.GunSpriteProperty GunSprite { get; protected set; }

        [SharedProperty]
        public Main.Aggregator.Properties.Behaviours.Tanks.TracksSpriteProperty TracksSprite { get; protected set; }

        [SharedProperty]
        public Main.Aggregator.Properties.Behaviours.Tanks.MoveAnimationProperty MoveAnimation { get; protected set; }
        [SharedProperty]
        public Main.Aggregator.Properties.Behaviours.Tanks.IdleAnimationProperty IdleAnimation { get; protected set; }
        [SharedProperty]
        public Main.Aggregator.Properties.Behaviours.Movable.IsMovingProperty IsMoving { get; set; }

        protected SpriteRenderer iGunRenderer = null;
        protected SpriteRenderer iTracksRenderer = null;

        [SharedPropertyViewer(typeof(Main.Aggregator.Properties.Behaviours.Tanks.BodySpriteProperty))]
        public void BodySpritePropertyViewer(Main.Aggregator.Events.Behaviours.Tanks.BodySpriteProperty eventData)
        {
            Sprite.Value = eventData.PropertyValue;
        }

        [SharedPropertyViewer(typeof(Main.Aggregator.Properties.Behaviours.Tanks.GunSpriteProperty))]
        public void GunSpritePropertyViewer(Main.Aggregator.Events.Behaviours.Tanks.GunSpriteProperty eventData)
        {
            if (iGunRenderer)
                iGunRenderer.sprite = eventData.PropertyValue;
        }

        [SharedPropertyViewer(typeof(Main.Aggregator.Properties.Behaviours.Tanks.TracksSpriteProperty))]
        public void TracksSpritePropertyViewer(Main.Aggregator.Events.Behaviours.Tanks.TracksSpriteProperty eventData)
        {
            if (iTracksRenderer)
                iTracksRenderer.sprite = eventData.PropertyValue;
        }

        [SharedPropertyViewer(typeof(Main.Aggregator.Properties.Behaviours.Tanks.GunObjectProperty))]
        public void GunObjectPropertyViewer(Main.Aggregator.Events.Behaviours.Tanks.GunObjectProperty eventData)
        {
            iGunRenderer = eventData.PropertyValue?.GetComponent<SpriteRenderer>();
            GunSprite.DirtyValue();
        }

        [SharedPropertyViewer(typeof(Main.Aggregator.Properties.Behaviours.Tanks.TracksObjectProperty))]
        public void TracksObjectPropertyViewer(Main.Aggregator.Events.Behaviours.Tanks.TracksObjectProperty eventData)
        {
            iTracksRenderer = eventData.PropertyValue?.GetComponent<SpriteRenderer>();
            TracksSprite.DirtyValue();

        }

        [SharedPropertyViewer(typeof(Aggregator.Properties.Tools.AnimatorWrapper.ControllerProperty))]
        public void AnimatorControllerViewer(Aggregator.Events.Tools.AnimatorWrapper.ControllerProperty eventData)
        {//
            IsMoving.EventValueFor((System.Action<Aggregator.Events.Behaviours.Movable.IsMovingProperty>)IsMovingPropertyViewer);
        }

        [SharedPropertyViewer(typeof(Aggregator.Properties.Behaviours.Movable.IsMovingProperty))]
        public void IsMovingPropertyViewer(Aggregator.Events.Behaviours.Movable.IsMovingProperty eventData)
        {
            if (eventData.PropertyValue)
                Event<Aggregator.Events.Tools.AnimatorWrapper.PlayEvent>(Container).Invoke(MoveAnimation.Value);
            else
                Event<Aggregator.Events.Tools.AnimatorWrapper.PlayEvent>(Container).Invoke(IdleAnimation.Value);
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            GameObject gun = transform.Find("Gun")?.gameObject;
            GameObject tracks = transform.Find("Tracks")?.gameObject;

            if (!gun)
            {
                gun = new GameObject("Gun", new System.Type[] { typeof(SpriteRenderer) } );
                gun.transform.parent = transform;
            }

            if (!tracks)
            {
                tracks = new GameObject("Tracks", new System.Type[] { typeof(SpriteRenderer) });
                tracks.transform.parent = transform;
            }

            GunObject.Value = gun;
            TracksObject.Value = tracks;
        }

        protected override bool DoEnable()
        {
            if (!base.DoEnable())
                return false;

            if (GunObject.Value)
                GunObject.Value.SetActive(true);

            if (TracksObject.Value)
                TracksObject.Value.SetActive(true);

            return true;
        }

        protected override bool DoDisable()
        {
            if (!base.DoEnable())
                return false;

            if (GunObject.Value)
                GunObject.Value.SetActive(false);

            if (TracksObject.Value)
                TracksObject.Value.SetActive(false);

            return true;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (GunObject.Value)
#if UNITY_EDITOR
                GameObject.DestroyImmediate(GunObject.Value);
#else
                GameObject.Destroy(GunObject.Value);
#endif

            if (TracksObject.Value)
#if UNITY_EDITOR
                GameObject.DestroyImmediate(TracksObject.Value);
#else
                GameObject.Destroy(TracksObject.Value);
#endif
        }
    }

}
