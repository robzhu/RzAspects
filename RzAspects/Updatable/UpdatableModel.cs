using System;
using System.Threading.Tasks;

namespace RzAspects
{
    /// <summary>
    /// A model that updates itself whenever the game time changes.
    /// </summary>
    public abstract class UpdatableModel : CompositePropertyChangeNotificationBase, IUpdatable
    {
        public event Action<IExpirable> OnExpired;
        public async Task ExpireAsync()
        {
            var tcs = new TaskCompletionSource<object>();
            Action<IExpirable> handler = ( expired ) => tcs.TrySetResult( expired );

            try
            {
                OnExpired += handler;
                await tcs.Task;
            }
            finally
            {
                OnExpired -= handler;
            }
        }

        public string PropertyIsExpired { get { return "IsExpired"; } }
        private bool _IsExpired = false;
        public bool IsExpired
        {
            get { return _IsExpired; }
            protected set
            {
                if( SetProperty( PropertyIsExpired, ref _IsExpired, value ) )
                {
                    if( value ) RaiseOnExpired();
                }
            }
        }

        public string PropertyDuration { get { return "Duration"; } }
        private IDuration _Duration = null;
        public IDuration Duration
        {
            get { return _Duration; }
            set { SetProperty( PropertyDuration, ref _Duration, value ); }
        }

        private int _updateGroupId = 0;
        public int UpdateGroupId { get { return _updateGroupId; } }
        public Guid Id { get; private set; }

        protected UpdatableModel( int updateId )
        {
            Id = Guid.NewGuid();

            _updateGroupId = updateId;
            IBucketUpdateService updateBucket = IoCContainer.GetInstance<IBucketUpdateService>();
            updateBucket.RegisterUpdatable( this );
        }

        protected UpdatableModel( IBucketUpdateService updateService = null )
        {
            Id = Guid.NewGuid();

            if( updateService == null )
                updateService = IoCContainer.GetInstance<IBucketUpdateService>();

            if( updateService != null )
            {
                updateService.RegisterUpdatable( this );
            }
        }

        protected void RaiseOnExpired()
        {
            if( OnExpired != null )
            {
                OnExpired( this );
                OnExpired = null;
            }
        }

        public void Update( UpdateTime time )
        {
            if( IsExpired ) return;

            UpdateInternal( time );

            if( ( Duration != null ) && ( Duration.State == DurationState.Completed ) && !IsExpired )
            {
                Expire();
            }
        }

        protected virtual void UpdateInternal( UpdateTime time ) { }

        public void Expire()
        {
            BeforeExpire();
            IsExpired = true;
            AfterExpire();
        }

        protected virtual void BeforeExpire() { }
        protected virtual void AfterExpire() { }
    }
}
