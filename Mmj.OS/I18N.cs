//
// I18N.cs
//
// Author:
//       Thomas Mayer <thomas@residuum.org>
//
// Copyright (c) 2015 Thomas Mayer
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

using System.IO;
using System.Reflection;
using NGettext;

namespace Mmj.OS
{
	public static class I18N
	{
		static readonly ICatalog Catalog = new Catalog ("MonoMultiJack", Path.Combine (Path.GetDirectoryName (Assembly.GetExecutingAssembly ().Location), "locale"));

		public static string _ (string text)
		{
			return Catalog.GetString (text);
		}

		public static string _ (string text, params object[] args)
		{
			return Catalog.GetString (text, args);
		}

		public static string _n (string text, string pluralText, long n)
		{
			return Catalog.GetPluralString (text, pluralText, n);
		}

		public static string _n (string text, string pluralText, long n, params object[] args)
		{
			return Catalog.GetPluralString (text, pluralText, n, args);
		}

		public static string _p (string context, string text)
		{
			return Catalog.GetParticularString (context, text);
		}

		public static string _p (string context, string text, params object[] args)
		{
			return Catalog.GetParticularString (context, text, args);
		}

		public static string _pn (string context, string text, string pluralText, long n)
		{
			return Catalog.GetParticularPluralString (context, text, pluralText, n);
		}

		public static string _pn (string context, string text, string pluralText, long n, params object[] args)
		{
			return Catalog.GetParticularPluralString (context, text, pluralText, n, args);
		}
	}
}
