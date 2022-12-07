﻿using System.Runtime.InteropServices;

namespace FXRTestBed
{
    public static class Utils
    {
        [DllImport("msvcrt.dll", CallingConvention=CallingConvention.Cdecl)]
        static extern int memcmp(byte[] b1, byte[] b2, long count);

        public static bool ByteArrayCompare(byte[] b1, byte[] b2)
        {
            // Validate buffers are the same length.
            // This also ensures that the count does not exceed the length of either buffer.
            return b1.Length == b2.Length && memcmp(b1, b2, b1.Length) == 0;
        }
    }
}