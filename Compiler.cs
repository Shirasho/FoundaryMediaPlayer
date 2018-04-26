// Check for operating system defines.
#if !WIN //&& !OSX && !UNIX
    #error Invalid/missing OS configuration.
#endif

// Ensure only one operating system is defined.
// Side note - fuck the C# team for shying away from even the most basic preprocessor support.
//#if WIN ^ OSX ^ UNIX
#if (WIN && OSX) || (WIN && UNIX) || (OSX && UNIX) 
    #error Only one OS configuration may be defined.
#endif

// Force RealMedia and QuickTime to use the custom engine.
//#define OVERRIDE_RM_QT_ENGINE