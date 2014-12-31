using System.Threading.Tasks;

namespace RzAspects
{
    public static class TaskUtility
    {
        private static Task _completedTask = Task.FromResult( false );

        public static Task CompletedTask()
        {
            return _completedTask;
        }
    }
}
