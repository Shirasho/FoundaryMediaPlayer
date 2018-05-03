using System;
using System.Collections.Generic;
using DirectShowLib;
using FluentAssertions;
using FoundaryMediaPlayer.Interop.Windows;

namespace FoundaryMediaPlayer.Engine
{
    public class FStreamPath
    {
        public Guid CLSID { get; set; }
        public string Filter { get; set; }
        public string Pin { get; set; }
    }

    public class FStreamPathCollection : List<FStreamPath>, IComparable<FStreamPathCollection>
    {
        public void Add(IBaseFilter filter, IPin pin)
        {
            filter.Should().NotBeNull();
            pin.Should().NotBeNull();

            Add(new FStreamPath
            {
                CLSID = WindowsInterop.GetCLSID(filter),
                Filter = GetFilterName(filter),
                Pin = GetPinName(pin)
            });
        }

        /// <inheritdoc />
        public int CompareTo(FStreamPathCollection other)
        {
            //TODO: Whot?
            //POSITION pos1 = GetHeadPosition();
            //POSITION pos2 = path.GetHeadPosition();

            //while (pos1 && pos2) {
            //    const path_t& p1 = GetNext(pos1);
            //    const path_t& p2 = path.GetNext(pos2);

            //    if (p1.filter != p2.filter) {
            //        return true;
            //    } else if (p1.pin != p2.pin) {
            //        return false;
            //    }
            //}

            //return true;

            return 0;
        }
    }

    public class FStreamDeadEndCollection : FStreamPathCollection
    {
        public List<AMMediaType> MediaTypes { get; } = new List<AMMediaType>();
    }
}
