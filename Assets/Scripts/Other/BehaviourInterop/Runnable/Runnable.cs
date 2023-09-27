using System;
using UnityEngine;

namespace Main.BehaviourInterop.Runnable
{
    [Serializable]
    public class Runnable: IStateContext<Runnable.RunnableStateBase>, IStateContext, IRunnable
    {
        [SerializeField]
        protected RunnableStateBase iChangingState = null;
        [SerializeField]
        protected RunnableStateBase iCurrentState = null;
        [SerializeField]
        protected RunnableStatus iStatus = RunnableStatus.Unknown;


        public enum RunnableStatus
        {
            Unknown = 0x01,
            Running = 0x02,
            Runned = 0x04,
            Pausing = 0x08,
            Paused = 0x10,
            Stopping = 0x20,
            Stopped = 0x40,
            Resetting = 0x80,
            Resetted = 0x100
        }

        public enum RunnableState
        {
            Unknown = 0x00,
            Run = 0x01,
            Pause = 0x02,
            Stop = 0x04,
            Reset = 0x08            
        }

        public class RunnableStateBase : IState<Runnable>, IState, IDisposable
        {
            public virtual bool Disposed { get => iDisposed; }
            public virtual bool Prepared { get => iPrepared; }
            public virtual bool Handled { get => iHandled; }
            public virtual Runnable Handler { get => iHandler; }
            public RunnableStatus AppliedStatus { get => iAppliedStatus; }

            protected Runnable iHandler = null;
            protected RunnableStatus iAppliedStatus = RunnableStatus.Unknown;
            protected bool iDisposed = false;
            protected bool iPrepared = false;
            protected bool iHandled = false;

            public RunnableStateBase(Runnable handler)
            {
                iHandler = handler;
            }

            public virtual bool Handle(IStateContext context)
            {
                return Handle((Runnable)context);
            }

            public virtual bool Handle(Runnable context)
            {
                iHandled = true;
                return true;
            }

            public virtual bool Prepare(IStateContext context)
            {
                iPrepared = true;
                return Prepare((IStateContext)context);
            }

            public virtual bool Prepare(Runnable context)
            {
                return true;
            }

            public virtual bool AfterPrepare(IStateContext context)
            {
                return true;
            }

            public virtual void Dispose()
            {
                if (iDisposed) return;

                iDisposed = true;

                if (!Handled && Prepared) Handler?.RollbackStatus();
            }

            public virtual void Reset()
            {
                iHandled = false;
                iDisposed = false;
            }
        }

        public class RunState : RunnableStateBase
        {
            public RunState(Runnable handler) : base(handler)
            {

            }

            public override bool Prepare(Runnable context)
            {
                if ((RunnableStatus.Runned | RunnableStatus.Running).HasFlag(context.Status)) return false;

                iAppliedStatus = RunnableStatus.Running;
                context.SetStatus(iAppliedStatus);
                return base.Prepare(context);
            }

            public override bool Handle(Runnable context)
            {
                iAppliedStatus = RunnableStatus.Runned;
                context.SetStatus(iAppliedStatus);
                return base.Handle(context);
            }

        }

        public class PauseState : RunnableStateBase
        {
            public PauseState(Runnable handler) : base(handler)
            {

            }
            public override bool Prepare(Runnable context)
            {
                if (!(RunnableStatus.Runned).HasFlag(context.Status)) return false;

                iAppliedStatus = RunnableStatus.Pausing;
                context.SetStatus(iAppliedStatus);
                return base.Prepare(context);
            }

            public override bool Handle(Runnable context)
            {
                iAppliedStatus = RunnableStatus.Paused;
                context.SetStatus(iAppliedStatus);
                return base.Handle(context);
            }

        }

        public class StopState : RunnableStateBase
        {
            public StopState(Runnable handler) : base(handler)
            {

            }

            public override bool Prepare(Runnable context)
            {
                if (!(RunnableStatus.Runned | RunnableStatus.Paused).HasFlag(context.Status)) return false;

                iAppliedStatus = RunnableStatus.Stopping;
                context.SetStatus(iAppliedStatus);
                return base.Prepare(context);
            }

            public override bool Handle(Runnable context)
            {
                iAppliedStatus = RunnableStatus.Stopped;
                context.SetStatus(iAppliedStatus);
                return base.Handle(context);
            }
        }

        public class ResetState : RunnableStateBase
        {
            public ResetState(Runnable handler) : base(handler)
            {

            }

            public override bool Prepare(Runnable context)
            {
                if (!(RunnableStatus.Runned | RunnableStatus.Paused).HasFlag(context.Status)) return false;

                iAppliedStatus = RunnableStatus.Resetting;
                context.SetStatus(iAppliedStatus);
                return base.Prepare(context);
            }

            public override bool Handle(Runnable context)
            {
                iAppliedStatus = RunnableStatus.Resetted;
                context.SetStatus(iAppliedStatus);
                return base.Handle(context);
            }
        }


        public virtual RunnableStatus Status
        {
            get => iStatus;
        }

        public virtual bool SetState(RunnableStateBase state)
        {
            if (!StateChanging(state)) return false;

            iChangingState = state;
            if (state.AfterPrepare(this))
            {
                return true;
            }
            else
            {
                RollbackStatus();
                iChangingState = null;
                return false;
            }
        }

        public virtual bool HandleState()
        {
            if ((iChangingState == null) || (iChangingState.Disposed)) return false;

            iCurrentState?.Dispose();
            iCurrentState = iChangingState;
            RunnableStateBase temp = iChangingState;
            iChangingState = null;
            temp.Handle(this);
            StateChanged(iCurrentState);
            return true;
        }

        protected void SetStatus(RunnableStatus status)
        {
            iStatus = status;
            StatusChanged(iStatus);
        }

        protected virtual bool StateChanging(RunnableStateBase state)
        {
            return state.Prepare(this);
        }

        protected virtual void StateChanged(RunnableStateBase state)
        {
        }

        protected virtual void StatusChanged(RunnableStatus status)
        {

        }

        public virtual bool Run()
        {
            return SetState(new RunState(this));
        }

        public virtual bool Pause()
        {
            return SetState(new PauseState(this));
        }

        public virtual bool Stop()
        {
            return SetState(new StopState(this));
        }

        public virtual bool SetState(IState state)
        {
            return SetState((RunnableStateBase)state);
        }

        public virtual bool Reset()
        {
            return SetState(new ResetState(this));
        }

        protected virtual void RollbackStatus()
        {
            if (iCurrentState != null) SetStatus(iCurrentState.AppliedStatus);
        }
    }
}
