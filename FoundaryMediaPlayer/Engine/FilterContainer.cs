using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Foundary.Extensions;

namespace FoundaryMediaPlayer.Engine
{
    public sealed class FFilterContainer : IComparable<FFilterContainer>, IEquatable<FFilterContainer>
    {
        public AFilterBase Filter {get; set;}
        public int Group {get; set;}
        public bool bExactMatch {get; set;}

        /// <inheritdoc />
        public int CompareTo(FFilterContainer other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;

            var groupComparison = Group.CompareTo(other.Group);
            if (groupComparison != 0) return groupComparison;

            var meritComparison = Filter.Merit.CompareTo(other.Filter.Merit);
            if (meritComparison != 0) return meritComparison;

            if (Filter.GUID == other.Filter.GUID)
            {
                FFilterFile a = Filter as FFilterFile;
                FFilterFile b = other.Filter as FFilterFile;

                if (a != null && b == null)
                {
                    return -1;
                }

                if (a == null && b != null)
                {
                    return 1;
                }
            }

            if (bExactMatch && !other.bExactMatch)
            {
                return -1;
            }

            if (!bExactMatch && other.bExactMatch)
            {
                return 1;
            }

            return 0;
        }

        /// <inheritdoc />
        public bool Equals(FFilterContainer other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Filter, other.Filter) && Group == other.Group;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is FFilterContainer container && Equals(container);
        }

        /// <inheritdoc />
        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Filter != null ? Filter.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Group;
                hashCode = (hashCode * 397) ^ bExactMatch.GetHashCode();
                return hashCode;
            }
        }
    }

    /// <summary>
    /// A sorted collection of filters. Filters will automatically be sorted using
    /// internal rules any time a new item is added.
    /// </summary>
    public sealed class FFilterContainerCollection : List<FFilterContainer>
    {
        public new void Add(FFilterContainer item)
        {
            this.AddUnique(item);
            Sort();
        }

        public void Add(AFilterBase filter, int group, bool bExactMatch)
        {
            filter.Should().NotBeNull();

            bool bInsert = true;

            var filterRegistry = filter as FFilterRegistry;

            foreach (var internalFilter in this)
            {
                // Reject filter duplicates.
                if (filter.Equals(internalFilter.Filter))
                {
                    bInsert = false;
                    break;
                }

                if (group != internalFilter.Group)
                {
                    continue;
                }

                // Reject duplicate moniker.
                if (internalFilter.Filter is FFilterRegistry filterRegistry2 &&
                    filterRegistry?.Moniker?.IsEqual(filterRegistry2.Moniker) >= 0)
                {
                    bInsert = false;
                    break;
                }
            }

            if (bInsert)
            {
                Add(new FFilterContainer
                {
                    bExactMatch = bExactMatch,
                    Filter = filter,
                    Group = group
                });

                Sort();
            }
        }
    }
}
