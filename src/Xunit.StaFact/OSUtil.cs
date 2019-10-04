// Copyright (c) Andrew Arnott. All rights reserved.
// Licensed under the Ms-PL license. See LICENSE.txt file in the project root for full license information.

namespace Xunit
{
#if !NET461
    using System.Runtime.InteropServices;

#endif

    public static class OSUtil
    {
        public static bool IsWindows()
        {
#if NET461
            // Assume we are running on Windows with .NET 4.6.1
            return true;
#else
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
#endif
        }
    }
}