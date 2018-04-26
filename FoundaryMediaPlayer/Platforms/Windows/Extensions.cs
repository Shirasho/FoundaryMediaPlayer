using HResult = PInvoke.HResult.Code;

namespace FoundaryMediaPlayer.Platforms.Windows
{
    /// <summary>
    /// Windows-specific native extensions.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Casts the <see cref="HResult"/> to an int.
        /// </summary>
        public static int AsInt(this HResult result)
        {
            return unchecked((int) result);
        }

        /// <summary>
        /// Returns whether this <see cref="int"/> is considered a successful <see cref="HResult"/>.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="bStrict">Whether the result must strictly equal <see cref="HResult.S_OK"/> to be considered successful.</param>
        public static bool IsSuccess(this int result, bool bStrict = false)
        {
            return bStrict ? result >= 0 : result == 0;
        }
    }
}
