using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Main.Objects;
using Main.Objects.Behaviours;
using Main.Objects.Behaviours.Attributes;
using Main;


namespace Main.Aggregator.Events.Managers.MapCameraManager
{
    public class ScaleStepProperty : SharedPropertyEvent<float>
    {
    }
}

namespace Main.Aggregator.Properties.Managers.MapCameraManager
{
    public class ScaleStepProperty : SharedProperty<float, Main.Aggregator.Events.Managers.MapCameraManager.ScaleStepProperty>
    {
        public override string GroupTag => "Navigation";
        public override string SharedName => "ScaleStep";
    }
}


namespace Main.Aggregator.Events.Managers.MapCameraManager
{
    public class ScaleRangeProperty : SharedPropertyEvent<Vector2>
    {
    }
}
namespace Main.Aggregator.Properties.Managers.MapCameraManager
{
    public class ScaleRangeProperty : SharedProperty<Vector2, Main.Aggregator.Events.Managers.MapCameraManager.ScaleRangeProperty>
    {
        public override string GroupTag => "Navigation";
        public override string SharedName => "ScaleRange";
    }
}

namespace Main.Managers
{
    [RequireComponent(typeof(MapCameraManager))]
    public class MapCameraNavigator : ObjectBehavioursBase
    {

        [SharedProperty]
        public Main.Aggregator.Properties.Managers.MapCameraManager.ScaleProperty ScaleStep { get; protected set; }
        [SharedProperty]
        public Main.Aggregator.Properties.Managers.MapCameraManager.ScaleRangeProperty ScaleRange { get; protected set; }

        [SharedPropertyViewer(typeof(Main.Aggregator.Properties.Managers.MapCameraManager.ScaleStepProperty))]
        public void ScaleStepPropertyViewer(Main.Aggregator.Events.Managers.MapCameraManager.ScaleStepProperty eventData)
        {
        }

        [SharedPropertyViewer(typeof(Main.Aggregator.Properties.Managers.MapCameraManager.ScaleRangeProperty))]
        public void ScaleRangePropertyViewer(Main.Aggregator.Events.Managers.MapCameraManager.ScaleRangeProperty eventData)
        {
        }

        protected void LateUpdate()
        {
            /*
            if (Input.GetMouseButtonDown(1))
            {
                ilastCurPosition = Input.mousePosition;
            }

            if (Input.GetMouseButton(1))
            {
                Vector3 delta = icachedCameraMan.Camera.ScreenToWorldPoint(ilastCurPosition) - icachedCameraMan.Camera.ScreenToWorldPoint(Input.mousePosition);
                icachedCameraMan.Position += (Vector2)delta;
            }

            Vector2 mouseDelta = Input.mouseScrollDelta;
            if (!MathKit.Vectors2DEquals(mouseDelta, Vector2.zero))
            {
                icachedCameraMan.Scale = MathKit.EnsureRange(icachedCameraMan.Scale - mouseDelta.y * iScaleStep, iScaleRange);
            }

            ilastCurPosition = Input.mousePosition;
            */
        }

    }

}
