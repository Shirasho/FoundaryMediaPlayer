using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

using HResult = PInvoke.HResult.Code;

namespace FoundaryMediaPlayer.Interop.Windows
{
    /// <summary>
    /// A utility class to help determine the success status of methods that return HRESULTs
    /// in a way similar to C++.
    /// </summary>
    /// <example>
    /// int hresult;
    /// if (ComResult.FAILED(hresult = NativeComMethod(object pUnk, out IGraphFilter2 graphFilter)))
    /// {
    ///    return hresult;
    /// }
    ///
    /// // OR
    /// if (ComResult.FAILED(() => NativeComMethod(object pUnk, out IGraphFilter2 graphFilter), out hresult))
    /// {
    ///    return hresult;
    /// }
    ///
    /// // C++
    /// if (FAILED(hresult = NativeComMethod(const IUnknown* pUnk, IGraphFilter2** graphGilter)))
    /// {
    ///    return hresult;
    /// }
    ///
    /// </example>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class ComResult
    {
        private int _Value { get; }

        /// <summary>
        /// A result using <see cref="HResult"/>.
        /// </summary>
        /// <param name="code"></param>
        public ComResult(HResult code)
        {
            _Value = unchecked((int) code);
        }

        /// <summary>
        /// A result using <see cref="int"/>.
        /// </summary>
        /// <param name="code"></param>
        public ComResult(int code)
        {
            _Value = code;
        }

        /// <summary>
        /// A result using <see cref="int"/> where the specified
        /// <see cref="long"/> will be unchecked cast into an <see cref="int"/>.
        /// </summary>
        /// <param name="code"></param>
        public ComResult(long code)
            : this(unchecked((int) code))
        {

        }

        public static implicit operator int(ComResult comResult)
        {
            return comResult._Value;
        }

        public static implicit operator HResult(ComResult value)
        {
            return (HResult) value._Value;
        }

        public static implicit operator ComResult(int code)
        {
            return new ComResult(code);
        }

        public static implicit operator ComResult(HResult code)
        {
            return new ComResult(code);
        }

        /// <summary>
        /// Returns whether <paramref name="result"/> is a successful HResult code.
        /// A null value will be considered a failure with <see cref="HResult.E_UNEXPECTED"/>.
        /// </summary>
        /// <param name="result">The result of the COM operation.</param>
        /// <param name="bStrict">Whether only <see cref="HResult.S_OK"/> is considered a success
        /// (in other words, whether to treat <see cref="HResult.S_FALSE"/> as a failure).</param>
        [PublicAPI]
        public static bool SUCCESS(int? result, bool bStrict = false)
        {
            return IsSuccess(result, out _, bStrict);
        }


        [PublicAPI]
        public static bool SUCCESS(Func<int> action, bool bStrict = false)
        {
            if (action == null)
            {
                return false;
            }

            return IsSuccess(action(), out _, bStrict);
        }

        /// <summary>
        /// Returns whether <paramref name="result"/> is a successful HResult code.
        /// A null value will be considered a failure with <see cref="HResult.E_UNEXPECTED"/>.
        /// </summary>
        /// <param name="result">The result of the COM operation.</param>
        /// <param name="bStrict">Whether only <see cref="HResult.S_OK"/> is considered a success
        /// (in other words, whether to treat <see cref="HResult.S_FALSE"/> as a failure).</param>
        [PublicAPI]
        public static bool SUCCESS(HResult? result, bool bStrict = false)
        {
            return IsSuccess(result, out HResult _, bStrict);
        }

        [PublicAPI]
        public static bool SUCCESS(Func<HResult> action, bool bStrict = false)
        {
            if (action == null)
            {
                return false;
            }

            return IsSuccess(action(), out HResult _, bStrict);
        }

        [PublicAPI]
        public static bool SUCCESS(Func<int> action, out int assignResultTo, bool bStrict = false)
        {
            if (action == null)
            {
                assignResultTo = unchecked((int)HResult.E_UNEXPECTED);
                return false;
            }

            return IsSuccess(action(), out assignResultTo, bStrict);
        }

        [PublicAPI]
        public static bool SUCCESS(Func<HResult> action, out int assignResultTo, bool bStrict = false)
        {
            if (action == null)
            {
                assignResultTo = unchecked((int)HResult.E_UNEXPECTED);
                return false;
            }

            return IsSuccess(action(), out assignResultTo, bStrict);
        }

        [PublicAPI]
        public static bool SUCCESS(Func<HResult> action, out HResult assignResultTo, bool bStrict = false)
        {
            if (action == null)
            {
                assignResultTo = HResult.E_UNEXPECTED;
                return false;
            }

            return IsSuccess(action(), out assignResultTo, bStrict);
        }

        /// <summary>
        /// Returns whether <paramref name="result"/> is an unsuccessful HResult code.
        /// A null value will be considered a success with <see cref="HResult.E_UNEXPECTED"/>.
        /// </summary>
        /// <param name="result">The result of the COM operation.</param>
        [PublicAPI]
        public static bool FAILED(int? result)
        {
            return !IsSuccess(result, out _);
        }

        [PublicAPI]
        public static bool FAILED(Func<int> action)
        {
            if (action == null)
            {
                return false;
            }

            return !IsSuccess(action(), out _);
        }

        /// <summary>
        /// Returns whether <paramref name="result"/> is an unsuccessful HResult code.
        /// A null value will be considered a success with <see cref="HResult.E_UNEXPECTED"/>.
        /// </summary>
        /// <param name="result">The result of the COM operation.</param>
        [PublicAPI]
        public static bool FAILED(HResult? result)
        {
            return !IsSuccess(result, out HResult _);
        }

        [PublicAPI]
        public static bool FAILED(Func<HResult> action)
        {
            if (action == null)
            {
                return false;
            }

            return !IsSuccess(action(), out HResult _);
        }

        [PublicAPI]
        public static bool FAILED(Func<int> action, out int assignResultTo)
        {
            if (action == null)
            {
                assignResultTo = unchecked((int)HResult.E_UNEXPECTED);
                return true;
            }

            return !IsSuccess(action(), out assignResultTo);
        }

        [PublicAPI]
        public static bool FAILED(Func<HResult> action, out int assignResultTo)
        {
            if (action == null)
            {
                assignResultTo = unchecked((int)HResult.E_UNEXPECTED);
                return true;
            }

            return !IsSuccess(action(), out assignResultTo);
        }

        [PublicAPI]
        public static bool FAILED(Func<HResult> action, out HResult assignResultTo)
        {
            if (action == null)
            {
                assignResultTo = HResult.E_UNEXPECTED;
                return true;
            }

            return !IsSuccess(action(), out assignResultTo);
        }

        [SuppressMessage("ReSharper", "RedundantAssignment")]
        private static bool IsSuccess(int? result, out int assignResultTo, bool bStrict = false)
        {
            assignResultTo = result ?? unchecked((int)HResult.E_UNEXPECTED);
            return !bStrict ? assignResultTo >= 0 : assignResultTo == 0;
        }

        [SuppressMessage("ReSharper", "RedundantAssignment")]
        private static bool IsSuccess(HResult? result, out int assignResultTo, bool bStrict = false)
        {
            assignResultTo = result.HasValue ? unchecked((int)result) : unchecked((int)HResult.E_UNEXPECTED);
            return !bStrict ? assignResultTo >= 0 : assignResultTo == 0;
        }

        [SuppressMessage("ReSharper", "RedundantAssignment")]
        private static bool IsSuccess(HResult? result, out HResult assignResultTo, bool bStrict = false)
        {
            assignResultTo = result ?? HResult.E_UNEXPECTED;
            return !bStrict ? result >= HResult.S_OK : result == HResult.S_OK;
        }
    }
}
