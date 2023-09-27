using System;

namespace Main.Objects.Behaviours
{
    public class TypeNotAssignableFromBehaviourBaseException : Exception
    {
        public TypeNotAssignableFromBehaviourBaseException(Type t) : base(string.Format("Type '{0}' must be assignable from '{1}'", t.FullName, typeof(IObjectBehavioursBase).FullName))
        {

        }
    }
}