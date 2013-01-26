//
// PointerConversions.cs
//
// Author:
//       Thomas Mayer <thomas@residuum.org>
//
// Copyright (c) 2013 Thomas Mayer
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Runtime.InteropServices;

namespace MonoMultiJack.ConnectionWrapper.Alsa.Types
{
	internal static class PointerConversions
	{
		public static SndSeqAddr PtrToSndSeqAddr (this IntPtr ptr)
		{
			try {
				return (SndSeqAddr)Marshal.PtrToStructure (
				    ptr,
				    typeof(SndSeqAddr)
				);
			} catch (Exception e) {
				#if DEBUG
				Console.WriteLine (e.Message);
				#endif
				return new SndSeqAddr ();
			}
		}

		public static IntPtr SndSeqAddrToPtr (this SndSeqAddr addr)
		{
			IntPtr ptr = typeof(SndSeqAddr).Malloc ();
			Marshal.StructureToPtr (addr, ptr, false);
			return ptr;
		}

		static IntPtr Malloc (this Type type)
		{
			return Marshal.AllocHGlobal (Marshal.SizeOf (type));
		}
	}
}

