using Main.Managers;

namespace Main.Level
{
    public class LevelObjectManager: ObjectManagerMonoBehaviourWrapper
    {
        public override IObjectManager ObjectManager { get; protected set; } = new ObjectManager();
    }
}