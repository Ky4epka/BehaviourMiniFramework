using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Generic
{
    public interface IPriorityObserver<PRIO_TYPE> where PRIO_TYPE : IComparable<PRIO_TYPE>
    {
        PRIO_TYPE Priority { get; set; }
    }
}
