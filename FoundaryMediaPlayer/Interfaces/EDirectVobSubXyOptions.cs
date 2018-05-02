using System.Diagnostics.CodeAnalysis;

namespace FoundaryMediaPlayer.Interfaces
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum EDirectVobSubXyIntOptions
    {
        INT_COLOR_SPACE = 0,
        INT_YUV_RANGE,
        INT_OVERLAY_CACHE_MAX_ITEM_NUM,
        INT_SCAN_LINE_DATA_CACHE_MAX_ITEM_NUM,
        INT_PATH_DATA_CACHE_MAX_ITEM_NUM,
        INT_OVERLAY_NO_BLUR_CACHE_MAX_ITEM_NUM,

        INT_BITMAP_MRU_CACHE_ITEM_NUM,
        INT_CLIPPER_MRU_CACHE_ITEM_NUM,

        INT_TEXT_INFO_CACHE_ITEM_NUM,
        INT_ASS_TAG_LIST_CACHE_ITEM_NUM,

        INT_SUBPIXEL_VARIANCE_CACHE_ITEM_NUM,

        INT_SUBPIXEL_POS_LEVEL,

        INT_LAYOUT_SIZE_OPT,
        INT_COUNT
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum EDirectVobSubXyBoolOptions
    {
        BOOL_FOLLOW_UPSTREAM_PREFERRED_ORDER,
        BOOL_HIDE_TRAY_ICON,
        BOOL_COUNT
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum EDirectVobSubXySizeOptions
    {
        SIZE_ORIGINAL_VIDEO,
        SIZE_ASS_PLAY_RESOLUTION,
        SIZE_USER_SPECIFIED_LAYOUT_SIZE,
        SIZE_LAYOUT_WITH,
        SIZE_COUNT
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum EDirectVobSubXyStringOptions
    {
        STRING_LOAD_EXT_LIST,
        STRING_PGS_YUV_RANGE,//TV,PC,GUESS(default)
        STRING_PGS_YUV_MATRIX,//BT601,BT709,GUESS(default)
        STRING_COUNT
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum EDirectVobSubXyBinOptions
    {
        //[ColorSpaceOpt1...ColorSpaceOptN]
        //size=N
        BIN_OUTPUT_COLOR_FORMAT,
        BIN_INPUT_COLOR_FORMAT,

        //struct CachesInfo
        //size = 1
        BIN_CACHES_INFO,

        //struct XyFlyWeightInfo
        //size = 1
        BIN_XY_FLY_WEIGHT_INFO,

        BIN_COUNT
    }
}
