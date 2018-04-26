using System;
using System.Runtime.InteropServices.ComTypes;
using DirectShowLib;

namespace FoundaryMediaPlayer.Engine.Windows
{
    /// <summary>
    /// Shockwave graph.
    /// </summary>
    public sealed class ShockwaveGraph : GraphBase
    {
        /// <inheritdoc />
        public override int AddFilter(IBaseFilter pFilter, string pName)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int RemoveFilter(IBaseFilter pFilter)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int EnumFilters(out IEnumFilters ppEnum)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int FindFilterByName(string pName, out IBaseFilter ppFilter)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int ConnectDirect(IPin ppinOut, IPin ppinIn, AMMediaType pmt)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int Reconnect(IPin ppin)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int Disconnect(IPin ppin)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int SetDefaultSyncSource()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int Connect(IPin ppinOut, IPin ppinIn)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int Render(IPin ppinOut)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int RenderFile(string lpcwstrFile, string lpcwstrPlayList)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int SetLogFile(IntPtr hFile)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int Abort()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int ShouldOperationContinue()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int AddSourceFilterForMoniker(IMoniker pMoniker, IBindCtx pCtx, string lpcwstrFilterName, out IBaseFilter ppFilter)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int ReconnectEx(IPin ppin, AMMediaType pmt)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int RenderEx(IPin pPinOut, AMRenderExFlags dwFlags, IntPtr pvContext)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int IsPinDirection(IPin pPin, PinDirection dir)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int IsPinConnected(IPin pPin)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int ConnectFilter(IBaseFilter pBF, IPin pPinIn)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int ConnectFilter(out IPin pPinOut, IBaseFilter pBF)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int ConnectFilterDirect(out IPin pPinOut, IBaseFilter pBF, AMMediaType pmt)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int NukeDownstream(object pUnk)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int AddToROT()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int RemoveFromROT()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int Run()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int Pause()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int Stop()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int GetState(int msTimeout, out FilterState pfs)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int RenderFile(string strFilename)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int AddSourceFilter(string strFilename, out object ppUnk)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int get_FilterCollection(out object ppUnk)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int get_RegFilterCollection(out object ppUnk)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int GetEventHandle(out IntPtr hEvent)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int WaitForCompletion(int msTimeout, out EventCode pEvCode)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int CancelDefaultHandling(EventCode lEvCode)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int RestoreDefaultHandling(EventCode lEvCode)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int SetNotifyFlags(NotifyFlags lNoNotifyFlags)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int GetNotifyFlags(out NotifyFlags lplNoNotifyFlags)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int GetDuration(out long pDuration)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int GetStopPosition(out long pStop)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int GetCurrentPosition(out long pCurrent)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int ConvertTimeFormat(out long pTarget, DsGuid pTargetFormat, long Source, DsGuid pSourceFormat)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int SetPositions(DsLong pCurrent, AMSeekingSeekingFlags dwCurrentFlags, DsLong pStop, AMSeekingSeekingFlags dwStopFlags)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int GetPositions(out long pCurrent, out long pStop)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int GetAvailable(out long pEarliest, out long pLatest)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int SetRate(double dRate)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int GetRate(out double pdRate)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int GetPreroll(out long pllPreroll)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int put_Caption(string caption)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int get_Caption(out string caption)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int put_WindowStyle(WindowStyle windowStyle)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int get_WindowStyle(out WindowStyle windowStyle)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int put_WindowStyleEx(WindowStyleEx windowStyleEx)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int get_WindowStyleEx(out WindowStyleEx windowStyleEx)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int put_AutoShow(OABool autoShow)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int get_AutoShow(out OABool autoShow)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int put_WindowState(WindowState windowState)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int get_WindowState(out WindowState windowState)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int put_BackgroundPalette(OABool backgroundPalette)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int get_BackgroundPalette(out OABool backgroundPalette)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int put_Visible(OABool visible)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int get_Visible(out OABool visible)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int put_Left(int left)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int get_Left(out int left)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int put_Width(int width)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int get_Width(out int width)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int put_Top(int top)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int get_Top(out int top)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int put_Height(int height)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int get_Height(out int height)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int put_Owner(IntPtr owner)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int get_Owner(out IntPtr owner)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int put_MessageDrain(IntPtr drain)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int get_MessageDrain(out IntPtr drain)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int get_BorderColor(out int color)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int put_BorderColor(int color)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int get_FullScreenMode(out OABool fullScreenMode)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int put_FullScreenMode(OABool fullScreenMode)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int SetWindowForeground(OABool focus)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int NotifyOwnerMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int SetWindowPosition(int left, int top, int width, int height)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int GetWindowPosition(out int left, out int top, out int width, out int height)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int GetMinIdealImageSize(out int width, out int height)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int GetMaxIdealImageSize(out int width, out int height)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int GetRestorePosition(out int left, out int top, out int width, out int height)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int HideCursor(OABool hideCursor)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int IsCursorHidden(out OABool hideCursor)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int get_AvgTimePerFrame(out double pAvgTimePerFrame)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int get_BitRate(out int pBitRate)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int get_BitErrorRate(out int pBitRate)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int get_VideoWidth(out int pVideoWidth)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int get_VideoHeight(out int pVideoHeight)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int put_SourceLeft(int SourceLeft)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int get_SourceLeft(out int pSourceLeft)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int put_SourceWidth(int SourceWidth)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int get_SourceWidth(out int pSourceWidth)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int put_SourceTop(int SourceTop)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int get_SourceTop(out int pSourceTop)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int put_SourceHeight(int SourceHeight)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int get_SourceHeight(out int pSourceHeight)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int put_DestinationLeft(int DestinationLeft)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int get_DestinationLeft(out int pDestinationLeft)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int put_DestinationWidth(int DestinationWidth)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int get_DestinationWidth(out int pDestinationWidth)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int put_DestinationTop(int DestinationTop)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int get_DestinationTop(out int pDestinationTop)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int put_DestinationHeight(int DestinationHeight)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int get_DestinationHeight(out int pDestinationHeight)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int SetSourcePosition(int left, int top, int width, int height)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int GetSourcePosition(out int left, out int top, out int width, out int height)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int SetDefaultSourcePosition()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int SetDestinationPosition(int left, int top, int width, int height)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int GetDestinationPosition(out int left, out int top, out int width, out int height)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int SetDefaultDestinationPosition()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int GetVideoSize(out int pWidth, out int pHeight)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int GetVideoPaletteEntries(int StartIndex, int Entries, out int pRetrieved, out int[] pPalette)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int GetCurrentImage(ref int pBufferSize, IntPtr pDIBImage)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int IsUsingDefaultSource()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int IsUsingDefaultDestination()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int put_Volume(int lVolume)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int get_Volume(out int plVolume)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int put_Balance(int lBalance)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int get_Balance(out int plBalance)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int QueryProgress(out long pllTotal, out long pllCurrent)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int AbortOperation()
        {
            throw new NotImplementedException();
        }
    }
}
