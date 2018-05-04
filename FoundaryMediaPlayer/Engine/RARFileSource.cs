using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using DirectShowLib;

namespace FoundaryMediaPlayer.Engine
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class FRARFileSource : AFilterBase, IFileSourceFilter
    {
        /// <inheritdoc />
        public FRARFileSource(Guid guid, string name = "", Merit merit = Merit.DoNotUse) 
            : base(guid, name, merit)
        {
        }

        /// <inheritdoc />
        public override int Create(out IBaseFilter baseFilter, out IList<object> unknowns)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public int Load(string pszFileName, AMMediaType pmt)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public int GetCurFile(out string pszFileName, AMMediaType pmt)
        {
            throw new NotImplementedException();
        }
    }
}
