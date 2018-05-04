using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices.ComTypes;
using DirectShowLib;
using FluentAssertions;
using Foundary;
using FoundaryMediaPlayer.Interop.Windows;
using Microsoft.Win32;
using HResult = PInvoke.HResult.Code;

namespace FoundaryMediaPlayer.Engine
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class FFilterRegistry : AFilterBase
    {
        /// <summary>
        /// The managed definition of the <see cref="IMoniker"/> interface.
        /// </summary>
        public IMoniker Moniker {get;}

        public FFilterRegistry(IMoniker moniker, Merit merit = Merit.DoNotUse)
            : this(Guid.Empty, merit: merit)
        {
            moniker.Should().NotBeNull();

            Moniker = moniker;

            Moniker.GetDisplayName(null, null, out string displayName);
            Name = displayName;

            QueryProperties();
        }

        /// <inheritdoc />
        public FFilterRegistry(Guid guid, string name = "", Merit merit = Merit.DoNotUse) 
            : base(guid, name, merit)
        {
            if (guid == Guid.Empty)
            {
                return;
            }

            string filterName;
            using (var key = new RegistryKeyReference($"CLSID\\{guid}", RegistryHive.ClassesRoot))
            {
                filterName = key.GetString(null);
            }

            if (!string.IsNullOrEmpty(filterName))
            {
                using (var key = new RegistryKeyReference($"CLSID\\{{083863F1-70DE-11d0-BD40-00A0C911CE86}}\\Instance\\{guid}"))
                {
                    Name = string.IsNullOrWhiteSpace(name) ? (key.GetString("FriendlyName") ?? Name) : Name;
                    var bytes = key.GetBinary("FilterData");
                    ExtractFilterData(bytes);
                }
            }
        }

        /// <inheritdoc />
        public override int Create(out IBaseFilter baseFilter, out IList<object> unknowns)
        {
            HResult comResult = HResult.E_FAIL;
            unknowns = new List<object>();

            if (Moniker != null)
            {
                Moniker.BindToObject(null, null, WindowsInterop.GetCLSID<IBaseFilter>(), out object boundObject);
                if (boundObject != null)
                {
                    GUID = WindowsInterop.GetCLSID(boundObject);
                    baseFilter = boundObject as IBaseFilter;
                    comResult = HResult.S_OK;
                }
                else
                {
                    baseFilter = null;
                }
            }
            else if (GUID != Guid.Empty)
            {
                try
                {
                    baseFilter = WindowsInterop.CoCreateInstance(GUID) as IBaseFilter;
                    comResult = HResult.S_OK;
                }
                catch
                {
                    baseFilter = null;
                    comResult = HResult.E_FAIL;
                }
            }
            else
            {
                baseFilter = null;
            }

            return CastResult(comResult);
        }

        private void QueryProperties()
        {
            Guid pbclsid = WindowsInterop.GetCLSID<IPropertyBag>();
            Moniker.BindToStorage(null, null, ref pbclsid, out object propertyBagRaw);
            if (propertyBagRaw is IPropertyBag propertyBag)
            {
                if (SUCCESS(propertyBag.Read("FriendlyName", out object fname, null)))
                {
                    Name = fname as string ?? Name;
                }

                if (SUCCESS(propertyBag.Read("CLSID", out object clsid, null)) && 
                    clsid is string s && Guid.TryParse(s, out Guid guid))
                {
                    GUID = guid;
                }

                if (SUCCESS(propertyBag.Read("FilterData", out object filterData, null)))
                {
                    ExtractFilterData(filterData as byte[]);
                }
            }
        }

        private void ExtractFilterData(byte[] data)
        {
            //IAMFilterData filterData = WindowsInterop.CoCreateInstance(IID.FilterMapper2);
            //if (filterData.ParseFilterData(data))
            {
                //TODO: FGFilter 284
                throw new NotImplementedException();
            }
        }
    }
}
