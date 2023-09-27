using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Generic
{
    public interface IPriorityObservable<T>
    {
        bool AddObserver(T observer);
        bool RemoveObserver(T observer);
    }
}
