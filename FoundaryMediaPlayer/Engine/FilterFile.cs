using System;
using System.Collections.Generic;
using DirectShowLib;

namespace FoundaryMediaPlayer.Engine
{
    public class FFilterFile : AFilterBase
    {
        /// <inheritdoc />
        public FFilterFile(Guid guid, string name = "", Merit merit = Merit.DoNotUse) 
            : base(guid, name, merit)
        {
        }

        /// <inheritdoc />
        public override int Create(out IBaseFilter baseFilter, out IList<object> unknowns)
        {
            throw new NotImplementedException();
        }
    }
}
