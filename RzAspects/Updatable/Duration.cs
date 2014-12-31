using System;

namespace RzAspects
{
    public enum DurationState
    {
        Paused,
        Tracking,
        Completed
    }

    public interface IDuration : IUpdatable
    {
        DurationState State { get; }

        /// <summary>
        /// This event is fired when the Elapsed value exceeds the TotalSpan value
        /// </summary>
        event Action OnTotalDurationElapsed;

        /// <summary>
        /// The total amount of time, in milliseconds, to elapse before the DurationElapsed event is fired.
        /// </summary>
        double TotalSpan { get; }

        /// <summary>
        /// The total amount of time that has elapsed since the Duration was started or reset.
        /// </summary>
        double TotalElapsed { get; }

        /// <summary>
        /// The amount of time (milliseconds) before the OnDurationElapsed event fires.
        /// </summary>
        double TotalRemaining { get; }

        /// <summary>
        /// The ratio of TotalRemaining / TotalSpan
        /// </summary>
        double RemainingRatio { get; }

        /// <summary>
        /// This event is fired periodically, as defined by the 
        /// </summary>
        event Action<int> OnPeriodicDurationElapsed;

        double PeriodSpan { get; }
        double PeriodElapsed { get; }
        double PeriodRemaining { get; }

        int PeriodCount { get; }

        void Pause();
        void Unpause();

        /// <summary>
        /// Resets this duration object.
        /// </summary>
        /// <param name="totalSpan">The total span that should elapse before the event is fired.</param>
        void Reset( double totalSpan, double periodSpan = -1 );
    }

    /// <summary>
    /// This class tracks a duration of time, raising an event when a specified time span elapses.
    /// </summary>
    public sealed class Duration : UpdatableModel, IDuration
    {
        public event Action OnTotalDurationElapsed;
        private void RaiseOnTotalDurationElapsed() { if( OnTotalDurationElapsed != null ) OnTotalDurationElapsed(); }

        public event Action<int> OnPeriodicDurationElapsed;
        private void RaiseOnPeriodicDurationElapsed() { if( OnPeriodicDurationElapsed != null ) OnPeriodicDurationElapsed( PeriodCount ); }

        public string PropertyState { get { return "State"; } }
        private DurationState _state = DurationState.Tracking;
        public DurationState State
        {
            get { return _state; }
            private set { SetProperty( PropertyState, ref _state, value ); }
        }

        public string PropertyTotalSpan { get { return "TotalSpan"; } }
        private double _TotalSpan;
        public double TotalSpan
        {
            get { return _TotalSpan; }
            private set { SetProperty( PropertyTotalSpan, ref _TotalSpan, value ); }
        }

        public string PropertyTotalElapsed { get { return "TotalElapsed"; } }
        private double _TotalElapsed = 0;
        public double TotalElapsed
        {
            get { return _TotalElapsed; }
            private set { SetProperty( PropertyTotalElapsed, ref _TotalElapsed, value ); }
        }

        public string PropertyTotalRemaining { get { return "TotalRemaining"; } }
        private double _totalRemaining;
        public double TotalRemaining
        {
            get { return _totalRemaining; }
            private set { SetProperty( PropertyTotalRemaining, ref _totalRemaining, value ); }
        }

        public string PropertyRemainingRatio { get { return "RemainingRatio"; } }
        private double _RemainingRatio;
        public double RemainingRatio
        {
            get { return _RemainingRatio; }
            private set { SetProperty( PropertyRemainingRatio, ref _RemainingRatio, value ); }
        }

        public string PropertyPeriodSpan { get { return "PeriodSpan"; } }
        private double _PeriodSpan;
        public double PeriodSpan
        {
            get { return _PeriodSpan; }
            private set { SetProperty( PropertyPeriodSpan, ref _PeriodSpan, value ); }
        }

        public string PropertyPeriodElapsed { get { return "PeriodElapsed"; } }
        private double _PeriodElapsed;
        public double PeriodElapsed
        {
            get { return _PeriodElapsed; }
            private set { SetProperty( PropertyPeriodElapsed, ref _PeriodElapsed, value ); }
        }

        public string PropertyPeriodRemaining { get { return "PeriodRemaining"; } }
        private double _PeriodRemaining;
        public double PeriodRemaining
        {
            get { return _PeriodRemaining; }
            set { SetProperty( PropertyPeriodRemaining, ref _PeriodRemaining, value ); }
        }

        public string PropertyPeriodCount { get { return "PeriodCount"; } }
        private int _PeriodCount;
        public int PeriodCount
        {
            get { return _PeriodCount; }
            private set { SetProperty( PropertyPeriodCount, ref _PeriodCount, value ); }
        }

        private bool _trackPeriod = false;
        
        public Duration( double totalSpan, bool trackDuration ) : this( totalSpan, -1, trackDuration ) { }

        /// <param name="totalSpan">Total span in milliseconds.</param>
        /// <param name="periodSpan">The period span in milliseconds.</param>
        /// <param name="startImmediately">Whether to start the clock immediately.</param>
        public Duration( double totalSpan, double periodSpan = -1, bool startImmediately = true )
        {
            Reset( totalSpan, periodSpan );
            if( !startImmediately ) Pause();
        }

        /// <param name="updateGroupId">The update groupd id to use.</param>
        /// <param name="totalSpan">Total span in milliseconds.</param>
        /// <param name="periodSpan">The period span in milliseconds.</param>
        /// <param name="trackDuration">Whether to start the clock immediately.</param>
        private Duration( int updateGroupId, double totalSpan, double periodSpan = -1, bool trackDuration = true )
            : base( updateGroupId ) 
        {
            Reset( totalSpan, periodSpan );
            if( !trackDuration ) Pause();
        }

        public static Duration CreateWithCustomUpdateGroup( int updateGroupId, double totalSpan, bool startImmediately = true, double periodSpan = -1 )
        {
            return new Duration( updateGroupId, totalSpan, periodSpan, startImmediately );
        }

        public void Pause()
        {
            if( State == DurationState.Tracking ) State = DurationState.Paused;
        }

        public void Unpause()
        {
            if( State == DurationState.Paused ) State = DurationState.Tracking;
        }

        public void Reset()
        {
            Reset( this.TotalSpan, this.PeriodSpan );
        }

        public void Reset( double totalSpan, double periodSpan = -1 )
        {
            if( totalSpan <= 0 ) throw new Exception( "totalSpan cannot be negative" );
            _trackPeriod = ( periodSpan > 0 );

            TotalSpan = totalSpan;
            TotalRemaining = totalSpan;
            TotalElapsed = 0;

            PeriodSpan = periodSpan;
            PeriodRemaining = periodSpan;
            PeriodElapsed = 0;

            State = DurationState.Tracking;
        }

        protected override void UpdateInternal( UpdateTime gameTime )
        {
            if( State == DurationState.Tracking )
            {
                TotalElapsed += gameTime.ElapsedTime;
                TotalRemaining -= gameTime.ElapsedTime;

                if( _trackPeriod )
                {
                    PeriodElapsed += gameTime.ElapsedTime;

                    int elapsedPeriods = (int) ( TotalElapsed / PeriodSpan );
                    while( PeriodCount < elapsedPeriods )
                    {
                        PeriodCount++;
                        PeriodElapsed -= PeriodSpan;
                        RaiseOnPeriodicDurationElapsed();
                    }

                    PeriodRemaining = PeriodSpan - PeriodElapsed;
                }

                if( TotalElapsed >= TotalSpan )
                {
                    RaiseOnTotalDurationElapsed();
                    State = DurationState.Completed;
                    TotalRemaining = 0;
                }

                RemainingRatio = TotalRemaining / TotalSpan;
            }
        }
    }
}
