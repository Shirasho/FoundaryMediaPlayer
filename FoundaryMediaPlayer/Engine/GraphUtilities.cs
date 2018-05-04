using System;
using System.Collections.Generic;
using System.IO;
using DirectShowLib;
using FluentAssertions;
using Foundary.Extensions;
using FoundaryMediaPlayer.Interop.Windows;

namespace FoundaryMediaPlayer.Engine
{
    public static class GraphUtilities
    {
        public static IBaseFilter GetFilterFromPin(IPin pPin)
        {
            if (pPin == null)
            {
                return null;
            }

            IBaseFilter result = null;
            if (pPin.QueryPinInfo(out var pinInfo) >= 0)
            {
                result = pinInfo.filter;
            }
            return result;
        }

        public static IBaseFilter GetUpstreamFilter(IBaseFilter baseFilter, IPin inputPin = null)
        {
            return GetFilterFromPin(GetUpstreamPin(baseFilter, inputPin));
        }

        public static IPin GetUpstreamPin(IBaseFilter baseFilter, IPin inputPin)
        {
            foreach (var pin in EnumPins(baseFilter))
            {
                if (inputPin != null && inputPin != pin)
                {
                    continue;
                }

                if (pin.QueryDirection(out var dir) >= 0 && dir == PinDirection.Input && pin.ConnectedTo(out var pinConnectedTo) >= 0)
                {
                    return pinConnectedTo;
                }
            }

            return null;
        }

        public static IReadOnlyList<Guid> ExtractMediaTypes(IPin pin)
        {
            var mediaTypes = new List<Guid>();

            foreach (var mediaType in EnumMediaTypes(pin))
            {
                bool bFound = false;
                for (int i = 0; !bFound && i < mediaTypes.Count; i += 2)
                {
                    if (mediaTypes[i] == mediaType.majorType && mediaTypes[i + 1] == mediaType.subType)
                    {
                        bFound = true;
                    }
                }

                if (!bFound)
                {
                    mediaTypes.Add(mediaType.majorType);
                    mediaTypes.Add(mediaType.subType);
                }
            }

            return mediaTypes;
        }

        public static IEnumerable<IPin> EnumPins(IBaseFilter baseFilter)
        {
            if (baseFilter != null && baseFilter.EnumPins(out var enumPins) >= 0)
            {
                var pins = new IPin[] { null };
                while (enumPins.Next(1, pins, IntPtr.Zero) == 0)
                {
                    yield return pins[0];
                    pins[0] = null;
                }
            }
        }

        public static IPin GetFirstPin(IBaseFilter baseFilter, PinDirection direction)
        {
            if (baseFilter != null)
            {
                foreach (var pin in EnumPins(baseFilter))
                {
                    if (ComResult.SUCCESS(pin.QueryDirection(out PinDirection dir)) && direction == dir)
                    {
                        return pin;
                    }
                }
            }

            return null;
        }

        public static IEnumerable<AMMediaType> EnumMediaTypes(IPin pin)
        {
            if (pin != null && ComResult.SUCCESS(pin.EnumMediaTypes(out IEnumMediaTypes enumMediaTypes)))
            {
                var mediaTypes = new AMMediaType[] { null };
                while (ComResult.SUCCESS(enumMediaTypes.Next(1, mediaTypes, IntPtr.Zero), true))
                {
                    yield return mediaTypes[0];
                    mediaTypes[0] = null;
                }
            }
        }

        public static IEnumerable<IBaseFilter> EnumFilters(IFilterGraph filterGraph)
        {
            if (filterGraph != null && ComResult.SUCCESS(filterGraph.EnumFilters(out var enumFilters)))
            {
                var filters = new IBaseFilter[] { null };
                while (ComResult.SUCCESS(enumFilters.Next(1, filters, IntPtr.Zero), true))
                {
                    yield return filters[0];
                    filters[0] = null;
                }
            }
        }

        public static IEnumerable<IBaseFilter> EnumCachedFilters(IGraphConfig graphConfig)
        {
            if (graphConfig != null && ComResult.SUCCESS(graphConfig.EnumCacheFilter(out var enumFilters)))
            {
                var filters = new IBaseFilter[] { null };
                while (ComResult.SUCCESS(enumFilters.Next(1, filters, IntPtr.Zero), true))
                {
                    yield return filters[0];
                    filters[0] = null;
                }
            }
        }

        public static string GetPinName(IPin pin)
        {
            if (pin != null && ComResult.SUCCESS(pin.QueryPinInfo(out PinInfo pinInfo)))
            {
                return pinInfo.name.Trim();
            }

            return string.Empty;
        }

        public static string GetFilterName(IBaseFilter filter)
        {
            if (filter != null && ComResult.SUCCESS(filter.QueryFilterInfo(out FilterInfo filterInfo)))
            {
                return filterInfo.achName.Trim();
            }

            return string.Empty;
        }

        public static bool CheckBytes(FileInfo file, string checkBytes)
        {
            if (file == null || string.IsNullOrEmpty(checkBytes))
            {
                return false;
            }

            var sl = new Queue<string>(checkBytes.Split(','));
            if (sl.Count < 4)
            {
                return false;
            }

            var size = file.Length;

            while (sl.Count >= 4)
            {
                var offsetStr = sl.Dequeue();
                var cbStr = sl.Dequeue();
                var maskStr = sl.Dequeue();
                var valStr = sl.Dequeue();

                long offset = long.Parse(offsetStr);
                long cb = long.Parse(cbStr);

                if (offset < 0)
                {
                    offset = size - offset;
                }

                if (string.IsNullOrEmpty(offsetStr) ||
                    string.IsNullOrEmpty(cbStr) ||
                    string.IsNullOrEmpty(valStr) ||
                    valStr.Length.IsOdd() ||
                    cb * 2 != valStr.Length)
                {
                    return false;
                }
                
                // LAME
                while (maskStr.Length < valStr.Length) {
                    maskStr += 'F';
                }

                var mask = StringToBin(maskStr);
                var val = StringToBin(valStr);

                using (var stream = file.OpenRead())
                {
                    for (int i = 0; i < val.Length; ++i)
                    {
                        var buffer = new byte[1];

                        try
                        {
                            var readBytes = stream.Read(buffer, (int) offset, 1);
                            if (readBytes == 0 || (buffer[0] & mask[i]) != val[i])
                            {
                                return false;
                            }
                        }
                        catch
                        {
                            return false;
                        }
                    }
                }
            }

            return sl.Count == 0;
        }

        public static byte[] StringToBin(string str)
        {
            str = str.Trim().ToUpper();
            str.Length.IsOdd().Should().BeFalse();
            
            var data = new byte[str.Length / 2];
            byte b = 0;

            for (int i = 0, j = str.Length; i < j; ++i) {
                var c = str[i];
                if (c >= '0' && c <= '9') {
                    if (i.IsEven()) {
                        b = (byte)((((c - '0') << 4) & 0xf0) | (b & 0x0f));
                    } else {
                        b = (byte)(((c - '0') & 0x0f) | (b & 0xf0));
                    }
                } else if (c >= 'A' && c <= 'F') {
                    if (i.IsEven()) {
                        b = (byte)((((c - 'A' + 10) << 4) & 0xf0) | (b & 0x0f));
                    } else {
                        b = (byte)(((c - 'A' + 10) & 0x0f) | (b & 0xf0));
                    }
                } else {
                    break;
                }

                if (i.IsOdd()) {
                    data[i >> 1] = b;
                    b = 0;
                }
            }

            return data;
        }
    }
}
