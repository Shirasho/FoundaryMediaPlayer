using System;
using System.Diagnostics.CodeAnalysis;

namespace FoundaryMediaPlayer.Engine.Windows
{
    /// <summary>
    /// A chapter.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class DSMChapter : IComparable<DSMChapter>, IEquatable<DSMChapter>
    {
        private static int Counter { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Order { get; }

        /// <summary>
        /// 
        /// </summary>
        public long Time { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DSMChapter()
            : this(null, 0)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="time"></param>
        public DSMChapter(string name, long time)
        {
            Order = Counter++;
            Name = name;
            Time = time;
        }

        /// <inheritdoc />
        public int CompareTo(DSMChapter other)
        {
            if (Time > other.Time)
            {
                return 1;
            }

            if (Time < other.Time)
            {
                return -1;
            }

            return Order - other.Order;
        }

        /// <inheritdoc />
        public bool Equals(DSMChapter other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Order == other.Order && Time == other.Time && string.Equals(Name, other.Name);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((DSMChapter) obj);
        }

        /// <inheritdoc />
        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Order;
                hashCode = (hashCode * 397) ^ Time.GetHashCode();
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
