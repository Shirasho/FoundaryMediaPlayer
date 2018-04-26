#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using DirectShowLib;
using FluentAssertions;
using Foundary.Extensions;
using FoundaryMediaPlayer.Platforms.Windows;
using log4net;
using HResult = PInvoke.HResult.Code;

namespace FoundaryMediaPlayer.Engine.Windows
{
    /// <summary>
    /// Filter graph manager. <see cref="IGraphBuilder2"/> impl.
    /// </summary>
    //CFManagerPlayer
    [Guid("09056CF8-B199-4E2E-9FE7-8EFCCA65E3EB")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComSourceInterfaces(typeof(IGraphBuilder2), typeof(IGraphBuilderDeadEnd))]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class FGGraph : IGraphBuilder2, IGraphBuilderDeadEnd
    {
        private static ILog _Logger { get; } = LogManager.GetLogger(typeof(FGGraph));

        private IFilterGraph _FilterGraph { get; }
        private IFilterGraph2 _FilterGraph2 => _FilterGraph as IFilterGraph2;
        private IFilterMapper2 _FilterMapper { get; }

        private IList<FGFilterBase> _SourceFilters { get; } = new List<FGFilterBase>();
        private IList<FGFilterBase> _TransformFilters { get; } = new List<FGFilterBase>();
        private IList<FGFilterBase> _OverrideFilters { get; } = new List<FGFilterBase>();

        private IList<object> _Unknowns { get; } = new List<object>();

        /// <summary>
        /// 
        /// </summary>
        public FGGraph()
        {
            _FilterGraph = Native.CoCreateInstance<IFilterGraph>(CLSID.FilterGraph);
            _FilterMapper = Native.CoCreateInstance<IFilterMapper2>(CLSID.FilterMapper2);
        }


        ~FGGraph()
        {
            //TODO: Dipose items?
            _SourceFilters.Clear();
            _TransformFilters.Clear();
            _OverrideFilters.Clear();

            _Unknowns.Clear();

            Native.CoRelease(_FilterGraph);
        }

        public int AddFilter(IBaseFilter pFilter, string pName)
        {
            if (_FilterGraph2 == null)
            {
                return HResult.E_UNEXPECTED.AsInt();
            }

            lock (this)
            {
                var result = _FilterGraph2.AddFilter(pFilter, pName);
                if (!result.IsSuccess())
                {
                    return result;
                }

                pFilter.JoinFilterGraph(null, null);
                return pFilter.JoinFilterGraph(this, pName);
            }
        }

        public int RemoveFilter(IBaseFilter pFilter)
        {
            if (_FilterGraph2 == null)
            {
                return HResult.E_UNEXPECTED.AsInt();
            }

            lock (this)
            {
                return _FilterGraph2.RemoveFilter(pFilter);
            }
        }


        public int EnumFilters(out IEnumFilters ppEnum)
        {
            if (_FilterGraph2 == null)
            {
                ppEnum = null;
                return HResult.E_UNEXPECTED.AsInt();
            }
            return _FilterGraph2.EnumFilters(out ppEnum);
        }


        public int FindFilterByName(string pName, out IBaseFilter ppFilter)
        {
            if (_FilterGraph2 == null)
            {
                ppFilter = null;
                return HResult.E_UNEXPECTED.AsInt();
            }

            lock (this)
            {
                return _FilterGraph2.FindFilterByName(pName, out ppFilter);
            }
        }


        public int ConnectDirect(IPin ppinOut, IPin ppinIn, AMMediaType pmt)
        {
            if (_FilterGraph2 == null)
            {
                return unchecked((int)HResult.E_UNEXPECTED);
            }

            lock (this)
            {
                var baseFilter = GetFilterFromPin(ppinIn);
                var clsid = CLSID.GetCLSID(baseFilter);

                var baseFilterUpstream = GetFilterFromPin(ppinOut);
                do
                {
                    if (baseFilterUpstream == baseFilter)
                    {
                        return unchecked((int)HResult.E_FAIL);
                    }

                    if (clsid != CLSID.Proxy && CLSID.GetCLSID(baseFilterUpstream) == clsid)
                    {
                        return unchecked((int)HResult.E_FAIL);
                    }

                    baseFilterUpstream = GetUpstreamFilter(baseFilterUpstream);
                } while (baseFilterUpstream != null);

                return _FilterGraph2.ConnectDirect(ppinOut, ppinIn, pmt);
            }
        }


        public int Reconnect(IPin ppin)
        {
            if (_FilterGraph2 == null)
            {
                return HResult.E_UNEXPECTED.AsInt();
            }

            lock (this)
            {
                return _FilterGraph2.Reconnect(ppin);
            }
        }


        public int Disconnect(IPin ppin)
        {
            if (_FilterGraph2 == null)
            {
                return HResult.E_UNEXPECTED.AsInt();
            }

            lock (this)
            {
                return _FilterGraph2.Disconnect(ppin);
            }
        }


        public int SetDefaultSyncSource()
        {
            if (_FilterGraph2 == null)
            {
                return HResult.E_UNEXPECTED.AsInt();
            }

            lock (this)
            {
                return _FilterGraph2.SetDefaultSyncSource();
            }
        }


        public int Connect(IPin ppinOut, IPin ppinIn)
        {
            return Connect(ppinOut, ppinIn, true);
        }

        public int Connect(IPin ppinOut, IPin ppinIn, bool bContinueRender)
        {
            lock (this)
            {
                if (ppinOut == null)
                {
                    return HResult.E_POINTER.AsInt();
                }

                if (IsPinDirection(ppinOut, PinDirection.Output).IsSuccess() ||
                    (ppinIn != null && IsPinDirection(ppinIn, PinDirection.Input).IsSuccess()))
                {
                    return HResult.E_FAIL.AsInt();
                }

                if (!IsPinConnected(ppinOut).IsSuccess() ||
                    (ppinIn != null && IsPinConnected(ppinIn).IsSuccess(true)))
                {
                    return unchecked((int) HResult.E_FAIL);
                }

                int result;
                bool bDeadEnd = true;

                // 1. Try a direct connection between the filters with no intermediate filters.
                {
                    if (ppinIn != null)
                    {
                        result = ConnectDirect(ppinOut, ppinIn, null);
                        if (result.IsSuccess())
                        {
                            return result;
                        }
                    }
                    else
                    {
                        if (ppinOut is IStreamBuilder streamBuilder)
                        {
                            result = streamBuilder.Render(ppinOut, this);
                            if (result.IsSuccess())
                            {
                                return result;
                            }

                            streamBuilder.Backout(ppinOut, this);
                        }
                    }
                }

                // 2. Try cached filters.
                {
                    if (this is IGraphConfig graphConfig)
                    {
                        foreach (var filter in EnumCachedFilters(graphConfig))
                        {
                            if (ppinIn != null && GetFilterFromPin(ppinIn) == filter)
                            {
                                continue;
                            }

                            graphConfig.RemoveFilterFromCache(filter);

                            if (ConnectFilterDirect(out ppinOut, filter, null).IsSuccess())
                            {
                                if (!IsStreamEnd(filter))
                                {
                                    bDeadEnd = false;
                                }

                                if ((result = ConnectFilter(filter, ppinIn)).IsSuccess())
                                {
                                    return result;
                                }
                            }

                            result = graphConfig.AddFilterToCache(filter);
                        }
                    }
                }

                // 3. Try filters in the graph.
                {
                    var filters = new List<IBaseFilter>();

                    foreach (var filter in EnumFilters(this))
                    {
                        if ((ppinIn != null && GetFilterFromPin(ppinIn) == filter) ||
                            GetFilterFromPin(ppinOut) == filter)
                        {
                            continue;
                        }

                        // TODO: This is a hack. Move to CLSID.
                        // ffdshow
                        if (CLSID.GetCLSID(ppinOut) == new Guid("04FE9017-F873-410E-871E-AB91661A4EF7") &&
                            CLSID.GetCLSID(filter) == new Guid("E30629D2-27E5-11CE-875D-00608CB78066"))
                        {
                            continue;
                        }

                        filters.Add(filter);
                    }

                    foreach (var filter in filters)
                    {
                        if ((result = ConnectFilterDirect(out ppinOut, filter, null)).IsSuccess())
                        {
                            if (!IsStreamEnd(filter))
                            {
                                bDeadEnd = false;
                            }

                            if ((result = ConnectFilter(filter, ppinIn)).IsSuccess())
                            {
                                return result;
                            }
                        }

                        Disconnect(ppinOut).Should().Be((int) HResult.S_OK);
                    }
                }

                // 4. Look up filters in the registry.
                {
                    var filterList = new FGFilterList();
                    var mediaTypes = ExtractMediaTypes(ppinOut);

                    foreach (var filter in _TransformFilters)
                    {
                        if (filter.Merit < Merit.DoNotUse || filter.CheckTypes(mediaTypes, false))
                        {
                            filterList.Insert(filter, 0, filter.CheckTypes(mediaTypes, true), false);
                        }
                    }

                    foreach (var filter in _OverrideFilters)
                    {
                        if (filter.Merit < Merit.DoNotUse || filter.CheckTypes(mediaTypes, false))
                        {
                            filterList.Insert(filter, 0, filter.CheckTypes(mediaTypes, true), false);
                        }
                    }

                    if (mediaTypes.Count != 0 &&
                        _FilterMapper.EnumMatchingFilters(
                            out IEnumMoniker enumMoniker, 0, false, Merit.DoNotUse, true,
                            mediaTypes.Count / 2, mediaTypes.ToArray(), null, null, false,
                            ppinIn != null, 0, null, null, null).IsSuccess())
                    {
                        var monikers = new IMoniker[1];
                        while (enumMoniker.Next(1, monikers, IntPtr.Zero).IsSuccess())
                        {
                            var filterRegistry = new FGFilterRegistery(monikers[0]);
                            filterList.Insert(filterRegistry, 0, filterRegistry.CheckTypes(mediaTypes, true));
                        }
                    }

                    // Let's check whether the madVR allocator presenter is in our list.
                    // It should be if madVR is selected as the video renderer.
                    FGFilterBase madVRAllocatorPresenter = null;
                    foreach (var filter in filterList)
                    {
                        if (filter.CLSID == CLSID.MadVRAllocatorPresenter)
                        {
                            madVRAllocatorPresenter = filter;
                            break;
                        }
                    }

                    const string madVRRendererName = "madVR Renderer";
                    int madVRIndex = -1;
                    for (int i = 0; i < filterList.Count; ++i)
                    {
                        FGFilterBase filter = filterList[i];
                        // Prevent duplicate madVR entries in the graph.
                        FindFilterByName(madVRRendererName, out IBaseFilter madVRFilter);
                        if (madVRFilter != null && filter.Name == madVRRendererName)
                        {
                            continue;
                        }

                        if (madVRAllocatorPresenter != null && filter.CLSID == CLSID.MadVR)
                        {
                            madVRIndex = i;

                            // Add a "temporary reference" for ease of use later.
                            // We will actually overwrite the original pointer in a bit.
                            filter = madVRAllocatorPresenter;
                        }

                        _Logger.Debug($"{nameof(FGGraph)} --> Connecting filter {filter.Name}.");

                        if (!filter.Create(out IBaseFilter baseFilter, out IList<object> Unknowns).IsSuccess())
                        {
                            _Logger.Warn($"{nameof(FGGraph)} --> Unable to create filter {filter.Name}.");
                        }

                        if (!(result = AddFilter(baseFilter, filter.Name)).IsSuccess())
                        {
                            _Logger.Warn($"{nameof(FGGraph)} --> Unable to create filter {filter.Name}.");
                            continue;
                        }

                        result = ConnectFilterDirect(out ppinOut, baseFilter, null);
                        if (result.IsSuccess())
                        {
                            if (!IsStreamEnd(baseFilter))
                            {
                                bDeadEnd = false;
                            }

                            if (bContinueRender)
                            {
                                result = ConnectFilter(baseFilter, ppinIn);
                            }

                            if (result.IsSuccess())
                            {
                                _Unknowns.AddRange(Unknowns);
                                var mixerPinConfig = Unknowns.FirstOrDefault(unk => unk is IMixerPinConfig && CLSID.GetCLSID(unk) == CLSID.IMixerPinConfig, null);
                                ((IMixerPinConfig) mixerPinConfig)?.SetAspectRatioMode(AspectRatioMode.Stretched);

                                if (baseFilter is IVMRAspectRatioControl arc)
                                {
                                    arc.SetAspectRatioMode(VMRAspectRatioMode.None);
                                }

                                if (baseFilter is IVMRAspectRatioControl9 arc9)
                                {
                                    arc9.SetAspectRatioMode(VMRAspectRatioMode.None);
                                }

                                if (baseFilter is IVMRMixerControl9 mc9)
                                {
                                    _Unknowns.Add(mc9);
                                }

                                if (baseFilter is IVMRMixerBitmap9 mb9)
                                {
                                    _Unknowns.Add(mb9);
                                }

                                if (baseFilter is IMFGetService mfgs && CLSID.GetCLSID(mfgs) == CLSID.GetCLSIDOf<IMFGetService>())
                                {
                                    CComPtr<IMFVideoDisplayControl> pMFVDC;
                                    CComPtr<IMFVideoMixerBitmap> pMFMB;
                                    CComPtr<IMFVideoProcessor> pMFVP;

                                    if (mfgs.GetService(MR_VIDEO_RENDER_SERVICE, IID_PPV_ARGS(&pMFVDC)).IsSuccess())
                                    {
                                        _Unknowns.Add(pMFVDC);
                                    }

                                    if (mfgs.GetService(MR_VIDEO_MIXER_SERVICE, IID_PPV_ARGS(&pMFMB)).IsSuccess())
                                    {
                                        _Unknowns.Add(pMFMB);
                                    }

                                    if (mfgs.GetService(MR_VIDEO_MIXER_SERVICE, IID_PPV_ARGS(&pMFVP)).IsSuccess())
                                    {
                                        _Unknowns.Add(pMFVP);
                                    }

                                    if (madVRAllocatorPresenter != null)
                                    {
                                        // Hook DXVA to have status and logging.
                                        CComPtr<IDirectXVideoDecoderService> pDecoderService;
                                        CComPtr<IDirect3DDeviceManager9> pDeviceManager;
                                        HANDLE hDevice = INVALID_HANDLE_VALUE;

                                        if (mfgs->GetService(MR_VIDEO_ACCELERATION_SERVICE, IID_PPV_ARGS(&pDeviceManager)).IsSuccess()
                                            && pDeviceManager->OpenDeviceHandle(&hDevice).IsSuccess()
                                            && pDeviceManager->GetVideoService(hDevice, IID_PPV_ARGS(&pDecoderService)).IsSuccess())
                                        {
                                            HookDirectXVideoDecoderService(pDecoderService);
                                            pDeviceManager->CloseDeviceHandle(hDevice);
                                        }
                                    }
                                }

                                return result;
                            }
                        }
                    }

                    if (madVRIndex >= 0)
                    {
                        // The pure madVR filter was selected (without the allocator presenter)
                        // subtitles, OSD etc don't work correctly without the allocator presenter
                        // so we prefer the allocator presenter over the pure filter.
                        filterList[madVRIndex] = madVRAllocatorPresenter;
                    }
                }

                if (bDeadEnd)
                {
                    var streamDeadEnd = new StreamDeadEnd();
                    streamDeadEnd.Add(m_streamPath);
                    int skip = 0;
                    foreach (var mediaType in EnumMediaTypes(ppinOut))
                    {
                        if (mediaType.majorType == MEDIATYPE_Stream && mediaType.subType == MEDIASUBTYPE_NULL)
                        {
                            ++skip;
                        }

                        streamDeadEnd.MediaTypes.Add(mediaType);
                    }

                    if (skip < (int)streamDeadEnd.MediaTypes.Length) {
                        StreamDeadEnds.Add(streamDeadEnd);
                    }
                }

                return ppinIn != null ? unchecked((int)0x80040217) : unchecked((int)0x80040218);
            }
        }


        public int Render(IPin ppinOut)
        {
            throw new NotImplementedException();
        }


        public int RenderFile(string lpcwstrFile, string lpcwstrPlayList)
        {
            throw new NotImplementedException();
        }


        public int AddSourceFilter(string lpcwstrFileName, string lpcwstrFilterName, out IBaseFilter ppFilter)
        {
            throw new NotImplementedException();
        }


        public int SetLogFile(IntPtr hFile)
        {
            throw new NotImplementedException();
        }


        public int Abort()
        {
            throw new NotImplementedException();
        }


        public int ShouldOperationContinue()
        {
            throw new NotImplementedException();
        }


        public int AddSourceFilterForMoniker(IMoniker pMoniker, IBindCtx pCtx, string lpcwstrFilterName, out IBaseFilter ppFilter)
        {
            throw new NotImplementedException();
        }


        public int ReconnectEx(IPin ppin, AMMediaType pmt)
        {
            throw new NotImplementedException();
        }


        public int RenderEx(IPin pPinOut, AMRenderExFlags dwFlags, IntPtr pvContext)
        {
            throw new NotImplementedException();
        }


        public int IsPinDirection(IPin pPin, PinDirection dir)
        {
            throw new NotImplementedException();
        }


        public int IsPinConnected(IPin pPin)
        {
            throw new NotImplementedException();
        }


        public int ConnectFilter(IBaseFilter pBF, IPin pPinIn)
        {
            throw new NotImplementedException();
        }


        public int ConnectFilter(out IPin pPinOut, IBaseFilter pBF)
        {
            throw new NotImplementedException();
        }


        public int ConnectFilterDirect(out IPin pPinOut, IBaseFilter pBF, AMMediaType pmt)
        {
            throw new NotImplementedException();
        }


        public int NukeDownstream(object pUnk)
        {
            throw new NotImplementedException();
        }


        public int FindInterface(ref Guid iid, out IntPtr ppv, bool bRemove)
        {
            throw new NotImplementedException();
        }


        public int AddToROT()
        {
            throw new NotImplementedException();
        }


        public int RemoveFromROT()
        {
            throw new NotImplementedException();
        }


        public uint GetCount()
        {
            throw new NotImplementedException();
        }


        public int GetDeadEnd(int iIndex, IList<string> path, IList<MediaType> mts)
        {
            throw new NotImplementedException();
        }

        private int CountPins(IBaseFilter filter, out int nIn, out int nOut, out int nInC, out int nOutC)
        {
            nIn = nOut = nInC = nOutC = 0;

            foreach (var pin in EnumPins(filter))
            {
                if (pin.QueryDirection(out var pinDir) == 0)
                {
                    pin.ConnectedTo(out var pPinConnectedTo);
                    if (pinDir == PinDirection.Input)
                    {
                        ++nIn;
                        if (pPinConnectedTo != null)
                        {
                            ++nInC;
                        }
                    }
                    else if (pinDir == PinDirection.Output)
                    {
                        ++nOut;
                        if (pPinConnectedTo != null)
                        {
                            ++nOutC;
                        }
                    }
                }
            }

            return nIn + nOut;
        }

        private bool IsStreamEnd(IBaseFilter filter)
        {
            CountPins(filter, out _, out int nOut, out _, out _);
            return nOut == 0;
        }

        private IBaseFilter GetFilterFromPin(IPin pPin)
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

        private IBaseFilter GetUpstreamFilter(IBaseFilter baseFilter, IPin inputPin = null)
        {
            return GetFilterFromPin(GetUpstreamPin(baseFilter, inputPin));
        }

        private IPin GetUpstreamPin(IBaseFilter baseFilter, IPin inputPin)
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

        private IReadOnlyList<Guid> ExtractMediaTypes(IPin pin)
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

        private IEnumerable<IPin> EnumPins(IBaseFilter baseFilter)
        {
            if (baseFilter != null && baseFilter.EnumPins(out var enumPins) >= 0)
            {
                // DirectShowLib messed up the interface on this one, but we'll try
                // and make do.
                var pins = new IPin[1];
                while (enumPins.Next(1, pins, IntPtr.Zero) == 0)
                {
                    yield return pins[0];
                    pins = new IPin[1];
                }
            }
        }

        private IEnumerable<AMMediaType> EnumMediaTypes(IPin pin)
        {
            if (pin != null && pin.EnumMediaTypes(out IEnumMediaTypes enumMediaTypes).IsSuccess())
            {
                var mediaTypes = new AMMediaType[1];
                while (enumMediaTypes.Next(1, mediaTypes, IntPtr.Zero).IsSuccess(true))
                {
                    yield return mediaTypes[0];
                    mediaTypes = new AMMediaType[1];
                }
            }
        }

        private IEnumerable<IBaseFilter> EnumFilters(IFilterGraph filterGraph)
        {
            if (filterGraph != null && filterGraph.EnumFilters(out var enumFilters).IsSuccess())
            {
                var filters = new IBaseFilter[1];
                while (enumFilters.Next(1, filters, IntPtr.Zero).IsSuccess(true))
                {
                    yield return filters[0];
                    filters = new IBaseFilter[1];
                }
            }
        }

        private IEnumerable<IBaseFilter> EnumCachedFilters(IGraphConfig graphConfig)
        {
            if (graphConfig != null && graphConfig.EnumCacheFilter(out var enumFilters).IsSuccess())
            {
                var filters = new IBaseFilter[1];
                while (enumFilters.Next(1, filters, IntPtr.Zero).IsSuccess(true))
                {
                    yield return filters[0];
                    filters = new IBaseFilter[1];
                }
            }
        }
    }
}
