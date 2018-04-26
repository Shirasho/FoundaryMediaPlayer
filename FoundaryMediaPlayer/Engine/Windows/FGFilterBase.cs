using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DirectShowLib;
using Foundary.Extensions;

namespace FoundaryMediaPlayer.Engine.Windows
{
    /// <summary>
    /// The base class for all FG filters.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal abstract class FGFilterBase
    {
        public Guid CLSID { get; }
        public string Name { get; }
        public IReadOnlyList<Guid> Types => _Types;

        public Merit Merit { get; private set; }

        private List<Guid> _Types { get; set; } = new List<Guid>();

        protected FGFilterBase(Guid clsid, string name = "", Merit merit = Merit.DoNotUse)
        {
            CLSID = clsid;
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
    }
}
