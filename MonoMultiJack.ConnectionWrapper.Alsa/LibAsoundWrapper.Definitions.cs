// 
// LibAsoundWrapper.Definitions.cs
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
namespace MonoMultiJack.ConnectionWrapper.Alsa
{
	internal static partial class LibAsoundWrapper
	{
		const string ASOUND_LIB_NAME = "libasound";
		const int SND_SEQ_NONBLOCK = 1;
		const int SND_SEQ_OPEN_DUPLEX = 3;
		const int SND_SEQ_PORT_CAP_NO_EXPORT = (1 << 7);
		const int SND_SEQ_PORT_CAP_READ = (1 << 0);
		const int SND_SEQ_PORT_CAP_WRITE = (1 << 1);
		const int SND_SEQ_PORT_CAP_DUPLEX = (1 << 4);
		const int SND_SEQ_USER_CLIENT = 1;
		const int SND_SEQ_PORT_SYSTEM_TIMER = 0;
		const int SND_SEQ_PORT_SYSTEM_ANNOUNCE = 1;
		const int SND_SEQ_PORT_TYPE_APPLICATION = (1 << 20);
		const int SND_SEQ_QUERY_SUBS_READ = 0;
		const int SND_SEQ_QUERY_SUBS_WRITE = 1;
	}
}