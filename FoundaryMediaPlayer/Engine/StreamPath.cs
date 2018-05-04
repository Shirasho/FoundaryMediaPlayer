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

        public bool Compare(FStreamPath other)
        {
            if (Filter != other.Filter)
            {
                return true;
            }

            if (Pin != other.Pin)
            {
                return false;
            }

            return true;
        }
    }

    public class FStreamPathCollection : List<FStreamPath>
    {
        public void Add(IBaseFilter filter, IPin pin)
        {
            filter.Should().NotBeNull();
            pin.Should().NotBeNull();

            Add(new FStreamPath
            {
                CLSID = WindowsInterop.GetCLSID(filter),
                Filter = GGraphUtilities.GetFilterName(filter),
                Pin = GGraphUtilities.GetPinName(pin)
            });
        }
    }

    public class FStreamDeadEndCollection : FStreamPathCollection
    {
        public List<AMMediaType> MediaTypes { get; } = new List<AMMediaType>();
    }
}
