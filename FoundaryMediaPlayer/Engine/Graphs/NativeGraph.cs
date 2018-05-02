using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using DirectShowLib;
using FluentAssertions;
using Foundary;
using Foundary.Extensions;
using FoundaryMediaPlayer.Interfaces;
using FoundaryMediaPlayer.Interop.Windows;
using log4net;
using Microsoft.Win32;
using PInvoke;

namespace FoundaryMediaPlayer.Engine.Graphs
{
    /// <summary>
    /// The native graph for the operating system. For Windows this is DirectShow.
    /// </summary>
    [Guid("09056CF8-B199-4E2E-9FE7-8EFCCA65E3EB")]
    [ClassInterface(ClassInterfaceType.None)]
    public sealed class NativeGraph : IGraphBuilder2, IGraphBuilderDeadEnd
    {
        private static ILog _Logger { get; } = LogManager.GetLogger(typeof(NativeGraph));

        private IFilterGraph2 _FilterGraph {get;}
        private IFilterMapper2 _FilterMapper {get;}

        private List<AFilterBase> _SourceFilters { get; } = new List<AFilterBase>();
        private List<AFilterBase> _TransformFilters { get; } = new List<AFilterBase>();
        private List<AFilterBase> _OverrideFilters { get; } = new List<AFilterBase>();

        private List<StreamDeadEnd> StreamDeadEnds { get; } = new List<StreamDeadEnd>();

        private FIUnknownCollection _Unknowns { get; } = new FIUnknownCollection();
        private string _GraphId { get; } = string.Format(CultureInfo.InvariantCulture, "{0:B}", Marshal.GenerateGuidForType(typeof(NativeGraph)));

        public NativeGraph()
        {
            _FilterGraph = WindowsInterop.CoCreateInstance<IFilterGraph2>(IID.FilterGraph);
            _FilterMapper = WindowsInterop.CoCreateInstance<IFilterMapper2>(IID.FilterMapper2);

            _FilterGraph.Should().NotBeNull();
            _FilterMapper.Should().NotBeNull();
        }

        public int AddFilter(IBaseFilter pFilter, string pName)
        {
            lock (this)
            {
                int result;
                if (ComResult.FAILED(result = _FilterGraph.AddFilter(pFilter, pName)))
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

        [SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
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
                    return unchecked((int)HResult.E_FAIL);
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

                            graphConfig.AddFilterToCache(filter);
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

                        Disconnect(ppinOut).Should().Be((int)HResult.S_OK);
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
                            filterList.Add(filter, 0, filter.CheckTypes(mediaTypes, true));
                        }
                    }

                    foreach (var filter in _OverrideFilters)
                    {
                        if (filter.Merit < Merit.DoNotUse || filter.CheckTypes(mediaTypes, false))
                        {
                            filterList.Add(filter, 0, filter.CheckTypes(mediaTypes, true));
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
                            var filterRegistry = new FGFilterRegistry(monikers[0]);
                            filterList.Add(filterRegistry, 0, filterRegistry.CheckTypes(mediaTypes, true));
                        }
                    }

                    // Let's check whether the madVR allocator presenter is in our list.
                    // It should be if madVR is selected as the video renderer.
                    FGFilterBase madVRAllocatorPresenter = null;
                    foreach (var filter in filterList)
                    {
                        if (filter.GUID == CLSID.MadVRAllocatorPresenter)
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

                        if (madVRAllocatorPresenter != null && filter.GUID == CLSID.MadVR)
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

                        if (!AddFilter(baseFilter, filter.Name).IsSuccess())
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
                                ((IMixerPinConfig)mixerPinConfig)?.SetAspectRatioMode(AspectRatioMode.Stretched);

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

                                //if (baseFilter is IMFGetService mfgs && CLSID.GetCLSID(mfgs) == CLSID.GetCLSIDOf<IMFGetService>())
                                //{
                                //    CComPtr<IMFVideoDisplayControl> pMFVDC;
                                //    CComPtr<IMFVideoMixerBitmap> pMFMB;
                                //    CComPtr<IMFVideoProcessor> pMFVP;

                                //    if (mfgs.GetService(MR_VIDEO_RENDER_SERVICE, IID_PPV_ARGS(&pMFVDC)).IsSuccess())
                                //    {
                                //        _Unknowns.Add(pMFVDC);
                                //    }

                                //    if (mfgs.GetService(MR_VIDEO_MIXER_SERVICE, IID_PPV_ARGS(&pMFMB)).IsSuccess())
                                //    {
                                //        _Unknowns.Add(pMFMB);
                                //    }

                                //    if (mfgs.GetService(MR_VIDEO_MIXER_SERVICE, IID_PPV_ARGS(&pMFVP)).IsSuccess())
                                //    {
                                //        _Unknowns.Add(pMFVP);
                                //    }

                                //    if (madVRAllocatorPresenter != null)
                                //    {
                                //        // Hook DXVA to have status and logging.
                                //        CComPtr<IDirectXVideoDecoderService> pDecoderService;
                                //        CComPtr<IDirect3DDeviceManager9> pDeviceManager;
                                //        HANDLE hDevice = INVALID_HANDLE_VALUE;

                                //        if (mfgs->GetService(MR_VIDEO_ACCELERATION_SERVICE, IID_PPV_ARGS(&pDeviceManager)).IsSuccess()
                                //            && pDeviceManager->OpenDeviceHandle(&hDevice).IsSuccess()
                                //            && pDeviceManager->GetVideoService(hDevice, IID_PPV_ARGS(&pDecoderService)).IsSuccess())
                                //        {
                                //            HookDirectXVideoDecoderService(pDecoderService);
                                //            pDeviceManager->CloseDeviceHandle(hDevice);
                                //        }
                                //    }
                                //}

                                return result;
                            }
                        }
                    }

                    if (madVRIndex >= 0)
                    {
                        // The pure madVR filter was selected (without the allocator presenter)
                        // subtitles, OSD etc don't work correctly without the allocator presenter
                        // so we prefer the allocator presenter over the pure filter.
                        filterList.SetFilter(madVRIndex, madVRAllocatorPresenter);
                    }
                }

                if (bDeadEnd)
                {
                    //var streamDeadEnd = new StreamDeadEnd();
                    //streamDeadEnd.Add(m_streamPath);
                    //int skip = 0;
                    //foreach (var mediaType in EnumMediaTypes(ppinOut))
                    //{
                    //    if (mediaType.majorType == MEDIATYPE_Stream && mediaType.subType == MEDIASUBTYPE_NULL)
                    //    {
                    //        ++skip;
                    //    }

                    //    streamDeadEnd.MediaTypes.Add(mediaType);
                    //}

                    //if (skip < (int)streamDeadEnd.MediaTypes.Length) {
                    //    StreamDeadEnds.Add(streamDeadEnd);
                    //}
                }

                return ppinIn != null ? unchecked((int)0x80040217) : unchecked((int)0x80040218);
            }
        }


        public int Render(IPin ppinOut)
        {
            lock (this)
            {
                return RenderEx(ppinOut, AMRenderExFlags.None, IntPtr.Zero);
            }
        }


        public int RenderFile(string lpcwstrFile, string lpcwstrPlayList)
        {
            lock (this)
            {
                StreamPath.Clear();
                StreamDeadEnds.Clear();

                List<StreamDeadEnd> deadEnds = new List<StreamDeadEnd>();

                int result;
                int resultRFS = HResult.S_OK.AsInt();

                if (!(result = EnumSourceFilters(lpcwstrFile, out FGFilterList filterList)).IsSuccess())
                {
                    return result;
                }

                foreach (var filter in filterList)
                {
                    if ((result = AddSourceFilter(filter, lpcwstrFile, filter.Name, out IBaseFilter baseFilter)).IsSuccess())
                    {
                        StreamPath.Clear();
                        StreamDeadEnds.Clear();

                        if (!(result = ConnectFilter(baseFilter, null)).IsSuccess())
                        {
                            return result;
                        }

                        NukeDownstream(baseFilter);
                        RemoveFilter(baseFilter);

                        deadEnds.AddRange(StreamDeadEnds);
                    }
                    else if (filter.GUID == CLSID.GetCLSIDOf<RARFileSource>() && ((result >> 16) & 0x1fff) == 4)
                    {
                        resultRFS = result;
                    }
                }

                StreamDeadEnds.AddRange(deadEnds);

                return resultRFS != HResult.S_OK.AsInt() ? resultRFS : result;
            }
        }


        public int AddSourceFilter(string lpcwstrFileName, string lpcwstrFilterName, out IBaseFilter ppFilter)
        {
            lock (this)
            {
                int result;
                if (!(result = EnumSourceFilters(lpcwstrFileName, out FGFilterList filterList)).IsSuccess())
                {
                    ppFilter = null;
                    return result;
                }

                foreach (var filter in filterList)
                {
                    if ((result = AddSourceFilter(filter, lpcwstrFileName, lpcwstrFilterName, out ppFilter)).IsSuccess())
                    {
                        return result;
                    }
                }

                ppFilter = null;
                return HResult.E_FAIL.AsInt();
            }
        }

        private int AddSourceFilter(FGFilterBase filter, string lpcwstrFileName, string lpcwstrFilterName, out IBaseFilter baseFilter)
        {
            int result;
            if (!(result = filter.Create(out IBaseFilter createFilter, out IList<object> unks)).IsSuccess())
            {
                baseFilter = null;
                return result;
            }
            

            //CComQIPtr<IFileSourceFilter> pFSF = pBF;
            //if (!pFSF)
            //{
            //    return E_NOINTERFACE;
            //}

            //if (FAILED(hr = AddFilter(pBF, lpcwstrFilterName)))
            //{
            //    return hr;
            //}

            //const AM_MEDIA_TYPE* pmt = nullptr;

            //CMediaType mt;
            //const CAtlList<GUID>&types = pFGF->GetTypes();
            //if (types.GetCount() == 2 && (types.GetHead() != GUID_NULL || types.GetTail() != GUID_NULL))
            //{
            //    mt.majortype = types.GetHead();
            //    mt.subtype = types.GetTail();
            //    pmt = &mt;
            //}

            //// sometimes looping with AviSynth
            //if (FAILED(hr = pFSF->Load(lpcwstrFileName, pmt)))
            //{
            //    RemoveFilter(pBF);
            //    return hr;
            //}

            //// doh :P
            //BeginEnumMediaTypes(GetFirstPin(pBF, PINDIR_OUTPUT), pEMT, pmt2) {
            //    static const GUID guid1 =
            //    { 0x640999A0, 0xA946, 0x11D0, { 0xA5, 0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 } };
            //    static const GUID guid2 =
            //    { 0x640999A1, 0xA946, 0x11D0, { 0xA5, 0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 } };
            //    static const GUID guid3 =
            //    { 0xD51BD5AE, 0x7548, 0x11CF, { 0xA5, 0x20, 0x00, 0x80, 0xC7, 0x7E, 0xF5, 0x8A } };

            //    if (pmt2->subtype == guid1 || pmt2->subtype == guid2 || pmt2->subtype == guid3)
            //    {
            //        RemoveFilter(pBF);
            //        pFGF = DEBUG_NEW CFGFilterRegistry(CLSID_NetShowSource);
            //        hr = AddSourceFilter(pFGF, lpcwstrFileName, lpcwstrFilterName, ppBF);
            //        delete pFGF;
            //        return hr;
            //    }
            //}
            //EndEnumMediaTypes(pmt2);

            //*ppBF = pBF.Detach();

            //m_pUnks.AddTailList(&pUnks);

            return HResult.S_OK.AsInt();
        }


        public int SetLogFile(IntPtr hFile)
        {
            if (_FilterGraph2 == null)
            {
                return HResult.E_UNEXPECTED.AsInt();
            }

            lock (this)
            {
                return _FilterGraph2.SetLogFile(hFile);
            }
        }


        public int Abort()
        {
            if (_FilterGraph2 == null)
            {
                return HResult.E_UNEXPECTED.AsInt();
            }

            lock (this)
            {
                return _FilterGraph2.Abort();
            }
        }


        public int ShouldOperationContinue()
        {
            if (_FilterGraph2 == null)
            {
                return HResult.E_UNEXPECTED.AsInt();
            }

            lock (this)
            {
                return _FilterGraph2.ShouldOperationContinue();
            }
        }


        public int AddSourceFilterForMoniker(IMoniker pMoniker, IBindCtx pCtx, string lpcwstrFilterName, out IBaseFilter ppFilter)
        {
            if (_FilterGraph2 == null)
            {
                ppFilter = null;
                return HResult.E_UNEXPECTED.AsInt();
            }

            lock (this)
            {
                return _FilterGraph2.AddSourceFilterForMoniker(pMoniker, pCtx, lpcwstrFilterName, out ppFilter);
            }
        }


        public int ReconnectEx(IPin ppin, AMMediaType pmt)
        {
            if (_FilterGraph2 == null)
            {
                return HResult.E_UNEXPECTED.AsInt();
            }

            lock (this)
            {
                return _FilterGraph2.ReconnectEx(ppin, pmt);
            }
        }


        public int RenderEx(IPin pPinOut, AMRenderExFlags dwFlags, IntPtr pvContext)
        {
            throw new NotImplementedException();
        }


        public int IsPinDirection(IPin pPin, PinDirection dir)
        {
            lock (this)
            {
                if (pPin == null)
                {
                    return HResult.E_POINTER.AsInt();
                }

                if (!pPin.QueryDirection(out PinDirection pinDir).IsSuccess())
                {
                    return HResult.E_FAIL.AsInt();
                }

                return dir == pinDir ? HResult.S_OK.AsInt() : HResult.S_FALSE.AsInt();
            }
        }


        public int IsPinConnected(IPin pPin)
        {
            lock (this)
            {
                if (pPin == null)
                {
                    return HResult.E_POINTER.AsInt();
                }

                return pPin.ConnectedTo(out IPin pinTo).IsSuccess() && pinTo != null ? HResult.S_OK.AsInt() : HResult.S_FALSE.AsInt();
            }
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
            lock (this)
            {
                foreach (var unk in _Unknowns)
                {
                    IntPtr unkPtr = Marshal.GetIUnknownForObject(unk);
                    if (Marshal.QueryInterface(unkPtr, ref iid, out ppv).IsSuccess())
                    {
                        if (bRemove)
                        {
                            _Unknowns.Remove(unk);
                        }

                        Marshal.Release(unkPtr);
                        return HResult.S_OK.AsInt();
                    }

                    Marshal.Release(unkPtr);
                }

                ppv = IntPtr.Zero;
                return HResult.E_NOINTERFACE.AsInt();
            }
        }


        public int AddToROT()
        {
            lock (this)
            {
                if (_Register != 0)
                {
                    return HResult.S_FALSE.AsInt();
                }

                int result;

                if ((result = Native.GetRunningObjectTable(0, out IRunningObjectTable rot)).IsSuccess() &&
                    (result = Native.CreateItemMoniker("!", _GraphId, out IMoniker moniker)).IsSuccess())
                {
                    _Register = rot.Register((int)ERunningObjectTableFlags.RegistrationKeepAlive, this, moniker);
                }

                return result;
            }
        }

        public int RemoveFromROT()
        {
            lock (this)
            {
                if (_Register == 0)
                {
                    return HResult.S_FALSE.AsInt();
                }

                int result;

                if ((result = Native.GetRunningObjectTable(0, out IRunningObjectTable rot)).IsSuccess())
                {
                    rot.Revoke(_Register);
                    _Register = 0;
                }

                return result;
            }
        }


        public uint GetCount()
        {
            lock (this)
            {
                return (uint)StreamDeadEnds.Count;
            }
        }

        /// <inheritdoc />
        public int GetDeadEnd(int iIndex, IList<string> path, IList<FMediaType> mts)
        {
            throw new NotImplementedException();
        }


        public int GetDeadEnd(int iIndex, IList<string> path, IList<MediaType> mts)
        {
            lock (this)
            {
                if (!StreamDeadEnds.IsValidIndex(iIndex))
                {
                    return HResult.E_FAIL.AsInt();
                }

                path.Clear();
                mts.Clear();

                var deadEnd = StreamDeadEnds[iIndex];
                path.Add($"{deadEnd.Filter}::{deadEnd.Pin}");
                mts.Add(deadEnd.MediaType);

                return HResult.S_OK.AsInt();
            }
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

        private int EnumSourceFilters(string fileName, out FGFilterList filterList)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                filterList = null;
                return HResult.E_POINTER.AsInt();
            }

            var fn = fileName.TrimStart();
            var protocol = fn.Substring(0, fn.IndexOf(':') + 1).TrimEnd(':').ToLowerInvariant();
            var ext = Path.GetExtension(fileName).ToLowerInvariant();

            FileInfo file = null;
            if (protocol.Length <= 1 || protocol == "file")
            {
                file = new FileInfo(fn);

                // In case of audio CDs with extra content, the audio tracks
                // cannot be accessed directly so we have to try opening it.
                if (!file.Exists && ext != ".cda")
                {
                    filterList = null;
                    return HResult.E_FAIL.AsInt();
                }
            }

            filterList = new FGFilterList();

            if (file == null || !file.Exists)
            {
                // Internet protocol.
                foreach (var filter in _SourceFilters)
                {
                    if (filter.Protocols.Contains(protocol))
                    {
                        filterList.Add(filter, 0);
                    }
                }
            }
            else
            {
                // Internal.
                foreach (var filter in _SourceFilters)
                {
                    foreach (var bytes in filter.CheckBytes)
                    {
                        if (CheckBytes(file, bytes))
                        {
                            filterList.Add(filter, 1);
                            break;
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(ext))
            {
                // Internal / file extension

                foreach (var filter in _SourceFilters)
                {
                    if (filter.Extensions.Contains(ext))
                    {
                        filterList.Add(filter, 2);
                    }
                }
            }

            // The rest.
            foreach (var filter in _SourceFilters)
            {
                if (filter.Protocols.Count == 0 && filter.CheckBytes.Count == 0 && filter.Extensions.Count == 0)
                {
                    filterList.Add(filter, 3);
                }
            }

            if (file == null || !file.Exists)
            {
                using (var reg = new RegistryKeyReference($"{protocol}\\Extensions", RegistryHive.ClassesRoot))
                {
                    if (Guid.TryParse(reg.GetString(ext), out Guid guid))
                    {
                        filterList.Add(LookupFilterRegistry(guid, _OverrideFilters), 4);
                    }
                }

                using (var reg = new RegistryKeyReference($"{protocol}", RegistryHive.ClassesRoot))
                {
                    if (Guid.TryParse(reg.GetString("Source Filter"), out Guid guid))
                    {
                        filterList.Add(LookupFilterRegistry(guid, _OverrideFilters), 5);
                    }
                }
            }
            else
            {
                CRegKey key;
                if (ERROR_SUCCESS == key.Open(HKEY_CLASSES_ROOT, _T("Media Type"), KEY_READ))
                {
                    FILETIME ft;
                    len = _countof(buff);
                    for (DWORD i = 0; ERROR_SUCCESS == key.EnumKey(i, buff, &len, &ft); i++, len = _countof(buff))
                    {
                        GUID majortype;
                        if (FAILED(GUIDFromCString(buff, majortype)))
                        {
                            continue;
                        }

                        CRegKey majorkey;
                        if (ERROR_SUCCESS == majorkey.Open(key, buff, KEY_READ))
                        {
                            len = _countof(buff);
                            for (DWORD j = 0; ERROR_SUCCESS == majorkey.EnumKey(j, buff, &len, &ft); j++, len = _countof(buff))
                            {
                                GUID subtype;
                                if (FAILED(GUIDFromCString(buff, subtype)))
                                {
                                    continue;
                                }

                                CRegKey subkey;
                                if (ERROR_SUCCESS == subkey.Open(majorkey, buff, KEY_READ))
                                {
                                    len = _countof(buff);
                                    if (ERROR_SUCCESS != subkey.QueryStringValue(_T("Source Filter"), buff, &len))
                                    {
                                        continue;
                                    }

                                    GUID clsid = GUIDFromCString(buff);
                                    TCHAR buff2[256];
                                    ULONG len2;

                                    len = _countof(buff);
                                    len2 = sizeof(buff2);
                                    for (DWORD k = 0, type;
                                            clsid != GUID_NULL && ERROR_SUCCESS == RegEnumValue(subkey, k, buff2, &len2, 0, &type, (BYTE*)buff, &len);
                                            k++, len = _countof(buff), len2 = sizeof(buff2))
                                    {
                                        if (CheckBytes(hFile, CString(buff)))
                                        {
                                            CFGFilter* pFGF = LookupFilterRegistry(clsid, m_override);
                                            pFGF->AddType(majortype, subtype);
                                            fl.Insert(pFGF, 9);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(ext))
            {
                CRegKey key;
                if (ERROR_SUCCESS == key.Open(HKEY_CLASSES_ROOT, _T("Media Type\\Extensions\\") + CString(ext), KEY_READ))
                {
                    len = _countof(buff);
                    ZeroMemory(buff, sizeof(buff));
                    LONG ret = key.QueryStringValue(_T("Source Filter"), buff, &len); // QueryStringValue can return ERROR_INVALID_DATA on bogus strings (radlight mpc v1003, fixed in v1004)
                    if (ERROR_SUCCESS == ret || ERROR_INVALID_DATA == ret && GUIDFromCString(buff) != GUID_NULL)
                    {
                        GUID clsid = GUIDFromCString(buff);
                        GUID majortype = GUID_NULL;
                        GUID subtype = GUID_NULL;

                        len = _countof(buff);
                        if (ERROR_SUCCESS == key.QueryStringValue(_T("Media Type"), buff, &len))
                        {
                            majortype = GUIDFromCString(buff);
                        }

                        len = _countof(buff);
                        if (ERROR_SUCCESS == key.QueryStringValue(_T("Subtype"), buff, &len))
                        {
                            subtype = GUIDFromCString(buff);
                        }

                        CFGFilter* pFGF = LookupFilterRegistry(clsid, m_override);
                        pFGF->AddType(majortype, subtype);
                        fl.Insert(pFGF, 7);
                    }
                }
            }

            var arFilter = LookupFilterRegistry(CLSID.AsyncReader, _OverrideFilters);
            arFilter.AddType(CLSID.MediaType_Stream, Guid.Empty);
            filterList.Add(arFilter, 9);

            return HResult.S_OK.AsInt();
        }

        private bool CheckBytes(FileInfo file, string checkBytes)
        {
            throw new NotImplementedException();
        }

        private FGFilterBase LookupFilterRegistry(Guid guid, IReadOnlyList<FGFilterBase> filters, Merit fallbackMerit = Merit.DoNotUse)
        {
            foreach (var filter in filters)
            {
                if (filter.GUID == guid)
                {
                    return new FGFilterRegistry(guid, merit: filter.Merit);
                }
            }

            return new FGFilterRegistry(guid, merit: fallbackMerit);
        }
    }
}
