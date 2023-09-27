using System.Collections;
using UnityEngine;
using Main.Objects;
using Main.Objects.Behaviours;
using Main.Objects.Behaviours.Attributes;
using Main.Events;
using Main;


namespace Main.Aggregator.Events.Tools.SpriteRendererWrapper
{
    public class SpriteRendererProperty : SharedPropertyEvent<SpriteRenderer>
    {
    }

}

namespace Main.Aggregator.Properties.Tools.SpriteRendererWrapper
{
    public class SpriteRendererProperty : SharedPropertyReference<SpriteRenderer, Events.Tools.SpriteRendererWrapper.SpriteRendererProperty>
    {
        public override string GroupTag => "2D Rendering";

        public override string SharedName => "SpriteRenderer";
    }

}


namespace Main.Aggregator.Events.Tools.SpriteRendererWrapper
{
    public class SpriteProperty : SharedPropertyEvent<Sprite>
    {
    }
}
namespace Main.Aggregator.Properties.Tools.SpriteRendererWrapper
{
    public class SpriteProperty : SharedPropertyReference<Sprite, Main.Aggregator.Events.Tools.SpriteRendererWrapper.SpriteProperty>
    {
        public override string GroupTag => "2D Rendering";
        public override string SharedName => "Sprite";
    }
}


namespace Main.Aggregator.Events.Tools.SpriteRendererWrapper
{
    public class SortingLayerNameProperty : SharedPropertyEvent<string>
    {
    }
}
namespace Main.Aggregator.Properties.Tools.SpriteRendererWrapper
{
    public class SortingLayerNameProperty : SharedProperty<string, Main.Aggregator.Events.Tools.SpriteRendererWrapper.SortingLayerNameProperty>
    {
        public override string GroupTag => "2D Rendering";
        public override string SharedName => "SortingLayerName";
    }
}


namespace Main.Aggregator.Events.Tools.SpriteRendererWrapper
{
    public class LayerOrderProperty : SharedPropertyEvent<int>
    {
    }
}
namespace Main.Aggregator.Properties.Tools.SpriteRendererWrapper
{
    public class LayerOrderProperty : SharedProperty<int, Main.Aggregator.Events.Tools.SpriteRendererWrapper.LayerOrderProperty>
    {
        public override string GroupTag => "2D Rendering";
        public override string SharedName => "LayerOrder";
    }
}

namespace Main.Objects.Behaviours.Tools
{
    /// <summary>
    /// Behaviour wrapper for Unity Sprite Renderer
    /// </summary>
    [RequireComponent(typeof(BehaviourContainer), typeof(SpriteRenderer))]
    [DisallowMultipleComponent]
    public class SpriteRendererBehaviourWrapper : ObjectBehavioursBase
    {
        [SharedProperty(InjectComponentToValue = typeof(SpriteRenderer))]
        public Aggregator.Properties.Tools.SpriteRendererWrapper.SpriteRendererProperty SpriteRenderer { get; protected set; }


        [SharedProperty]
        public Main.Aggregator.Properties.Tools.SpriteRendererWrapper.SpriteProperty Sprite { get; protected set; }

        [SharedProperty]
        public Main.Aggregator.Properties.Tools.SpriteRendererWrapper.SortingLayerNameProperty SortingLayerName { get; protected set; }


        [SharedProperty]
        public Main.Aggregator.Properties.Tools.SpriteRendererWrapper.LayerOrderProperty LayerOrder { get; protected set; }

        [SharedPropertyViewer(typeof(Main.Aggregator.Properties.Tools.SpriteRendererWrapper.SpriteProperty))]
        public void SpritePropertyViewer(Main.Aggregator.Events.Tools.SpriteRendererWrapper.SpriteProperty eventData)
        {
            if (SpriteRenderer.Value)
                SpriteRenderer.Value.sprite = eventData.PropertyValue;
        }

        [SharedPropertyViewer(typeof(Main.Aggregator.Properties.Tools.SpriteRendererWrapper.SortingLayerNameProperty))]
        public void SortingLayerNamePropertyViewer(Main.Aggregator.Events.Tools.SpriteRendererWrapper.SortingLayerNameProperty eventData)
        {
            if (SpriteRenderer.Value)
                SpriteRenderer.Value.sortingLayerName = eventData.PropertyValue;
        }

        [SharedPropertyViewer(typeof(Main.Aggregator.Properties.Tools.SpriteRendererWrapper.LayerOrderProperty))]
        public void LayerOrderPropertyViewer(Main.Aggregator.Events.Tools.SpriteRendererWrapper.LayerOrderProperty eventData)
        {
            if (SpriteRenderer.Value)
                SpriteRenderer.Value.sortingOrder = eventData.PropertyValue;
        }

        protected override bool DoEnable()
        {
            if (!base.DoEnable())
                return false;

            if (SpriteRenderer.Value != null)
                SpriteRenderer.Value.enabled = true;//

            return true;
        }

        protected override bool DoDisable()
        {
            if (!base.DoDisable())
                return false;

            if (SpriteRenderer.Value != null)
                SpriteRenderer.Value.enabled = false;

            return true;
        }

        protected override void OnDestroy()
        {
            // Detects indirect destroying state of game object
            // This solutions needs for avoid exception: Destroying object multiple times. Don't use DestroyImmediate on the same object in OnDisable or OnDestroy.
            if (gameObject.activeInHierarchy &&
                SpriteRenderer.Value)
#if UNITY_EDITOR
                GameObject.DestroyImmediate(SpriteRenderer.Value);
#else
                GameObject.Destroy(SpriteRenderer.Value);
#endif
            base.OnDestroy();
        }
    }
}