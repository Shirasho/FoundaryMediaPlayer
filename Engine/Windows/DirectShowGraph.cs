#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using DirectShowLib;
using FoundaryMediaPlayer.Platforms.Windows;

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
    public class DirectShowGraph : IGraphBuilder2, IGraphBuilderDeadEnd
    {
        private IFilterGraph _FilterGraph { get; }
        private IFilterGraph2 _FilterGraph2 => _FilterGraph as IFilterGraph2;
        private IFilterMapper2 _FilterMapper { get; }

        private IList<FGFilter> _SourceFilters { get; } = new List<FGFilter>();
        private IList<FGFilter> _TransformFilters { get; } = new List<FGFilter>();
        private IList<FGFilter> _OverrideFilters { get; } = new List<FGFilter>();

        private IDictionary<Type, Guid> _Unknowns { get; } = new Dictionary<Type, Guid>();

        /// <summary>
        /// 
        /// </summary>
        public DirectShowGraph()
        {
            _FilterGraph = Native.CoCreateInstance<IFilterGraph>(CLSID.FilterGraph);
            _FilterMapper = Native.CoCreateInstance<IFilterMapper2>(CLSID.FilterMapper2);
        }


        ~DirectShowGraph()
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
                return unchecked((int)HResult.E_UNEXPECTED);
            }

            lock (this)
            {
                var result = _FilterGraph2.AddFilter(pFilter, pName);
                if (result < 0)
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
                return unchecked((int)HResult.E_UNEXPECTED);
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
                return unchecked((int)HResult.E_UNEXPECTED);
            }
            return _FilterGraph2.EnumFilters(out ppEnum);
        }


        public int FindFilterByName(string pName, out IBaseFilter ppFilter)
        {
            if (_FilterGraph2 == null)
            {
                ppFilter = null;
                return unchecked((int)HResult.E_UNEXPECTED);
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
                var clsid = baseFilter.GetType().GetTypeInfo().GUID;

                var baseFilterUpstream = GetFilterFromPin(ppinOut);
                do
                {
                    if (baseFilterUpstream == baseFilter)
                    {
                        return unchecked((int)HResult.E_FAIL);
                    }

                    if (clsid != CLSID.Proxy && baseFilterUpstream.GetType().GetTypeInfo().GUID == clsid)
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
                return unchecked((int)HResult.E_UNEXPECTED);
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
                return unchecked((int)HResult.E_UNEXPECTED);
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
                return unchecked((int)HResult.E_UNEXPECTED);
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
                    return unchecked((int)HResult.E_POINTER);
                }

                if (IsPinDirection(ppinOut, PinDirection.Output) >= 0 ||
                    (ppinIn != null && IsPinDirection(ppinIn, PinDirection.Input) >= 0))
                {
                    return unchecked((int)HResult.E_FAIL);
                }

                if (IsPinConnected(ppinOut) < 0 ||
                    (ppinIn != null && IsPinConnected(ppinIn) == 0))
                {
                    return unchecked((int)HResult.E_FAIL);
                }

                bool bDeadEnd = true;
                if (ppinIn != null)
                {
                    var result = ConnectDirect(ppinOut, ppinIn, null);
                    if (result == 0)
                    {
                        return result;
                    }
                }
                else
                {
                    if (ppinOut is IStreamBuilder streamBuilder)
                    {
                        var result = streamBuilder.Render(ppinOut, this);
                        if (result == 0)
                        {
                            return result;
                        }

                        streamBuilder.Backout(ppinOut, this);
                    }
                }

                if (this is IGraphConfig graphConfig)
                {
                    BeginEnumCachedFilters(pGC, pEF, pBF) {
                        if (pPinIn && GetFilterFromPin(pPinIn) == pBF)
                        {
                            continue;
                        }

                        hr = pGC->RemoveFilterFromCache(pBF);

                        // does RemoveFilterFromCache call AddFilter like AddFilterToCache calls RemoveFilter ?

                        if (SUCCEEDED(hr = ConnectFilterDirect(pPinOut, pBF, nullptr)))
                        {
                            if (!IsStreamEnd(pBF))
                            {
                                fDeadEnd = false;
                            }

                            if (SUCCEEDED(hr = ConnectFilter(pBF, pPinIn)))
                            {
                                return hr;
                            }
                        }

                        hr = pGC->AddFilterToCache(pBF);
                    }
                    EndEnumCachedFilters;
                }

                CInterfaceList<IBaseFilter> pBFs;

                BeginEnumFilters(this, pEF, pBF) {
                    if (pPinIn && GetFilterFromPin(pPinIn) == pBF
                        || GetFilterFromPin(pPinOut) == pBF)
                    {
                        continue;
                    }

                    // HACK: ffdshow - audio capture filter
                    if (GetCLSID(pPinOut) == GUIDFromCString(_T("{04FE9017-F873-410E-871E-AB91661A4EF7}"))
                        && GetCLSID(pBF) == GUIDFromCString(_T("{E30629D2-27E5-11CE-875D-00608CB78066}")))
                    {
                        continue;
                    }

                    pBFs.AddTail(pBF);
                }
                EndEnumFilters;

                POSITION pos = pBFs.GetHeadPosition();
                while (pos)
                {
                    IBaseFilter* pBF = pBFs.GetNext(pos);

                    if (SUCCEEDED(hr = ConnectFilterDirect(pPinOut, pBF, nullptr)))
                    {
                        if (!IsStreamEnd(pBF))
                        {
                            fDeadEnd = false;
                        }

                        if (SUCCEEDED(hr = ConnectFilter(pBF, pPinIn)))
                        {
                            return hr;
                        }
                    }

                    EXECUTE_ASSERT(SUCCEEDED(Disconnect(pPinOut)));
                }

                CFGFilterList fl;

                CAtlArray<GUID> types;
                ExtractMediaTypes(pPinOut, types);

                POSITION pos = m_transform.GetHeadPosition();
                while (pos)
                {
                    CFGFilter* pFGF = m_transform.GetNext(pos);
                    if (pFGF->GetMerit() < MERIT64_DO_USE || pFGF->CheckTypes(types, false))
                    {
                        fl.Insert(pFGF, 0, pFGF->CheckTypes(types, true), false);
                    }
                }

                pos = m_override.GetHeadPosition();
                while (pos)
                {
                    CFGFilter* pFGF = m_override.GetNext(pos);
                    if (pFGF->GetMerit() < MERIT64_DO_USE || pFGF->CheckTypes(types, false))
                    {
                        fl.Insert(pFGF, 0, pFGF->CheckTypes(types, true), false);
                    }
                }

                CComPtr<IEnumMoniker> pEM;
                if (!types.IsEmpty()
                    && SUCCEEDED(m_pFM->EnumMatchingFilters(
                        &pEM, 0, FALSE, MERIT_DO_NOT_USE + 1,
                        TRUE, (DWORD)types.GetCount() / 2, types.GetData(), nullptr, nullptr, FALSE,
                        !!pPinIn, 0, nullptr, nullptr, nullptr)))
                {
                    for (CComPtr<IMoniker> pMoniker; S_OK == pEM->Next(1, &pMoniker, nullptr); pMoniker = nullptr)
                    {
                        CFGFilterRegistry* pFGF = DEBUG_NEW CFGFilterRegistry(pMoniker);
                        fl.Insert(pFGF, 0, pFGF->CheckTypes(types, true));
                    }
                }

                // let's check whether the madVR allocator presenter is in our list
                // it should be if madVR is selected as the video renderer
                CFGFilter* pMadVRAllocatorPresenter = nullptr;
                pos = fl.GetHeadPosition();
                while (pos)
                {
                    CFGFilter* pFGF = fl.GetNext(pos);
                    if (pFGF->GetCLSID() == CLSID_madVRAllocatorPresenter)
                    {
                        // found it!
                        pMadVRAllocatorPresenter = pFGF;
                        break;
                    }
                }

                pos = fl.GetHeadPosition();
                while (pos)
                {
                    CFGFilter* pFGF = fl.GetNext(pos);

                    // Checks if madVR is already in the graph to avoid two instances at the same time
                    CComPtr<IBaseFilter> pBFmadVR;
                    FindFilterByName(_T("madVR Renderer"), &pBFmadVR);
                    if (pBFmadVR && (pFGF->GetName() == _T("madVR Renderer")))
                    {
                        continue;
                    }

                    if (pMadVRAllocatorPresenter && (pFGF->GetCLSID() == CLSID_madVR))
                    {
                        // the pure madVR filter was selected (without the allocator presenter)
                        // subtitles, OSD etc don't work correctly without the allocator presenter
                        // so we prefer the allocator presenter over the pure filter
                        pFGF = pMadVRAllocatorPresenter;
                    }

                    TRACE(_T("FGM: Connecting '%s'\n"), pFGF->GetName());

                    CComPtr<IBaseFilter> pBF;
                    CInterfaceList < IUnknown, &IID_IUnknown > pUnks;
                    if (FAILED(pFGF->Create(&pBF, pUnks)))
                    {
                        TRACE(_T("     --> Filter creation failed\n"));
                        continue;
                    }

                    if (FAILED(hr = AddFilter(pBF, pFGF->GetName())))
                    {
                        TRACE(_T("     --> Adding the filter failed\n"));
                        pUnks.RemoveAll();
                        pBF.Release();
                        continue;
                    }

                    hr = ConnectFilterDirect(pPinOut, pBF, nullptr);
                    /*
                    if (FAILED(hr))
                    {
                        if (types.GetCount() >= 2 && types[0] == MEDIATYPE_Stream && types[1] != GUID_NULL)
                        {
                            CMediaType mt;

                            mt.majortype = types[0];
                            mt.subtype = types[1];
                            mt.formattype = FORMAT_None;
                            if (FAILED(hr)) hr = ConnectFilterDirect(pPinOut, pBF, &mt);

                            mt.formattype = GUID_NULL;
                            if (FAILED(hr)) hr = ConnectFilterDirect(pPinOut, pBF, &mt);
                        }
                    }
                    */
                    if (SUCCEEDED(hr))
                    {
                        if (!IsStreamEnd(pBF))
                        {
                            fDeadEnd = false;
                        }

                        if (bContinueRender)
                        {
                            hr = ConnectFilter(pBF, pPinIn);
                        }

                        if (SUCCEEDED(hr))
                        {
                            m_pUnks.AddTailList(&pUnks);

                            // maybe the application should do this...

                            POSITION posInterface = pUnks.GetHeadPosition();
                            while (posInterface)
                            {
                                if (CComQIPtr < IMixerPinConfig, &IID_IMixerPinConfig > pMPC = pUnks.GetNext(posInterface)) {
                                    pMPC->SetAspectRatioMode(AM_ARMODE_STRETCHED);
                                }
                            }

                            if (CComQIPtr < IVMRAspectRatioControl > pARC = pBF)
                            {
                                pARC->SetAspectRatioMode(VMR_ARMODE_NONE);
                            }

                            if (CComQIPtr < IVMRAspectRatioControl9 > pARC = pBF)
                            {
                                pARC->SetAspectRatioMode(VMR_ARMODE_NONE);
                            }

                            if (CComQIPtr < IVMRMixerControl9 > pMC = pBF)
                            {
                                m_pUnks.AddTail(pMC);
                            }

                            if (CComQIPtr < IVMRMixerBitmap9 > pMB = pBF)
                            {
                                m_pUnks.AddTail(pMB);
                            }

                            if (CComQIPtr < IMFGetService, &__uuidof(IMFGetService) > pMFGS = pBF) {
                                CComPtr<IMFVideoDisplayControl> pMFVDC;
                                CComPtr<IMFVideoMixerBitmap> pMFMB;
                                CComPtr<IMFVideoProcessor> pMFVP;

                                if (SUCCEEDED(pMFGS->GetService(MR_VIDEO_RENDER_SERVICE, IID_PPV_ARGS(&pMFVDC))))
                                {
                                    m_pUnks.AddTail(pMFVDC);
                                }

                                if (SUCCEEDED(pMFGS->GetService(MR_VIDEO_MIXER_SERVICE, IID_PPV_ARGS(&pMFMB))))
                                {
                                    m_pUnks.AddTail(pMFMB);
                                }

                                if (SUCCEEDED(pMFGS->GetService(MR_VIDEO_MIXER_SERVICE, IID_PPV_ARGS(&pMFVP))))
                                {
                                    m_pUnks.AddTail(pMFVP);
                                }

                                //CComPtr<IMFWorkQueueServices> pMFWQS;
                                //pMFGS->GetService (MF_WORKQUEUE_SERVICES, IID_PPV_ARGS(&pMFWQS));
                                //pMFWQS->BeginRegisterPlatformWorkQueueWithMMCSS(

                                if (pMadVRAllocatorPresenter)
                                {
                                    // Hook DXVA to have status and logging.
                                    CComPtr<IDirectXVideoDecoderService> pDecoderService;
                                    CComPtr<IDirect3DDeviceManager9> pDeviceManager;
                                    HANDLE hDevice = INVALID_HANDLE_VALUE;

                                    if (SUCCEEDED(pMFGS->GetService(MR_VIDEO_ACCELERATION_SERVICE, IID_PPV_ARGS(&pDeviceManager)))
                                        && SUCCEEDED(pDeviceManager->OpenDeviceHandle(&hDevice))
                                        && SUCCEEDED(pDeviceManager->GetVideoService(hDevice, IID_PPV_ARGS(&pDecoderService))))
                                    {
                                        HookDirectXVideoDecoderService(pDecoderService);
                                        pDeviceManager->CloseDeviceHandle(hDevice);
                                    }

                                    pDeviceManager.Release();
                                    pDecoderService.Release();
                                }
                            }

                            return hr;
                        }
                    }

                    EXECUTE_ASSERT(SUCCEEDED(RemoveFilter(pBF)));
                    TRACE(_T("     --> Failed to connect\n"));
                    pUnks.RemoveAll();
                    pBF.Release();
                }
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
            if (baseFilter != null && baseFilter.EnumPins(out IEnumPins enumPins) >= 0)
            {
                // DirectShowLib REALLY fucked up the interface on this one. Totally wrong.
                for (IPin pin = null; enumPins.Next(1, new[] { pin }, IntPtr.Zero) == 0; pin = null)
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
            }

            return null;
        }
    }
}
