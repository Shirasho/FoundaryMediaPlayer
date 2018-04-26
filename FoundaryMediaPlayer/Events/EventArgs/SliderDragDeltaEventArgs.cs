using System.Windows.Controls.Primitives;

namespace FoundaryMediaPlayer.Events
{
    /// <summary>
    /// 
    /// </summary>
    public class SliderDragDeltaEventArgs : DragDeltaEventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="horizontalChange"></param>
        /// <param name="verticalChange"></param>
        public SliderDragDeltaEventArgs(double horizontalChange, double verticalChange) 
            : base(horizontalChange, verticalChange)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        public SliderDragDeltaEventArgs(DragDeltaEventArgs e)
            : base(e.HorizontalChange, e.VerticalChange)
        {
            Handled = e.Handled;
            RoutedEvent = e.RoutedEvent;
            Source = e.Source;
        }
    }
}
