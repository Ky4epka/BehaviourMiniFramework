using Main.Managers;

namespace Main.Objects
{
    public interface IManagedObject
    {
        IObjectManager MasterManager { get; set; }
    }

}
