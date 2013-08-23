//
// MarshallingHelper.cs
//
// Author:
//       Thomas Mayer <thomas@residuum.org>
//
// Copyright (c) 2009-2013 Thomas Mayer
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

namespace MonoMultiJack.ConnectionWrapper
{
	public static class MarshallingHelper
	{
		public static string PtrToString (this IntPtr p)
		{
			if (p == IntPtr.Zero) {
				return null;
			}
			return Marshal.PtrToStringAnsi (p);
		}

		public static string[] PtrToStringArray (this IntPtr stringArray)
		{
			if (stringArray == IntPtr.Zero) {
				return new string[]{};
			} 
 
			ushort arrayCount = stringArray.CountStrings ();
			return stringArray.PtrToStringArray (arrayCount);
		}
 
		static ushort CountStrings (this IntPtr stringArray)
		{
			ushort count = 0;
			while (Marshal.ReadIntPtr (stringArray, count*IntPtr.Size) != IntPtr.Zero) {
				++count;
			}
			return count;
		}
 
		static string[] PtrToStringArray (this IntPtr stringArray, ushort count)
		{
			if (stringArray == IntPtr.Zero) {
				return new string[count];
			} 
 
			string[] members = new string[count];
			for (int i = 0; i < count; ++i) {
				IntPtr s = Marshal.ReadIntPtr (stringArray, i * IntPtr.Size);
				members [i] = PtrToString (s);
			} 
			return members;
		}
	}
}

