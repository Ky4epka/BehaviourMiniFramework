using Main.Other;
using System;
using System.Reflection;

namespace Main.Managers
{
    public class ObjectManager: ContainerCollection, IObjectManager
    {
        public IObjectManager MasterManager
        {
            get => iMasterManager;

            set
            {
                if (iMasterManager != null)
                    this.RemoveEventListener(iMasterManager);

                iMasterManager = value;

                if (iMasterManager != null)
                    this.AddEventListener(iMasterManager);
            }
        }

        protected IObjectManager iMasterManager = null;

        public override void Dispose()
        {
            if (Disposed)
                throw new ObjectDisposedException(this.ToString());

            Clear();
            MasterManager = null;
            base.Dispose();
        }

    }
}
