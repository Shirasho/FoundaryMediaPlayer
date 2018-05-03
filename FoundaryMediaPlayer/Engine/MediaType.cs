using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using DirectShowLib;
using FoundaryMediaPlayer.Interop.Windows;

namespace FoundaryMediaPlayer.Engine
{
    [StructLayout(LayoutKind.Sequential)]
    public sealed class FMediaType : AMMediaType, IEquatable<FMediaType>
    {
        public FMediaType()
            : this(Guid.Empty, Guid.Empty)
        {

        }

        public FMediaType(Guid majorType)
            : this(majorType, Guid.Empty)
        {

        }

        public FMediaType(Guid majorType, Guid subType)
        {
            this.majorType = majorType;
            this.subType = subType;

            sampleSize = 1;
            fixedSizeSamples = true;
        }

        public bool IsValid()
        {
            return majorType != Guid.Empty;
        }

        public bool IsPartiallyValid()
        {
            return majorType == Guid.Empty ||
                   subType == Guid.Empty;
        }

        public bool IsInvalid()
        {
            return majorType == Guid.Empty &&
                   subType == Guid.Empty;
        }

        public bool IsPartialMatch(FMediaType other)
        {
            if (other == null ||
                (other.majorType != Guid.Empty && majorType != other.majorType) ||
                (other.subType != Guid.Empty && subType != other.subType))
            {
                return false;
            }

            // Format block must match exactly if specified.
            if (other.formatType != Guid.Empty)
            {
                if (formatType != other.formatType ||
                    formatSize != other.formatSize)
                {
                    return false;
                }

                byte[] contents1 = new byte[formatSize];
                byte[] contents2 = new byte[other.formatSize];

                Marshal.Copy(formatPtr, contents1, 0, formatSize);
                Marshal.Copy(other.formatPtr, contents2, 0, other.formatSize);

                return WindowsInterop.MemoryCompare(contents1, contents2, formatSize) == 0;
            }

            return true;
        }

        public int GetSampleSize()
        {
            return fixedSizeSamples ? sampleSize : 0;
        }

        public void SetSampleSize(int size)
        {
            if (size == 0)
            {
                fixedSizeSamples = false;
            }
            else
            {
                fixedSizeSamples = true;
                sampleSize = size;
            }
        }

        /// <inheritdoc />
        public bool Equals(FMediaType other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            bool bCondition1 = majorType == other.majorType &&
                               subType == other.subType &&
                               formatType == other.formatType &&
                               formatSize == other.formatSize;

            if (!bCondition1)
            {
                return false;
            }

            if (formatSize == 0)
            {
                return true;
            }

            byte[] contents1 = new byte[formatSize];
            byte[] contents2 = new byte[other.formatSize];

            Marshal.Copy(formatPtr, contents1, 0, formatSize);
            Marshal.Copy(other.formatPtr, contents2, 0, other.formatSize);

            return WindowsInterop.MemoryCompare(contents1, contents2, formatSize) == 0;

        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is FMediaType type && Equals(type);
        }

        /// <inheritdoc />
        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = majorType.GetHashCode();
                hashCode = (hashCode * 397) ^ subType.GetHashCode();
                hashCode = (hashCode * 397) ^ formatType.GetHashCode();
                hashCode = (hashCode * 397) ^ formatSize;
                return hashCode;
            }
        }
    }
}
