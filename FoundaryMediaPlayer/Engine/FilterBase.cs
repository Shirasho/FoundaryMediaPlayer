using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DirectShowLib;
using Foundary.Extensions;
using FoundaryMediaPlayer.Interop.Windows;

namespace FoundaryMediaPlayer.Engine
{
    /// <summary>
    /// The base class for all filters.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public abstract class AFilterBase : AComObjectBase, IEquatable<AFilterBase>
    {
        public Guid GUID { get; protected set; }
        public string Name { get; protected set; }
        public IReadOnlyList<Guid> Types => _Types;

        public List<string> Protocols { get; } = new List<string>();
        public List<string> Extensions { get; } = new List<string>();
        public List<string> CheckBytes { get; } = new List<string>();

        public Merit Merit { get; }

        private List<Guid> _Types { get; set; } = new List<Guid>();

        protected AFilterBase(Guid guid, string name = "", Merit merit = Merit.DoNotUse)
        {
            GUID = guid;
            Name = name;
            Merit = merit;
        }

        public void SetTypes(IList<Guid> types)
        {
            _Types = new List<Guid>(types);
        }

        public void AddType(Guid majorType, Guid subType)
        {
            _Types.Add(majorType);
            _Types.Add(subType);
        }

        public bool CheckTypes(IReadOnlyList<Guid> inTypes, bool bExactMatch)
        {
            foreach (var mediaTypeEnumerable in Types.TakeIterator(2, false))
            {
                var mediaTypes = mediaTypeEnumerable as Guid[] ?? mediaTypeEnumerable.ToArray();
                var majorType = mediaTypes[0];
                var subType = mediaTypes[1];

                foreach (var inMediaTypeEnumerable in inTypes.TakeIterator(2, false))
                {
                    var inMediaTypes = inMediaTypeEnumerable as Guid[] ?? inMediaTypeEnumerable.ToArray();
                    var inMajorType = inMediaTypes[0];
                    var inSubType = inMediaTypes[1];

                    if (bExactMatch)
                    {
                        return majorType != Guid.Empty && majorType == inMajorType &&
                               subType != Guid.Empty && subType == inSubType;
                    }

                    return (majorType == Guid.Empty || inMajorType == Guid.Empty || majorType == inMajorType) &&
                           (subType == Guid.Empty || inSubType == Guid.Empty || subType == inSubType);
                }
            }

            return false;
        }

        public abstract int Create(out IBaseFilter baseFilter, out IList<object> unknowns);

        /// <inheritdoc />
        public bool Equals(AFilterBase other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            if (GUID != Guid.Empty &&
                GUID == other.GUID &&
                other.Merit == Merit.DoNotUse)
            {
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((AFilterBase) obj);
        }

        /// <inheritdoc />
        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = GUID.GetHashCode();
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Protocols != null ? Protocols.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Extensions != null ? Extensions.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (CheckBytes != null ? CheckBytes.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int) Merit;
                hashCode = (hashCode * 397) ^ (_Types != null ? _Types.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
