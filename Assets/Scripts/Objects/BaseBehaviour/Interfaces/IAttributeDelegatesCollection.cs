using System;
using System.Collections.Generic;

namespace Main.Objects.Behaviours
{
    using Attributes;

    public interface IAttributeDelegatesCollection
    {
        void ClearDelegatesCacheByAttribute(Type attributeType);
    }
}