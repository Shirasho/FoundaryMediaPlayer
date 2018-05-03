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

using HResult = PInvoke.HResult.Code;

namespace FoundaryMediaPlayer.Engine.Graphs
{
    /// <summary>
    /// The native graph for the operating system. For Windows this is DirectShow.
    /// </summary>
    /// <remarks>
    /// The native graph cannot inherit from <see cref="ANonNativeGraphBase"/> since
    /// the concrete implementation is defined in COM. This class is simply a wrapper
    /// for that native class, and we forward most requests to the native implementation.
    /// </remarks>
    [Guid("09056CF8-B199-4E2E-9FE7-8EFCCA65E3EB")]
    [ClassInterface(ClassInterfaceType.None)]
    [SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
    public sealed class NativeGraph : IGraphBuilder2, IGraphBuilderDeadEnd, IDisposable
    {
        private static ILog _Logger { get; } = LogManager.GetLogger(typeof(NativeGraph));

        private IFilterGraph2 _FilterGraph { get; }
        private IFilterMapper2 _FilterMapper { get; }

        private List<AFilterBase> _SourceFilters { get; } = new List<AFilterBase>();
        private List<AFilterBase> _TransformFilters { get; } = new List<AFilterBase>();
        private List<AFilterBase> _OverrideFilters { get; } = new List<AFilterBase>();

        private FStreamPathCollection _StreamPaths { get; set; }
        private List<FStreamDeadEndCollection> _StreamDeadEnds { get; } = new List<FStreamDeadEndCollection>();

        private FIUnknownCollection _Unknowns { get; } = new FIUnknownCollection();
        private string _GraphId { get; } = string.Format(CultureInfo.InvariantCulture, "{0:B}", Marshal.GenerateGuidForType(typeof(NativeGraph)));
        private ulong _Register { get; set; }

        public NativeGraph()
        {
            _FilterGraph = WindowsInterop.CoCreateInstance<IFilterGraph2>(IID.FilterGraph);
            _FilterMapper = WindowsInterop.CoCreateInstance<IFilterMapper2>(IID.FilterMapper2);

            _FilterGraph.Should().NotBeNull();
            _FilterMapper.Should().NotBeNull();
        }

        ~NativeGraph()
        {
            ReleaseUnmanagedResources();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        private void ReleaseUnmanagedResources()
        {
            lock (this)
            {
                _SourceFilters.Clear();
                _TransformFilters.Clear();
                _OverrideFilters.Clear();
                _Unknowns.Clear();
                WindowsInterop.CoRelease(_FilterGraph);
            }
        }

        #region Interfaces

        public int AddFilter(IBaseFilter pFilter, string pName)
        {
            lock (this)
            {
                ComResult result;
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
            lock (this)
            {
                return _FilterGraph.RemoveFilter(pFilter);
            }
        }

        public int EnumFilters(out IEnumFilters ppEnum)
        {
            //TODO: Evaluate lock
            lock (this)
            {
                return _FilterGraph.EnumFilters(out ppEnum);
            }
        }


        public int FindFilterByName(string pName, out IBaseFilter ppFilter)
        {
            lock (this)
            {
                return _FilterGraph.FindFilterByName(pName, out ppFilter);
            }
        }


        public int ConnectDirect(IPin ppinOut, IPin ppinIn, AMMediaType pmt)
        {
            lock (this)
            {
                var baseFilter = GetFilterFromPin(ppinIn);
                var clsid = WindowsInterop.GetCLSID(baseFilter);

                var baseFilterUpstream = GetFilterFromPin(ppinOut);
                do
                {
                    if (baseFilterUpstream == baseFilter)
                    {
                        return new ComResult(HResult.E_FAIL);
                    }

                    if (clsid != IID.Proxy && WindowsInterop.GetCLSID(baseFilterUpstream) == clsid)
                    {
                        return new ComResult(HResult.E_FAIL);
                    }

                    baseFilterUpstream = GetUpstreamFilter(baseFilterUpstream);
                } while (baseFilterUpstream != null);

                return _FilterGraph.ConnectDirect(ppinOut, ppinIn, pmt);
            }
        }

        public int Reconnect(IPin ppin)
        {
            lock (this)
            {
                return _FilterGraph.Reconnect(ppin);
            }
        }

        public int Disconnect(IPin ppin)
        {
            lock (this)
            {
                return _FilterGraph.Disconnect(ppin);
            }
        }

        public int SetDefaultSyncSource()
        {
            lock (this)
            {
                return _FilterGraph.SetDefaultSyncSource();
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
                    return new ComResult(HResult.E_POINTER);
                }

                ComResult result;
                if (ComResult.FAILED(result = IsPinDirection(ppinOut, PinDirection.Output)) ||
                    (ppinIn != null && ComResult.FAILED(result = IsPinDirection(ppinIn, PinDirection.Input))))
                {
                    return result;
                }

                if (ComResult.FAILED(result = IsPinConnected(ppinOut)) ||
                    (ppinIn != null && ComResult.SUCCESS(IsPinConnected(ppinIn), true)))
                {
                    return unchecked((int)HResult.E_FAIL);
                }

                bool bDeadEnd = true;

                // 1. Try a direct connection between the filters with no intermediate filters.
                {
                    if (ppinIn != null)
                    {
                        if (ComResult.SUCCESS(result = ConnectDirect(ppinOut, ppinIn, null)))
                        {
                            return result;
                        }
                    }
                    else
                    {
                        if (ppinOut is IStreamBuilder streamBuilder)
                        {
                            if (ComResult.SUCCESS(result = streamBuilder.Render(ppinOut, this)))
                            {
                                return result;
                            }

                            streamBuilder.Backout(ppinOut, this);
                        }
                    }
                }

                // 2. Try cached filters.
                {
                    //TODO: _FilterGraph should be `this`, but this class is never an IGraphConfig.
                    if (_FilterGraph is IGraphConfig graphConfig)
                    {
                        foreach (var filter in EnumCachedFilters(graphConfig))
                        {
                            if (ppinIn != null && GetFilterFromPin(ppinIn) == filter)
                            {
                                continue;
                            }

                            graphConfig.RemoveFilterFromCache(filter);

                            if (ComResult.SUCCESS(result = ConnectFilterDirect(out ppinOut, filter, null)))
                            {
                                if (!IsStreamEnd(filter))
                                {
                                    bDeadEnd = false;
                                }

                                if (ComResult.SUCCESS(result = ConnectFilter(filter, ppinIn)))
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

                        // FFDShow
                        if (WindowsInterop.GetCLSID(ppinOut) == IID.FFDShowVideoDecoder &&
                            WindowsInterop.GetCLSID(filter) == IID.AudioRecord)
                        {
                            continue;
                        }

                        filters.Add(filter);
                    }

                    foreach (var filter in filters)
                    {
                        if (ComResult.SUCCESS(result = ConnectFilterDirect(out ppinOut, filter, null)))
                        {
                            if (!IsStreamEnd(filter))
                            {
                                bDeadEnd = false;
                            }

                            if (ComResult.SUCCESS(result = ConnectFilter(filter, ppinIn)))
                            {
                                return result;
                            }
                        }

                        Disconnect(ppinOut).Should().Be((int)HResult.S_OK);
                    }
                }

                // 4. Look up filters in the registry.
                {
                    var filterList = new FFilterContainerCollection();
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
                        ComResult.SUCCESS(result = _FilterMapper.EnumMatchingFilters(
                            out IEnumMoniker enumMoniker, 0, false, Merit.DoNotUse, true,
                            mediaTypes.Count / 2, mediaTypes.ToArray(), null, null, false,
                            ppinIn != null, 0, null, null, null)))
                    {
                        var monikers = new IMoniker[] { null };
                        while (ComResult.SUCCESS(result = enumMoniker.Next(1, monikers, IntPtr.Zero)))
                        {
                            var filterRegistry = new FFilterRegistry(monikers[0]);
                            filterList.Add(filterRegistry, 0, filterRegistry.CheckTypes(mediaTypes, true));
                        }
                    }

                    // Let's check whether the madVR allocator presenter is in our list.
                    // It should be if madVR is selected as the video renderer.
                    AFilterBase madVRAllocatorPresenter = null;
                    foreach (var filterContainer in filterList)
                    {
                        if (filterContainer.Filter.GUID == IID.MadVRAllocatorPresenter)
                        {
                            madVRAllocatorPresenter = filterContainer.Filter;
                            break;
                        }
                    }

                    const string madVRRendererName = "madVR Renderer";
                    foreach (var filterContainer in filterList)
                    {
                        // Prevent duplicate madVR entries in the graph.
                        FindFilterByName(madVRRendererName, out IBaseFilter madVRFilter);
                        if (madVRFilter != null && filterContainer.Filter.Name == madVRRendererName)
                        {
                            continue;
                        }

                        if (madVRAllocatorPresenter != null && filterContainer.Filter.GUID == IID.MadVR)
                        {
                            // Add a "temporary reference" for ease of use later.
                            // We will actually overwrite the original pointer in a bit.
                            filterContainer.Filter = madVRAllocatorPresenter;
                        }

                        _Logger.Debug($"{nameof(NativeGraph)} --> Connecting filter {filterContainer.Filter.Name}.");

                        if (ComResult.FAILED(result = filterContainer.Filter.Create(out IBaseFilter baseFilter, out IList<object> unknowns)))
                        {
                            _Logger.Warn($"{nameof(NativeGraph)} --> Unable to create filter {filterContainer.Filter.Name}.");
                        }

                        if (ComResult.FAILED(result = AddFilter(baseFilter, filterContainer.Filter.Name)))
                        {
                            _Logger.Warn($"{nameof(NativeGraph)} --> Unable to create filter {filterContainer.Filter.Name}.");
                            continue;
                        }

                        if (ComResult.SUCCESS(result = ConnectFilterDirect(out ppinOut, baseFilter, null)))
                        {
                            if (!IsStreamEnd(baseFilter))
                            {
                                bDeadEnd = false;
                            }

                            if (bContinueRender)
                            {
                                result = ConnectFilter(baseFilter, ppinIn);
                            }

                            if (ComResult.SUCCESS(result))
                            {
                                _Unknowns.AddRange(unknowns);
                                var mixerPinConfig = unknowns.FirstOrDefault(unk => unk is IMixerPinConfig && WindowsInterop.GetCLSID(unk) == IID.IMixerPinConfig, null);
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

                                //TODO: Finish implementation.
                                //if (baseFilter is IMFGetService mfgs && CLSID.GetCLSID(mfgs) == CLSID.GetCLSIDOf<IMFGetService>())
                                {
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

                                    if (madVRAllocatorPresenter != null)
                                    {
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
                                    }
                                }

                                return result;
                            }
                        }
                    }
                }

                if (bDeadEnd)
                {
                    var streamDeadEnd = new FStreamDeadEndCollection();
                    streamDeadEnd.AddRange(_StreamPaths);

                    int skip = 0;
                    foreach (var mediaType in EnumMediaTypes(ppinOut))
                    {
                        if (mediaType.majorType == IID.MEDIATYPE_Stream && 
                            mediaType.subType == Guid.Empty)
                        {
                            ++skip;
                        }

                        streamDeadEnd.MediaTypes.Add(mediaType);
                    }

                    if (skip < streamDeadEnd.MediaTypes.Count) {
                        _StreamDeadEnds.AddRange(streamDeadEnd);
                    }
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
                _StreamPaths.Clear();
                _StreamDeadEnds.Clear();

                var deadEnds = new FStreamDeadEndCollection();

                ComResult result;
                // ReSharper disable once InconsistentNaming
                ComResult resultRFS = HResult.S_OK;

                if (ComResult.FAILED(result = EnumSourceFilters(lpcwstrFile, out FFilterContainerCollection filterList)))
                {
                    return result;
                }

                foreach (var filterContainer in filterList)
                {
                    if (ComResult.SUCCESS(result = AddSourceFilter(filterContainer.Filter, lpcwstrFile, filterContainer.Filter.Name, out IBaseFilter baseFilter)))
                    {
                        _StreamPaths.Clear();
                        _StreamDeadEnds.Clear();

                        if (ComResult.FAILED(result = ConnectFilter(baseFilter, null)))
                        {
                            return result;
                        }

                        NukeDownstream(baseFilter);
                        RemoveFilter(baseFilter);

                        deadEnds.AddRange(_StreamDeadEnds);
                    }
                    else if (filterContainer.Filter.GUID == WindowsInterop.GetCLSID<FRARFileSource>() && ((result >> 16) & 0x1fff) == 4)
                    {
                        resultRFS = result;
                    }
                }

                _StreamDeadEnds.AddRange(deadEnds);

                return resultRFS != HResult.S_OK ? resultRFS : result;
            }
        }

        public int AddSourceFilter(string lpcwstrFileName, string lpcwstrFilterName, out IBaseFilter ppFilter)
        {
            lock (this)
            {
                ppFilter = null;

                ComResult result;
                if (ComResult.FAILED(result = EnumSourceFilters(lpcwstrFileName, out FFilterContainerCollection filterList)))
                {
                    return result;
                }

                foreach (var filterContainer in filterList)
                {
                    if (ComResult.SUCCESS(result = AddSourceFilter(filterContainer.Filter, lpcwstrFileName, lpcwstrFilterName, out ppFilter)))
                    {
                        return result;
                    }
                }
                
                return new ComResult(HResult.E_FAIL);
            }
        }

        public int SetLogFile(IntPtr hFile)
        {
            lock (this)
            {
                return _FilterGraph.SetLogFile(hFile);
            }
        }

        public int Abort()
        {
            lock (this)
            {
                return _FilterGraph.Abort();
            }
        }

        public int ShouldOperationContinue()
        {
            lock (this)
            {
                return _FilterGraph.ShouldOperationContinue();
            }
        }

        public int AddSourceFilterForMoniker(IMoniker pMoniker, IBindCtx pCtx, string lpcwstrFilterName, out IBaseFilter ppFilter)
        {
            lock (this)
            {
                return _FilterGraph.AddSourceFilterForMoniker(pMoniker, pCtx, lpcwstrFilterName, out ppFilter);
            }
        }

        public int ReconnectEx(IPin ppin, AMMediaType pmt)
        {
            lock (this)
            {
                return _FilterGraph.ReconnectEx(ppin, pmt);
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
                int result;
                if (pPin == null)
                {
                    return new ComResult(HResult.E_POINTER);
                }

                if (ComResult.FAILED(result = pPin.QueryDirection(out PinDirection pinDir)))
                {
                    return result;
                }

                return new ComResult(dir == pinDir ? HResult.S_OK : HResult.S_FALSE);
            }
        }

        public int IsPinConnected(IPin pPin)
        {
            lock (this)
            {
                if (pPin == null)
                {
                    return new ComResult(HResult.E_POINTER);
                }

                bool bSuccess = ComResult.SUCCESS(pPin.ConnectedTo(out IPin pinTo));

                return new ComResult(bSuccess && pinTo != null ? HResult.S_OK : HResult.S_FALSE);
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
                ppv = IntPtr.Zero;

                foreach (var unk in _Unknowns)
                {
                    IntPtr unkPtr = Marshal.GetIUnknownForObject(unk);
                    if (ComResult.SUCCESS(Marshal.QueryInterface(unkPtr, ref iid, out ppv)))
                    {
                        if (bRemove)
                        {
                            _Unknowns.Remove(unk);
                        }

                        Marshal.Release(unkPtr);
                        return new ComResult(HResult.S_OK);
                    }

                    Marshal.Release(unkPtr);
                }

                return new ComResult(HResult.E_NOINTERFACE);
            }
        }

        public int AddToROT()
        {
            lock (this)
            {
                if (_Register != 0)
                {
                    return new ComResult(HResult.S_FALSE);
                }

                ComResult result;

                if (ComResult.SUCCESS(result = WindowsInterop.GetRunningObjectTable(0, out IRunningObjectTable rot)) &&
                    ComResult.SUCCESS(result = WindowsInterop.CreateItemMoniker("!", _GraphId, out IMoniker moniker)))
                {
                    _Register = (ulong)rot.Register((int)ERunningObjectTableFlags.RegistrationKeepAlive, this, moniker);
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
                    return new ComResult(HResult.S_FALSE);
                }

                ComResult result;

                if (ComResult.SUCCESS(result = WindowsInterop.GetRunningObjectTable(0, out IRunningObjectTable rot)))
                {
                    rot.Revoke((int)_Register);
                    _Register = 0;
                }

                return result;
            }
        }

        public uint GetCount()
        {
            lock (this)
            {
                return (uint)_StreamDeadEnds.Count;
            }
        }

        /// <inheritdoc />
        public int GetDeadEnd(int iIndex, IList<string> path, IList<AMMediaType> mts)
        {
            lock (this)
            {
                if (!_StreamDeadEnds.IsValidIndex(iIndex))
                {
                    return new ComResult(HResult.E_FAIL);
                }

                path.Clear();
                mts.Clear();

                var deadEnds = _StreamDeadEnds[iIndex];
                foreach (var deadEnd in deadEnds)
                {
                    path.Add($"{deadEnd.Filter}::{deadEnd.Pin}");
                }

                mts.AddRange(_StreamDeadEnds[iIndex].MediaTypes);

                return new ComResult(HResult.S_OK);
            }
        }

        #endregion

        private AFilterBase LookupFilterRegistry(Guid guid, IReadOnlyList<AFilterBase> filters, Merit fallbackMerit = Merit.DoNotUse)
        {
            foreach (var filter in filters)
            {
                if (filter.GUID == guid)
                {
                    return new FFilterRegistry(guid, merit: filter.Merit);
                }
            }

            return new FFilterRegistry(guid, merit: fallbackMerit);
        }

        private int EnumSourceFilters(string fileName, out FFilterContainerCollection filterList)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                filterList = null;
                return new ComResult(HResult.E_POINTER);
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
                    return new ComResult(HResult.E_FAIL);
                }
            }

            filterList = new FFilterContainerCollection();

            if (file == null || !file.Exists)
            {
                // Internet protocol.
                foreach (var filter in _SourceFilters)
                {
                    if (filter.Protocols.Contains(protocol))
                    {
                        filterList.Add(filter, 0, false);
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
                            filterList.Add(filter, 1, false);
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
                        filterList.Add(filter, 2, false);
                    }
                }
            }

            // The rest.
            foreach (var filter in _SourceFilters)
            {
                if (filter.Protocols.Count == 0 && filter.CheckBytes.Count == 0 && filter.Extensions.Count == 0)
                {
                    filterList.Add(filter, 3, false);
                }
            }

            if (file == null || !file.Exists)
            {
                using (var reg = new RegistryKeyReference($"{protocol}\\Extensions", RegistryHive.ClassesRoot))
                {
                    if (Guid.TryParse(reg.GetString(ext), out Guid guid))
                    {
                        filterList.Add(LookupFilterRegistry(guid, _OverrideFilters), 4, false);
                    }
                }

                using (var reg = new RegistryKeyReference($"{protocol}", RegistryHive.ClassesRoot))
                {
                    if (Guid.TryParse(reg.GetString("Source Filter"), out Guid guid))
                    {
                        filterList.Add(LookupFilterRegistry(guid, _OverrideFilters), 5, false);
                    }
                }
            }
            else
            {
                using (var key = new RegistryKeyReference("Media Type", RegistryHive.ClassesRoot))
                {
                    foreach (var enumKey in key.IterateSubKeys())
                    {
                        if (!Guid.TryParse(enumKey.Name, out Guid majorType))
                        {
                            continue;
                        }

                        using (var subkeyLevel1 = new RegistryKeyReference(key, enumKey.Name))
                        {
                            foreach (var enumSubKey in subkeyLevel1.IterateSubKeys())
                            {
                                if (!Guid.TryParse(enumSubKey.Name, out Guid subType))
                                {
                                    continue;
                                }

                                Guid.TryParse(enumSubKey.GetString("Source Filter"), out Guid clsid);
                                foreach (var regValueEntry in enumSubKey.GetValues())
                                {
                                    var regValue = regValueEntry.Value.ToString();
                                    if (CheckBytes(file, regValue))
                                    {
                                        var filter = LookupFilterRegistry(clsid, _OverrideFilters);
                                        filter.AddType(majorType, subType);
                                        filterList.Add(filter, 0, false);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // Try and get the filters by the extension.
            if (!string.IsNullOrEmpty(ext))
            {
                using (var extKey = new RegistryKeyReference($"Media Type\\Extensions\\{ext}", RegistryHive.ClassesRoot))
                {
                    var sourceFilterGuidRaw = extKey.GetString("Source Filter");
                    if (!string.IsNullOrWhiteSpace(sourceFilterGuidRaw) &&
                        Guid.TryParse(sourceFilterGuidRaw, out Guid sourceFilterGuid) &&
                        sourceFilterGuid != Guid.Empty)
                    {
                        Guid.TryParse(extKey.GetString("MediaType"), out Guid majorType);
                        Guid.TryParse(extKey.GetString("SubType"), out Guid subType);

                        var filter = LookupFilterRegistry(sourceFilterGuid, _OverrideFilters);
                        filter.AddType(majorType, subType);
                        filterList.Add(filter, 7, false);
                    }
                }
            }

            var arFilter = LookupFilterRegistry(IID.AsyncReader, _OverrideFilters);
            arFilter.AddType(IID.MEDIATYPE_Stream, Guid.Empty);
            filterList.Add(arFilter, 9, false);

            return new ComResult(HResult.S_OK);
        }

        private int AddSourceFilter(AFilterBase filter, string lpcwstrFileName, string lpcwstrFilterName, out IBaseFilter baseFilter)
        {
            ComResult result;
            baseFilter = null;

            if (ComResult.FAILED(result = filter.Create(out IBaseFilter createFilter, out IList<object> unks)))
            {
                return result;
            }

            if (!(createFilter is IFileSourceFilter fileSourceFilter))
            {
                return new ComResult(HResult.E_NOINTERFACE);
            }

            if (ComResult.FAILED(result = AddFilter(createFilter, lpcwstrFilterName)))
            {
                return result;
            }

            AMMediaType amMediaType = null;
            var types = filter.Types;

            if (filter.Types.Count == 2 &&
                types.First() != Guid.Empty &&
                types.Last() != Guid.Empty)
            {
                amMediaType = new FMediaType(types.First(), types.Last());
            }

            if (ComResult.FAILED(result = fileSourceFilter.Load(lpcwstrFileName, amMediaType)))
            {
                RemoveFilter(createFilter);
                return result;
            }

            foreach (var mediaType in EnumMediaTypes(GetFirstPin(createFilter, PinDirection.Output)))
            {
                //    static const GUID guid1 =
                //    { 0x640999A0, 0xA946, 0x11D0, { 0xA5, 0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 } };
                //    static const GUID guid2 =
                //    { 0x640999A1, 0xA946, 0x11D0, { 0xA5, 0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 } };
                //    static const GUID guid3 =
                //    { 0xD51BD5AE, 0x7548, 0x11CF, { 0xA5, 0x20, 0x00, 0x80, 0xC7, 0x7E, 0xF5, 0x8A } };
                if (mediaType.subType == guid1 ||
                    mediaType.subType == guid2 ||
                    mediaType.subType == guid3)
                {
                    RemoveFilter(createFilter);
                    return AddSourceFilter(new FFilterRegistry(IID.NetShowSource), lpcwstrFileName, lpcwstrFilterName, out baseFilter);
                }
            }

            baseFilter = createFilter;
            _Unknowns.AddRange(unks);

            return new ComResult(HResult.S_OK);
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
                var pins = new IPin[] { null };
                while (enumPins.Next(1, pins, IntPtr.Zero) == 0)
                {
                    yield return pins[0];
                    pins[0] = null;
                }
            }
        }

        private IPin GetFirstPin(IBaseFilter baseFilter, PinDirection direction)
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

        private IEnumerable<AMMediaType> EnumMediaTypes(IPin pin)
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

        private IEnumerable<IBaseFilter> EnumFilters(IFilterGraph filterGraph)
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

        private IEnumerable<IBaseFilter> EnumCachedFilters(IGraphConfig graphConfig)
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
    }
}
