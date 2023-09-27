using Main.Events;
using Main.Objects;

namespace Main
{
    namespace Aggregator.Events.MapCell
    {
        public sealed class PathCostProperty : SharedPropertyEvent<int>
        {
            public PathCostProperty() : base()
            {
            }
        }
    }
}