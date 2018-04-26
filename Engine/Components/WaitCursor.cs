using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Threading;

namespace FoundaryMediaPlayer.Engine.Components
{
    /// <summary>
    /// Causes the application to display a wait cursor until all <see cref="WaitCursor"/> objects are disposed.
    /// </summary>
    /// <remarks>This class internally tracks all active cursors.</remarks>
    public sealed class WaitCursor : IDisposable
    {
        /// <summary>
        /// The default cursor.
        /// </summary>
        private static Cursor DefaultCursor { get; } = Mouse.OverrideCursor;

        private static List<WaitCursor> ActiveCursors { get; } = new List<WaitCursor>();

        /// <summary>
        /// Creates a new <see cref="WaitCursor"/> instance, causing the application to
        /// display a wait cursor (hourglass, for example). This class internally tracks
        /// all active cursors.
        /// </summary>
        public WaitCursor()
        {
            lock (ActiveCursors)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                ActiveCursors.Add(this);
            }
        }

        /// <summary>
        /// Disposes of the <see cref="WaitCursor"/> instance and returns
        /// the cursor back to the application default if no other <see cref="WaitCursor"/>
        /// instances are active.
        /// </summary>
        public void Dispose()
        {
            lock (ActiveCursors)
            {
                ActiveCursors.Remove(this);
                if (ActiveCursors.Count == 0)
                {
                    Mouse.OverrideCursor = DefaultCursor;
                }
            }
        }

        /// <summary>
        /// Disposes and invalidates all <see cref="WaitCursor"/> instances and resets
        /// the cursor back to the default cursor.
        /// </summary>
        public static void DisposeAll()
        {
            lock (ActiveCursors)
            {
                ActiveCursors.Clear();
                Mouse.OverrideCursor = DefaultCursor;
            }
        }
    }
}
