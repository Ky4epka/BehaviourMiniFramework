
using Main;
using Main.Objects;
using Main.Objects.Behaviours;
using Main.Objects.Behaviours.Attributes;
using UnityEngine;

namespace Main.Aggregator.Events.Behaviours.Movable.RigidbodyMovable
{
    public class Rigidbody2DProperty : SharedPropertyEvent<UnityEngine.Rigidbody2D>
    {
    }
}
namespace Main.Aggregator.Properties.Behaviours.Movable.RigidbodyMovable
{
    public class Rigidbody2DProperty : SharedPropertyReference<UnityEngine.Rigidbody2D, Main.Aggregator.Events.Behaviours.Movable.RigidbodyMovable.Rigidbody2DProperty>
    {
        public override string GroupTag => "Common";
        public override string SharedName => "Rigidbody";
    }
}

namespace Main.Objects.Behaviours.Movable
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class RigidbodyMotionBehaviour : DynamicMotionBehaviour
    {
        [SharedProperty(InjectComponentToValue = typeof(Rigidbody2D))]
        public Aggregator.Properties.Behaviours.Movable.RigidbodyMovable.Rigidbody2DProperty RigidbodyProperty { get; protected set; }

        protected void FixedUpdate()
        {
            if (RigidbodyProperty.Value)
                RigidbodyProperty.Value.velocity = Vector2.zero;
        }

        protected override void ApplyPosition(Vector2 position)
        {
            if (RigidbodyProperty.Value && Application.isPlaying)
                RigidbodyProperty.Value.MovePosition(position);
            else
                base.ApplyPosition(position);
        }

        protected override bool DoEnable()
        {
            if (!base.DoEnable())
                return false;

            if (RigidbodyProperty.Value)
                RigidbodyProperty.Value.simulated = true;

            return true;
        }

        protected override bool DoDisable()
        {
            if (!base.DoDisable())
                return false;

            if (RigidbodyProperty.Value)
                RigidbodyProperty.Value.simulated = false;

            return true;
        }

        protected override void OnDestroy()
        {            
            // Detects indirect destroying state of game object
            // This solutions needs for avoid exception: Destroying object multiple times. Don't use DestroyImmediate on the same object in OnDisable or OnDestroy.
            if (gameObject.activeInHierarchy &&
                RigidbodyProperty.Value)
#if UNITY_EDITOR
                GameObject.DestroyImmediate(RigidbodyProperty.Value);
#else
                GameObject.Destroy(Animator.Value);
#endif

            base.OnDestroy();
        }
    }
}
