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
    public class BulletSampleProperty : SharedPropertyEvent<GameObject>
    {
    }
}

namespace Main.Aggregator.Properties.Behaviours.Tanks
{
    public class BulletSampleProperty : SharedPropertyReference<GameObject, Main.Aggregator.Events.Behaviours.Tanks.BulletSampleProperty>
    {
        public override string GroupTag => "Tank.gun";
        public override string SharedName => "BulletSample";
    }
}

namespace Main.Aggregator.Events.Behaviours.Tanks
{
    public class DoFireEvent : EventDataBase
    {

    }
}


namespace Main.Aggregator.Events.Behaviours.Tanks
{
    public class FirePointProperty : SharedPropertyEvent<Transform>
    {
    }
}
namespace Main.Aggregator.Properties.Behaviours.Tanks
{
    public class FirePointProperty : SharedPropertyReference<Transform, Main.Aggregator.Events.Behaviours.Tanks.FirePointProperty>
    {
        public override string GroupTag => "Tank.gun";
        public override string SharedName => "FirePoint";
    }
}


namespace Main.Aggregator.Events.Behaviours.Tanks
{
    public class FireDelayProperty : SharedPropertyEvent<float>
    {
    }
}
namespace Main.Aggregator.Properties.Behaviours.Tanks
{
    public class FireDelayProperty : SharedProperty<float, Main.Aggregator.Events.Behaviours.Tanks.FireDelayProperty>
    {
        public override string GroupTag => "Tank.gun";
        public override string SharedName => "FireDelay";
    }
}


namespace Main.Aggregator.Events.Behaviours.Tanks
{
    public class ReloadTimeProperty : SharedPropertyEvent<float>
    {
    }
}
namespace Main.Aggregator.Properties.Behaviours.Tanks
{
    public class ReloadTimeProperty : SharedProperty<float, Main.Aggregator.Events.Behaviours.Tanks.ReloadTimeProperty>
    {
        public override string GroupTag => "Tank.gun";
        public override string SharedName => "ReloadTime";
    }
}

namespace Main.Objects.Behaviours.Tanks
{
    /*
     * fire point (transform)
     * Fire delay (float)
     * Reload (float)
     */
    [Unique]
    public class GunBehaviour : ObjectBehavioursBase
    {
        [SharedProperty]
        public Main.Aggregator.Properties.Behaviours.Tanks.BulletSampleProperty BulletSample { get; protected set; }

        [SharedProperty]
        public Main.Aggregator.Properties.Behaviours.Tanks.FirePointProperty FirePoint { get; protected set; }

        [SharedProperty]
        public Main.Aggregator.Properties.Behaviours.Tanks.FireDelayProperty FireDelay { get; protected set; }
        [SharedProperty]
        public Main.Aggregator.Properties.Behaviours.Tanks.ReloadTimeProperty ReloadTime { get; protected set; }


        protected Coroutine iFireCoroutine = null;

        [EnabledStateEvent]
        public void DoFireEvent(Aggregator.Events.Behaviours.Tanks.DoFireEvent eventData)
        {
            if (iFireCoroutine == null)
                iFireCoroutine = StartCoroutine(DoFire());
        }

        protected virtual IEnumerator DoFire()
        {
            if (!BulletSample.Value)
                yield break;

            yield return new WaitForSeconds(FireDelay.Value);
            Event<Aggregator.Events.Tools.AnimatorWrapper.PlayEvent>(Container).Invoke("Fire", 1, 0f);

            GameObject newBullet = Managers.PoolManager.Instance.RentElement<Bullets.BulletBaseBehaviour>()?.gameObject;

            if (!newBullet)
            {
                GLog.LogError(nameof(GunBehaviour), $"Could not rent {nameof(Bullets.BulletBaseBehaviour)} from pool.");
                yield break;
            }
            newBullet.SetActive(true);

            yield return new WaitForSeconds(0.1f);
            Bullets.BulletBaseBehaviour bulletController = newBullet.GetComponent<Bullets.BulletBaseBehaviour>();

            if (bulletController == null)
            {
                GLog.Log("Could not exectue the fire action for a reason: The object '"+gameObject+"' has no bullet controller '"+nameof(Bullets.BulletBaseBehaviour)+"' component");
                yield break;                    
            }

            bulletController.Event<Aggregator.Events.Behaviours.Tanks.Bullets.DoFireEvent>(Container).Invoke(Container, FirePoint.Value);

            yield return new WaitForSeconds(ReloadTime.Value);

            StopCoroutine(iFireCoroutine);
            iFireCoroutine = null;
        }
    }
}