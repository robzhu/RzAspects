using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RzAspects
{
    public class CompositeTaskUnitProcessor<T> where T : ICompositeTaskUnit<T>
    {
        public async Task ProcessTaskUnitRootAsync( T root, Func<T, Task> processor )
        {
            var rootTasks = root.ExpandParallel();
            await ProcessParallelTaskUnitsAsync( rootTasks, processor );
        }

        private async Task ProcessParallelTaskUnitsAsync( IEnumerable<T> taskUnits, Func<T, Task> processor )
        {
            await Task.WhenAll( TaskUnitTaskEnumerator( taskUnits, processor ) );
        }

        private IEnumerable<Task> TaskUnitTaskEnumerator( IEnumerable<T> taskUnits, Func<T, Task> processor )
        {
            foreach( var tu in taskUnits )
            {
                yield return ProcessTaskUnitAsync( tu, processor );
            }
        }

        private async Task ProcessTaskUnitAsync( T unit, Func<T, Task> processor )
        {
            await processor( unit );
            await ProcessParallelTaskUnitsAsync( unit.GetNextUnits(), processor );
        }
    }
}
