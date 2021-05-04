using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using CryptoSaver.Core.ExtensionMethods;

namespace CryptoSaver.Core
{
    /// <summary>
    /// Provides helper methods to aid in certain animations
    /// </summary>
    public static class AnimationHelper
    {
        private static readonly Type This = typeof(AnimationHelper);

        /// <summary>
        /// Fades the specified elements.
        /// </summary>
        /// <param name="elements">The elements.</param>
        /// <param name="toValue">To value.</param>
        /// <param name="durationSeconds">The duration seconds.</param>
        /// <returns>System.Threading.Tasks.Task.</returns>
        public static async Task Fade(this IEnumerable<UIElement> elements, double toValue = 0, double durationSeconds = 3)
        {
            // This.Log().Info("Fading " + elements.Count() + " element(s)");
            var tasks = elements.Select(element => Fade((UIElement)element, toValue, durationSeconds.Secs())).ToList();
            await Task.WhenAll(tasks);
            //Thread.Sleep(100);
        }

        /// <summary>
        /// Fades the specified UI element.
        /// </summary>
        /// <param name="uiElement">The UI element.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>System.Threading.Tasks.Task.</returns>
        public static async Task Fade(this UIElement uiElement, CancellationToken? cancellationToken = null)
        {
            await Start_Animaiton(uiElement, UIElement.OpacityProperty, 0, null, cancellationToken);
        }

        /// <summary>
        /// Fades the specified UI element.
        /// </summary>
        /// <param name="uiElement">The UI element.</param>
        /// <param name="toValue">To value.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>System.Threading.Tasks.Task.</returns>
        public static async Task Fade(this UIElement uiElement, double toValue = 0, TimeSpan? duration = null, CancellationToken? cancellationToken = null)
        {
            await Start_Animaiton(uiElement, UIElement.OpacityProperty, toValue, duration, cancellationToken);
        }

        /// <summary>
        /// Cancel any animations that are running on the specified dependency property.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="dependencyProperty">The dependency property.</param>
        public static void CancelAnimation(this UIElement element, DependencyProperty dependencyProperty)
        {
            element.BeginAnimation(dependencyProperty, null);
            // cts.Cancel();
        }

        /// <summary>
        /// Starts the animation.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="dependencyProperty">The dependency property.</param>
        /// <param name="toValue">To value.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>System.Threading.Tasks.Task.</returns>
        public static Task Start_Animaiton(this UIElement element, DependencyProperty dependencyProperty, double toValue, TimeSpan? duration, CancellationToken? cancellationToken = null)
        {
            var tcs = new TaskCompletionSource<bool>();
            var fromValue = (double)element.GetValue(dependencyProperty);

            if (duration == null) duration = 3.Secs();

            if (fromValue.Equals(toValue))
            {
                tcs.SetResult(true);
                return tcs.Task;
            }

            var animation = new DoubleAnimation
            {
                From = fromValue,
                To = toValue,
                Duration = (Duration)duration,
                FillBehavior = FillBehavior.HoldEnd
            };

            EventHandler onComplete = null;

            cancellationToken?.ThrowIfCancellationRequested();

            onComplete = (a, e) =>
            {
                animation.Completed -= onComplete;

                element.SetValue(dependencyProperty, element.GetValue(dependencyProperty));
                element.CancelAnimation(dependencyProperty);

                tcs.SetResult(true);
            };

            animation.Completed += onComplete;
            element.BeginAnimation(dependencyProperty, animation);

            return tcs.Task;
        }
    }
}