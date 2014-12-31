using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RzAspects
{
    public static class PropertyAnimator
    {
        static List<IUpdatable> _animations = new List<IUpdatable>();

        /// <summary>
        /// Animates a property using the UpdatableModel component
        /// </summary>
        /// <param name="setter">The delegate to call to update the value.</param>
        /// <param name="from">The value to animate from.</param>
        /// <param name="to">The value to animaet to.</param>
        /// <param name="duration">The duration of the animation in milliseconds.</param>
        /// <param name="equation">The animation equation.</param>
        /// <returns>The task that signals the completion of this animation.</returns>
        public static async Task AnimateAsync( 
            Action<double> setter, 
            double from, 
            double to, 
            double duration,
            EasingFunctionId equation = EasingFunctionId.Linear )
        {
            await AnimateAsync( setter, from, to, duration, EasingFunctions.EquationToFunc( equation ) );
        }

        /// <summary>
        /// Animates a property using the UpdatableModel component
        /// </summary>
        /// <param name="setter">The delegate to call to update the value.</param>
        /// <param name="from">The value to animate from.</param>
        /// <param name="to">The value to animaet to.</param>
        /// <param name="duration">The duration of the animation in milliseconds.</param>
        /// <param name="easingFunction">The animation function.</param>
        /// <returns>The task that signals the completion of this animation.</returns>
        public static async Task AnimateAsync(
            Action<double> setter,
            double from,
            double to,
            double duration,
            Func<double,double,double,double,double> easingFunction )
        {
            PropertyAnimation animation = new PropertyAnimation( setter, from, to, duration, easingFunction );

            _animations.Add( animation );
            await animation.ExpireAsync();
            _animations.Remove( animation );
        }
    }
}
