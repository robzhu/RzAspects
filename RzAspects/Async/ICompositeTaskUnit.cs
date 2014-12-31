using System.Collections.Generic;

namespace RzAspects
{
    public interface ICompositeTaskUnit<T> 
    {
        T And( params T[] parallelUnits );
        T Then( params T[] nextUnits );
        IEnumerable<T> GetParallelUnits();
        IEnumerable<T> ExpandParallel();
        IEnumerable<T> GetNextUnits();
    }
}
