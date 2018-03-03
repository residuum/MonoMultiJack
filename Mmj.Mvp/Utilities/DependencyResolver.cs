//
// DependencyResolver.cs
//
// Author:
//       Thomas Mayer <thomas@residuum.org>
//
// Copyright (c) 2009-2014 Thomas Mayer
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
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Mmj.Utilities
{
	static class DependencyResolver
	{
		public static T GetImplementation<T> (string configEntry, object[] parameters) where T : class
		{
			string appSetting = ConfigurationManager.AppSettings [configEntry];
			Type type = Type.GetType (appSetting);
			Debug.Assert (type != null);
			Type[] parameterTypes = (parameters ?? Enumerable.Empty<object> ()).Select (parameter => parameter.GetType ()).ToArray ();
			ConstructorInfo ctor = type.GetConstructor (parameterTypes);
			Debug.Assert (ctor != null);
			T instance = ctor.Invoke (parameters) as T;
			Debug.Assert (instance != null);
			return instance;
		}

		public static T GetImplementation<T> (string configEntry) where T : class
		{
			return GetImplementation<T> (configEntry, null);
		}
	}
}