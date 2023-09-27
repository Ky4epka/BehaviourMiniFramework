using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.BehaviourInterop.Runnable
{

    public interface IStateContext<StateType> where StateType : IState
    {
        bool SetState(StateType state);

        bool HandleState();
    }
}
