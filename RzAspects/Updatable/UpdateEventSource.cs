using System;
using System.Diagnostics;
using System.Threading;

namespace RzAspects
{
    public interface IUpdateEventSource
    {
        event Action<UpdateTime> Updated;
        bool Paused { get; }
        void Pause();
        void Unpause();
    }

    public abstract class UpdateEventSource : IUpdateEventSource
    {
        public event Action<UpdateTime> Updated;

        protected Stopwatch _stopwatch = new Stopwatch();
        protected long _lastUpdateTime = 0;

        public bool Paused { get; protected set; }

        public UpdateEventSource()
        {
            Unpause();
        }

        public void Pause()
        {
            lock( _stopwatch )
            {
                Paused = true;
                _stopwatch.Stop();
            }
        }

        public void Unpause()
        {
            lock( _stopwatch )
            {
                Paused = false;
                _stopwatch.Start();
            }
        }

        protected void OnUpdate( object sender, EventArgs e )
        {
            if( !Paused )
            {
                if( Updated != null )
                {
                    Updated( CreateUpdateEvent() );
                }

                _lastUpdateTime = _stopwatch.ElapsedMilliseconds;
            }
        }

        protected abstract UpdateTime CreateUpdateEvent();
    }

    public class MockUpdateSource : IUpdateEventSource
    {
        public event Action<UpdateTime> Updated;

        public bool Paused { get; set; }

        public void Pause()
        {
            Paused = true;
        }

        public void Unpause()
        {
            Paused = false;
        }

        public void Update()
        {
            Update( new UpdateTime() );
        }

        public void Update( UpdateTime ut )
        {
            if( Updated != null ) Updated( ut );
        }
    }
}
