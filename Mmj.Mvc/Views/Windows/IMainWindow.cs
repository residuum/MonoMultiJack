//
// IMainWindow.cs
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
using System.Collections.Generic;
using Mmj.Configuration;
using Mmj.Configuration.Configuration;
using Mmj.Views.Widgets;

namespace Mmj.Views.Windows
{
	public interface IMainWindow : IWindow
	{
		/// <summary>
		/// Sets the app start widgets.
		/// </summary>
		/// <value>
		/// The app start widgets.
		/// </value>
		IEnumerable<IAppStartWidget> AppStartWidgets { set; }

		/// <summary>
		/// Sets the connection widgets.
		/// </summary>
		/// <value>
		/// The connection widgets.
		/// </value>
		IEnumerable<IConnectionWidget> ConnectionWidgets { set; }

		/// <summary>
		/// Sets the window configuration.
		/// </summary>
		/// <value>
		/// The window configuration.
		/// </value>
		WindowConfiguration WindowConfiguration { get; set; }

		/// <summary>
		/// Sets a value indicating whether this <see cref="Mmj.Views.Windows.IMainWindow"/> jackd is active.
		/// </summary>
		/// <value>
		/// <c>true</c> if jackd is active; otherwise, <c>false</c>.
		/// </value>
		bool JackdIsRunning { set; }

		/// <summary>
		/// Sets a value indicating whether this <see cref="Mmj.Views.Windows.IMainWindow"/> apps are running.
		/// </summary>
		/// <value>
		/// <c>true</c> if apps are running; otherwise, <c>false</c>.
		/// </value>
		bool AppsAreRunning { set; }

		bool Fullscreen { get; set; }

		/// <summary>
		/// Controller should start Jackd.
		/// </summary>
		event EventHandler StartJackd;
		/// <summary>
		/// Controller should stop Jackd.
		/// </summary>
		event EventHandler StopJackd;
		/// <summary>
		/// Controller should stop all audio apps and Jackd.
		/// </summary>
		event EventHandler StopAll;
		/// <summary>
		/// Controller should show Jackd configuration.
		/// </summary>
		event EventHandler ConfigureJackd;
		/// <summary>
		/// Controller should show configuration for audio applications.
		/// </summary>
		event EventHandler ConfigureApps;
		/// <summary>
		/// Controller should show about.
		/// </summary>
		event EventHandler About;
		/// <summary>
		/// Controller should show help.
		/// </summary>
		event EventHandler Help;
		/// <summary>
		/// Controller should quit application.
		/// </summary>
		event EventHandler Quit;
	}
}