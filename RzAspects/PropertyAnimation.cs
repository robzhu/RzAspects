using System;
using System.Diagnostics;

namespace RzAspects
{
    /// <summary>
    /// This class gradually changes the value of a property over time as defined by a tweening function.
    /// </summary>
    public class PropertyAnimation : UpdatableModel
    {
        public event Action Completed;

        public string PropertyIsCompleted { get { return "IsCompleted"; } }
        private bool _IsCompleted = false;
        public bool IsCompleted
        {
            get { return _IsCompleted; }
            private set { SetProperty( PropertyIsCompleted, ref _IsCompleted, value ); }
        }

        private Func<double,double,double,double,double> _easingFunction;
        private Action<double> _setter;
        private double _to;
        private double _from;
        private double _startTime = 0;
        private double _elapsedTime = 0;
        private double _duration;
        private bool _started = false;

        public PropertyAnimation(
            Action<double> setter,
            double from,
            double to,
            double duration,
            EasingFunctionId equation )
            : this( setter, from, to, duration, EasingFunctions.EquationToFunc( equation ) )
        {
        }

        public PropertyAnimation( 
            Action<double> setter, 
            double from, 
            double to, 
            double duration,
            Func<double, double, double, double, double> easingFunction )
        {
            Debug.Assert( setter != null );

            _setter = setter;
            _to = to;
            _from = from;
            _duration = duration;
            _easingFunction = easingFunction;
        }

        protected override void UpdateInternal( UpdateTime time )
        {
            if( !_started )
            {
                _startTime = time.TotalTime;
                _started = true;
            }

            _elapsedTime = time.TotalTime - _startTime;
            if( _elapsedTime > _duration )
            {
                _elapsedTime = _duration;

                _setter( _to );

                IsCompleted = true;
                RaiseCompleted();
                Expire();
            }
            else
            {
                double value = _easingFunction( _elapsedTime, _from, _to, _duration );
                _setter( value );
            }
        }

        private void RaiseCompleted()
        {
            if( Completed != null )
            {
                Completed();
                Completed = null;
            }
        }
    }
}
