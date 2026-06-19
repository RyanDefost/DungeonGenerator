using Partitions;

namespace Passes.RoomPasses
{
    public abstract class BaseRoomPass : GenerationPass
    {
        public abstract bool SetPass(Partition partition);
    }
}