#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices.ComTypes;
using DirectShowLib;
using FoundaryMediaPlayer.Platforms.Windows;

using User32 = PInvoke.User32;
using HResult = PInvoke.HResult.Code;

namespace FoundaryMediaPlayer.Engine.Windows
{
    /// <summary>
    /// Base graph.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public abstract class GraphBase
        : IGraphBuilder2
        , IMediaControl
        , IMediaEventEx
        , IMediaSeeking
        , IVideoWindow
        , IBasicVideo
        , IBasicAudio
        , IAMOpenProgress
        , IGraphEngine
    {
        private struct GMSG
        {
            public EventCode EventCode { get; set; }
            public IntPtr Param1 { get; set; }
            public IntPtr Param2 { get; set; }
        }
        
        private IntPtr _NotifyWndHandle { get; set; } = IntPtr.Zero;
        private User32.WindowMessage _NotifyMsg { get; set; }
        private IntPtr _NotifyInstData { get; set; } = IntPtr.Zero;
        private Queue<GMSG> _MessageQueue { get; } = new Queue<GMSG>();

        public abstract int AddFilter(IBaseFilter pFilter, string pName);
        public abstract int RemoveFilter(IBaseFilter pFilter);
        public abstract int EnumFilters(out IEnumFilters ppEnum);
        public abstract int FindFilterByName(string pName, out IBaseFilter ppFilter);
        public abstract int ConnectDirect(IPin ppinOut, IPin ppinIn, AMMediaType pmt);
        public abstract int Reconnect(IPin ppin);
        public abstract int Disconnect(IPin ppin);
        public abstract int SetDefaultSyncSource();
        public abstract int Connect(IPin ppinOut, IPin ppinIn);
        public abstract int Render(IPin ppinOut);
        public abstract int RenderFile(string lpcwstrFile, string lpcwstrPlayList);
        
        public virtual int AddSourceFilter(string lpcwstrFileName, string lpcwstrFilterName, out IBaseFilter ppFilter)
        {
            ppFilter = null;
            return RenderFile(lpcwstrFileName, null);
        }

        public abstract int SetLogFile(IntPtr hFile);
        public abstract int Abort();
        public abstract int ShouldOperationContinue();
        public abstract int AddSourceFilterForMoniker(IMoniker pMoniker, IBindCtx pCtx, string lpcwstrFilterName, out IBaseFilter ppFilter);
        /// <inheritdoc />
        public abstract int ReconnectEx(IPin ppin, AMMediaType pmt);
        /// <inheritdoc />
        public abstract int RenderEx(IPin pPinOut, AMRenderExFlags dwFlags, IntPtr pvContext);
        /// <inheritdoc />
        public abstract int IsPinDirection(IPin pPin, PinDirection dir);
        /// <inheritdoc />
        public abstract int IsPinConnected(IPin pPin);
        /// <inheritdoc />
        public abstract int ConnectFilter(IBaseFilter pBF, IPin pPinIn);
        /// <inheritdoc />
        public abstract int ConnectFilter(out IPin pPinOut, IBaseFilter pBF);
        /// <inheritdoc />
        public abstract int ConnectFilterDirect(out IPin pPinOut, IBaseFilter pBF, AMMediaType pmt);
        /// <inheritdoc />
        public abstract int NukeDownstream(object pUnk);

        /// <inheritdoc />
        [Obsolete("Use direct cast instead.", true)]
        public virtual int FindInterface(ref Guid iid, out IntPtr ppv, bool bRemove)
        {
            ppv = IntPtr.Zero;
            return 0;
            //return QueryInterface(iid, ppv);
        }

        /// <inheritdoc />
        public abstract int AddToROT();
        /// <inheritdoc />
        public abstract int RemoveFromROT();
        /// <inheritdoc />
        public abstract int Run();
        /// <inheritdoc />
        public abstract int Pause();
        /// <inheritdoc />
        public abstract int Stop();
        /// <inheritdoc />
        public abstract int GetState(int msTimeout, out FilterState pfs);
        /// <inheritdoc />
        public abstract int RenderFile(string strFilename);
        /// <inheritdoc />
        public abstract int AddSourceFilter(string strFilename, out object ppUnk);
        /// <inheritdoc />
        public abstract int get_FilterCollection(out object ppUnk);
        /// <inheritdoc />
        public abstract int get_RegFilterCollection(out object ppUnk);

        /// <inheritdoc />
        public virtual int StopWhenReady()
        {
            return Stop();
        }

        public abstract int GetEventHandle(out IntPtr hEvent);

        public int GetEvent(out EventCode lEventCode, out IntPtr lParam1, out IntPtr lParam2, int msTimeout)
        {
            if (_MessageQueue.Count == 0)
            {
                lEventCode = EventCode.Complete;
                lParam1 = IntPtr.Zero;
                lParam2 = IntPtr.Zero;
                return unchecked((int)HResult.E_FAIL);
            }

            var message = _MessageQueue.Dequeue();
            lEventCode = message.EventCode;
            lParam1 = message.Param1;
            lParam2 = message.Param2;

            return (int)HResult.S_OK;
        }

        public abstract int WaitForCompletion(int msTimeout, out EventCode pEvCode);

        public abstract int CancelDefaultHandling(EventCode lEvCode);

        public abstract int RestoreDefaultHandling(EventCode lEvCode);

        public int FreeEventParams(EventCode lEvCode, IntPtr lParam1, IntPtr lParam2)
        {
            //if (lEvCode == EventCode.ErrorAbort) //EC_BG_ERROR
            //{
            //    if (lParam1 != IntPtr.Zero)
            //    {
            //        CoTaskMemFree(lParam1);
            //    }
            //}

            return (int)HResult.S_OK;
        }

        /// <inheritdoc />
        public int SetNotifyWindow(IntPtr hwnd, int lMsg, IntPtr lInstanceData)
        {
            _NotifyWndHandle = hwnd;
            _NotifyMsg = (User32.WindowMessage)lMsg;
            _NotifyInstData = lInstanceData;

            if (!User32.IsWindow(_NotifyWndHandle))
            {
                _NotifyWndHandle = IntPtr.Zero;
                return unchecked((int)HResult.E_FAIL);
            }

            return (int)HResult.S_OK;
        }

        /// <inheritdoc />
        public abstract int SetNotifyFlags(NotifyFlags lNoNotifyFlags);
        /// <inheritdoc />
        public abstract int GetNotifyFlags(out NotifyFlags lplNoNotifyFlags);

        /// <inheritdoc />
        public virtual int GetCapabilities(out AMSeekingSeekingCapabilities pCapabilities)
        {
            pCapabilities = AMSeekingSeekingCapabilities.CanSeekAbsolute | AMSeekingSeekingCapabilities.CanGetCurrentPos | AMSeekingSeekingCapabilities.CanGetDuration;
            return (int)HResult.S_OK;
        }

        /// <inheritdoc />
        public int CheckCapabilities(ref AMSeekingSeekingCapabilities pCapabilities)
        {
            if (pCapabilities == AMSeekingSeekingCapabilities.None)
            {
                return (int)HResult.S_OK;
            }

            GetCapabilities(out AMSeekingSeekingCapabilities capabilities);
            var capabilities2 = capabilities & pCapabilities;
            return capabilities2 == AMSeekingSeekingCapabilities.None ? unchecked((int)HResult.E_FAIL) :
                (capabilities2 == pCapabilities ? (int)HResult.S_OK : (int)HResult.S_FALSE);
        }

        /// <inheritdoc />
        public int IsFormatSupported(Guid pFormat)
        {
            return pFormat == CLSID.TIME_FORMAT_MEDIA_TIME ? (int)HResult.S_OK : (int)HResult.S_FALSE;
        }

        /// <inheritdoc />
        public int QueryPreferredFormat(out Guid pFormat)
        {
            return GetTimeFormat(out pFormat);
        }

        /// <inheritdoc />
        public int GetTimeFormat(out Guid pFormat)
        {
            pFormat = CLSID.TIME_FORMAT_MEDIA_TIME;
            return (int)HResult.S_OK;
        }

        /// <inheritdoc />
        public int IsUsingTimeFormat(Guid pFormat)
        {
            return IsFormatSupported(pFormat);
        }

        /// <inheritdoc />
        public int SetTimeFormat(Guid pFormat)
        {
            return IsFormatSupported(pFormat) == (int)HResult.S_OK ? (int)HResult.S_OK : unchecked((int)HResult.E_INVALIDARG);
        }

        /// <inheritdoc />
        public abstract int GetDuration(out long pDuration);
        /// <inheritdoc />
        public abstract int GetStopPosition(out long pStop);
        /// <inheritdoc />
        public abstract int GetCurrentPosition(out long pCurrent);
        /// <inheritdoc />
        public abstract int ConvertTimeFormat(out long pTarget, DsGuid pTargetFormat, long Source, DsGuid pSourceFormat);
        /// <inheritdoc />
        public abstract int SetPositions(DsLong pCurrent, AMSeekingSeekingFlags dwCurrentFlags, DsLong pStop, AMSeekingSeekingFlags dwStopFlags);
        /// <inheritdoc />
        public abstract int GetPositions(out long pCurrent, out long pStop);
        /// <inheritdoc />
        public abstract int GetAvailable(out long pEarliest, out long pLatest);
        /// <inheritdoc />
        public abstract int SetRate(double dRate);
        /// <inheritdoc />
        public abstract int GetRate(out double pdRate);

        /// <inheritdoc />
        public abstract int GetPreroll(out long pllPreroll);
        /// <inheritdoc />
        public abstract int put_Caption(string caption);
        /// <inheritdoc />
        public abstract int get_Caption(out string caption);
        /// <inheritdoc />
        public abstract int put_WindowStyle(WindowStyle windowStyle);
        /// <inheritdoc />
        public abstract int get_WindowStyle(out WindowStyle windowStyle);

        /// <inheritdoc />
        public abstract int put_WindowStyleEx(WindowStyleEx windowStyleEx);

        /// <inheritdoc />
        public abstract int get_WindowStyleEx(out WindowStyleEx windowStyleEx);

        /// <inheritdoc />
        public abstract int put_AutoShow(OABool autoShow);

        /// <inheritdoc />
        public abstract int get_AutoShow(out OABool autoShow);

        /// <inheritdoc />
        public abstract int put_WindowState(WindowState windowState);

        /// <inheritdoc />
        public abstract int get_WindowState(out WindowState windowState);

        /// <inheritdoc />
        public abstract int put_BackgroundPalette(OABool backgroundPalette);

        /// <inheritdoc />
        public abstract int get_BackgroundPalette(out OABool backgroundPalette);

        /// <inheritdoc />
        public abstract int put_Visible(OABool visible);

        /// <inheritdoc />
        public abstract int get_Visible(out OABool visible);

        /// <inheritdoc />
        public abstract int put_Left(int left);

        /// <inheritdoc />
        public abstract int get_Left(out int left);

        /// <inheritdoc />
        public abstract int put_Width(int width);

        /// <inheritdoc />
        public abstract int get_Width(out int width);

        /// <inheritdoc />
        public abstract int put_Top(int top);

        /// <inheritdoc />
        public abstract int get_Top(out int top);

        /// <inheritdoc />
        public abstract int put_Height(int height);

        /// <inheritdoc />
        public abstract int get_Height(out int height);

        /// <inheritdoc />
        public abstract int put_Owner(IntPtr owner);

        /// <inheritdoc />
        public abstract int get_Owner(out IntPtr owner);

        /// <inheritdoc />
        public abstract int put_MessageDrain(IntPtr drain);

        /// <inheritdoc />
        public abstract int get_MessageDrain(out IntPtr drain);

        /// <inheritdoc />
        public abstract int get_BorderColor(out int color);

        /// <inheritdoc />
        public abstract int put_BorderColor(int color);

        /// <inheritdoc />
        public abstract int get_FullScreenMode(out OABool fullScreenMode);

        /// <inheritdoc />
        public abstract int put_FullScreenMode(OABool fullScreenMode);

        /// <inheritdoc />
        public abstract int SetWindowForeground(OABool focus);

        /// <inheritdoc />
        public abstract int NotifyOwnerMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam);

        /// <inheritdoc />
        public abstract int SetWindowPosition(int left, int top, int width, int height);

        /// <inheritdoc />
        public abstract int GetWindowPosition(out int left, out int top, out int width, out int height);

        /// <inheritdoc />
        public abstract int GetMinIdealImageSize(out int width, out int height);

        /// <inheritdoc />
        public abstract int GetMaxIdealImageSize(out int width, out int height);

        /// <inheritdoc />
        public abstract int GetRestorePosition(out int left, out int top, out int width, out int height);

        /// <inheritdoc />
        public abstract int HideCursor(OABool hideCursor);

        /// <inheritdoc />
        public abstract int IsCursorHidden(out OABool hideCursor);

        /// <inheritdoc />
        public abstract int get_AvgTimePerFrame(out double pAvgTimePerFrame);

        /// <inheritdoc />
        public abstract int get_BitRate(out int pBitRate);

        /// <inheritdoc />
        public abstract int get_BitErrorRate(out int pBitRate);

        /// <inheritdoc />
        public abstract int get_VideoWidth(out int pVideoWidth);

        /// <inheritdoc />
        public abstract int get_VideoHeight(out int pVideoHeight);

        /// <inheritdoc />
        public abstract int put_SourceLeft(int SourceLeft);

        /// <inheritdoc />
        public abstract int get_SourceLeft(out int pSourceLeft);

        /// <inheritdoc />
        public abstract int put_SourceWidth(int SourceWidth);

        /// <inheritdoc />
        public abstract int get_SourceWidth(out int pSourceWidth);

        /// <inheritdoc />
        public abstract int put_SourceTop(int SourceTop);

        /// <inheritdoc />
        public abstract int get_SourceTop(out int pSourceTop);

        /// <inheritdoc />
        public abstract int put_SourceHeight(int SourceHeight);

        /// <inheritdoc />
        public abstract int get_SourceHeight(out int pSourceHeight);

        /// <inheritdoc />
        public abstract int put_DestinationLeft(int DestinationLeft);

        /// <inheritdoc />
        public abstract int get_DestinationLeft(out int pDestinationLeft);

        /// <inheritdoc />
        public abstract int put_DestinationWidth(int DestinationWidth);

        /// <inheritdoc />
        public abstract int get_DestinationWidth(out int pDestinationWidth);

        /// <inheritdoc />
        public abstract int put_DestinationTop(int DestinationTop);

        /// <inheritdoc />
        public abstract int get_DestinationTop(out int pDestinationTop);

        /// <inheritdoc />
        public abstract int put_DestinationHeight(int DestinationHeight);

        /// <inheritdoc />
        public abstract int get_DestinationHeight(out int pDestinationHeight);

        /// <inheritdoc />
        public abstract int SetSourcePosition(int left, int top, int width, int height);

        /// <inheritdoc />
        public abstract int GetSourcePosition(out int left, out int top, out int width, out int height);

        /// <inheritdoc />
        public abstract int SetDefaultSourcePosition();

        /// <inheritdoc />
        public abstract int SetDestinationPosition(int left, int top, int width, int height);

        /// <inheritdoc />
        public abstract int GetDestinationPosition(out int left, out int top, out int width, out int height);

        /// <inheritdoc />
        public abstract int SetDefaultDestinationPosition();

        /// <inheritdoc />
        public abstract int GetVideoSize(out int pWidth, out int pHeight);

        /// <inheritdoc />
        public abstract int GetVideoPaletteEntries(int StartIndex, int Entries, out int pRetrieved, out int[] pPalette);

        /// <inheritdoc />
        public abstract int GetCurrentImage(ref int pBufferSize, IntPtr pDIBImage);

        /// <inheritdoc />
        public abstract int IsUsingDefaultSource();

        /// <inheritdoc />
        public abstract int IsUsingDefaultDestination();

        /// <inheritdoc />
        public abstract int put_Volume(int lVolume);

        /// <inheritdoc />
        public abstract int get_Volume(out int plVolume);

        /// <inheritdoc />
        public abstract int put_Balance(int lBalance);

        /// <inheritdoc />
        public abstract int get_Balance(out int plBalance);

        /// <inheritdoc />
        public abstract int QueryProgress(out long pllTotal, out long pllCurrent);

        /// <inheritdoc />
        public abstract int AbortOperation();

        /// <inheritdoc />
        public abstract EEngineType GetEngine();

        public void NotifyEvent(EventCode lEventCode, IntPtr? lParam1 = null, IntPtr? lParam2 = null)
        {
            if (_NotifyWndHandle == IntPtr.Zero)
            {
                return;
            }

            var message = new GMSG
            {
                EventCode = lEventCode,
                Param1 = lParam1 ?? IntPtr.Zero,
                Param2 = lParam2 ?? IntPtr.Zero
            };
            _MessageQueue.Enqueue(message);

            User32.PostMessage(_NotifyWndHandle, _NotifyMsg, IntPtr.Zero, _NotifyInstData);
        }

        protected void ClearMessageQueue()
        {
            while (_MessageQueue.Count > 0)
            {
                var message = _MessageQueue.Dequeue();
                FreeEventParams(message.EventCode, message.Param1, message.Param2);
            }
        }
    }
}
