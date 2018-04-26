using System;

namespace FoundaryMediaPlayer.Events
{
    /// <summary>
    /// 
    /// </summary>
    public class SliderSeekEventArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public object Sender { get; }

        /// <summary>
        /// 
        /// </summary>
        public double PercentOffsetFromZero { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="percentOffSetFromZero"></param>
        public SliderSeekEventArgs(object sender, double percentOffSetFromZero)
        {
            Sender = sender;
            PercentOffsetFromZero = percentOffSetFromZero;
        }
    }
}
