using System.Runtime.InteropServices;
using FoundaryMediaPlayer.Interop.Windows;

namespace FoundaryMediaPlayer.Engine.Classes
{
    [ComImport, Guid("E1A8B82A-32CE-4B0D-BE0D-AA68C772E423")]
    public class MadVR
    {
    }

    [ComImport, Guid(IID.CLSID_LAVSplitterSource)]
    internal class LAVSplitterSource
    {
    }

    [ComImport, Guid(IID.CLSID_LAVVideo)]
    internal class LAVVideo
    {
    }

    [ComImport, Guid(IID.CLSID_LAVAudio)]
    internal class LAVAudio
    {
    }

    [ComImport, Guid(IID.CLSID_XYVSFilter)]
    internal class XYVSFilter
    {
    }

    [ComImport, Guid(IID.CLSID_XYSubFilter)]
    internal class XySubFilter
    {
    }
}
