// Check for operating system defines.
#if !WIN
#error Invalid/missing OS configuration.
#endif

// Ensure only one operating system is defined.
//#if WIN ^ OSX ^ UNIX
#if (WIN && OSX) || (WIN && UNIX) || (OSX && UNIX)
#error Only one OS configuration may be defined.
#endif

#region Define Valiation

#if ENABLE_CONSOLE && !WIN
#error The console window is not supported for this platform.
#endif

#endregion

/*
 * To enable these defines, add them to the project predefined
 * compilation symbols list.
 *
 *
 * Force RealMedia and QuickTime to use the custom engine.
 * OVERRIDE_RM_QT_ENGINE
 *
 * Whether the console is allowed on this platform.
 * ENABLE_CONSOLE
 */