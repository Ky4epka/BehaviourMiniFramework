using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Main;
using Main.Objects;
using Main.Objects.Behaviours;
using Main.Objects.Behaviours.Attributes;


namespace Main.Aggregator.Events.Managers.MapCameraManager
{
    public sealed class TargetMapProperty: SharedPropertyEvent<Main.Map>
    {

    }

    public sealed class ScaleProperty: SharedPropertyEvent<float>
    {

    }

    public sealed class ControllingCameraProperty: SharedPropertyEvent<Camera>
    {

    }

    public sealed class PositionProperty: SharedPropertyEvent<Vector2>
    {

    }

    public sealed class FollowBodyProperty : SharedPropertyEvent<GameObject>
    {

    }

    public sealed class WorldBoundsProperty: SharedPropertyEvent<Rect>
    {

    }

    public sealed class WorldBoundsSpacingProperty: SharedPropertyEvent<Vector3>
    {

    }
}

namespace Main.Aggregator.Properties.Managers.MapCameraManager
{
    public sealed class TargetMapProperty : SharedPropertyReference<Main.Map, Aggregator.Events.Managers.MapCameraManager.TargetMapProperty>
    {
        public override string GroupTag => "Common";

        public override string SharedName => "TargetMap";
    }

    public sealed class ScaleProperty : SharedProperty<float, Events.Managers.MapCameraManager.ScaleProperty>
    {
        public override string GroupTag => "Common";

        public override string SharedName => "Scale";
    }

    public sealed class ControllingCameraProperty : SharedPropertyReference<Camera, Events.Managers.MapCameraManager.ControllingCameraProperty>
    {
        public override string GroupTag => "Common";
        public override string SharedName => "ControllingCamera";
    }

    public sealed class PositionProperty : SharedProperty<Vector2, Events.Managers.MapCameraManager.PositionProperty>
    {
        public override string GroupTag => "Common";
        public override string SharedName => "Position";
    }

    public sealed class FollowBodyProperty : SharedPropertyReference<GameObject, Events.Managers.MapCameraManager.FollowBodyProperty>
    {
        public override string GroupTag => "Common";
        public override string SharedName => "FollowBody";
    }

    public sealed class WorldBoundsProperty : SharedProperty<Rect, Events.Managers.MapCameraManager.WorldBoundsProperty>
    {
        public override string GroupTag => "Common";
        public override string SharedName => "WorldBounds";
    }


    public sealed class WorldBoundsSpacingProperty : SharedProperty<Vector3, Events.Managers.MapCameraManager.WorldBoundsSpacingProperty>
    {
        public override string GroupTag => "Common";
        public override string SharedName => "WorldBoundsSpacing";
    }
}

namespace Main.Managers
{
    public class MapCameraManager : ObjectBehavioursBase
    {
        [SharedProperty]
        public Aggregator.Properties.Managers.MapCameraManager.TargetMapProperty TargetMap { get; protected set; }
        [SharedProperty]
        public Aggregator.Properties.Managers.MapCameraManager.ControllingCameraProperty ControllingCamera { get; protected set; }
        [SharedProperty]
        public Aggregator.Properties.Managers.MapCameraManager.FollowBodyProperty FollowBody { get; protected set; }
        [SharedProperty]
        public Aggregator.Properties.Managers.MapCameraManager.PositionProperty Position { get; protected set; }
        [SharedProperty]
        public Aggregator.Properties.Managers.MapCameraManager.ScaleProperty Scale { get; protected set; }
        [SharedProperty]
        public Aggregator.Properties.Managers.MapCameraManager.WorldBoundsProperty WorldBounds { get; protected set; }
        [SharedProperty]
        public Aggregator.Properties.Managers.MapCameraManager.WorldBoundsSpacingProperty WorldBoundsSpacing { get; protected set; }



        protected static MapCameraManager iInstance = null;

        [SharedPropertyHandler(typeof(Aggregator.Properties.Managers.MapCameraManager.ScaleProperty), RehandleOnEnabled = true)]
        public bool ScalePropertyHandler(ISharedProperty eventData, float oldValue, ref float newValue)
        {
            if (Mathf.Abs(newValue) <= 0.1f)
                newValue = 0.1f;

            return true;
        }

        public static MapCameraManager Instance
        {
            get
            {
                if (iInstance == null)
                {
                    iInstance = GameObject.FindObjectOfType<MapCameraManager>();

                    if (iInstance == null)
                        throw new System.NullReferenceException($"Could not find a {nameof(MapCameraManager)}");
                }

                DontDestroyOnLoad(iInstance);

                return iInstance;
            }
        }

        protected void Recalc()
        {
            if ((TargetMap.Value == null) ||
                (ControllingCamera.Value == null))
                return;

            Rect bounds = TargetMap.Value.Common.WorldBounds.Value;
            float ortho = bounds.height / 2f - (bounds.width / (MathKit.NumbersEquals(bounds.height, 0) ? 1f : bounds.height));
            Vector3 pos = Position.Value;

            if (FollowBody.Value != null)
                pos = FollowBody.Value.transform.position;

            pos = MathKit.EnsureVectorRectRange(pos, TargetMap.Value.Common.WorldBounds.Value);
            pos.z = -5;
            ortho = ortho * Scale.Value;
            if (MathKit.NumbersEquals(ortho, 0))
                ortho = 1f;
                
            ControllingCamera.Value.orthographicSize = ortho;
            ControllingCamera.Value.transform.position = pos;//
        }

        protected override void Awake()
        {
            base.Awake();
            WorldBounds.ReadonlyValueProvider = () => {
                Rect result = Rect.zero;

                if (ControllingCamera.Value != null)
                {
                    result.min = ControllingCamera.Value.ViewportToWorldPoint(ControllingCamera.Value.rect.min) - WorldBoundsSpacing.Value;
                    result.max = ControllingCamera.Value.ViewportToWorldPoint(ControllingCamera.Value.rect.max) + WorldBoundsSpacing.Value;
                }

                return result;
            };
        }

        public void LateUpdate()
        {
            Recalc();
        }

        protected override void OnValidate()
        {
            base.OnValidate();
                       
            if (ControllingCamera != null && !ControllingCamera.Value)
                ControllingCamera.Value = Camera.main;
        }

    }

}