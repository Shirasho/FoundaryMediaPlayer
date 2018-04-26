using System;
using System.Threading;
using FluentAssertions;

namespace FoundaryMediaPlayer
{
    /// <summary>
    /// <see cref="CancellationTokenSource"/> extensions.
    /// </summary>
    public static class CancellationTokenSourceExtensions
    {
        /// <summary>
        /// Ensure that the <see cref="CancellationTokenSource"/> has not been
        /// cancelled. An <see cref="OperationCanceledException"/> will be thrown if it
        /// has.
        /// </summary>
        public static void Check(this CancellationTokenSource source)
        {
            source.Should().NotBeNull();
            source.Token.Should().NotBeNull();

            source.Token.ThrowIfCancellationRequested();
        }
    }
}
