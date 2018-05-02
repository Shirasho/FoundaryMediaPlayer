using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace FoundaryMediaPlayer.Application.Components
{
    /// <summary>
    /// Causes the application to display a wait cursor until all <see cref="FWaitCursor"/> objects are disposed.
    /// </summary>
    /// <remarks>This class internally tracks all active cursors.</remarks>
    public sealed class FWaitCursor : IDisposable
    {
        /// <summary>
        /// The default cursor.
        /// </summary>
        private static Cursor DefaultCursor { get; } = Mouse.OverrideCursor;

        private static List<FWaitCursor> ActiveCursors { get; } = new List<FWaitCursor>();

        /// <summary>
        /// Creates a new <see cref="FWaitCursor"/> instance, causing the application to
        /// display a wait cursor (hourglass, for example). This class internally tracks
        /// all active cursors.
        /// </summary>
        public FWaitCursor()
        {
            lock (ActiveCursors)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                ActiveCursors.Add(this);
            }
        }

        /// <summary>
        /// Disposes of the <see cref="FWaitCursor"/> instance and returns
        /// the cursor back to the application default if no other <see cref="FWaitCursor"/>
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
        /// Disposes and invalidates all <see cref="FWaitCursor"/> instances and resets
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
