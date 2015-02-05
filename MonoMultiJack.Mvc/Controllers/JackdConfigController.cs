//
// JackdConfigController.cs
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
using MonoMultiJack.Configuration;
using MonoMultiJack.Controllers.EventArguments;
using MonoMultiJack.Windows;

namespace MonoMultiJack.Controllers
{
	public class JackdConfigController : IController
	{
		JackdConfiguration _jackdConfig;
		readonly IJackdConfigWindow _jackdConfigWindow;

		public JackdConfigController (JackdConfiguration jackdConfig)
		{
			_jackdConfig = jackdConfig;
			_jackdConfigWindow = new JackdConfigWindow ();
			_jackdConfigWindow.Closing += HandleClosing;
			_jackdConfigWindow.SaveJackd += HandleSaveJackd;
			_jackdConfigWindow.Icon = Icons.Program;
			_jackdConfigWindow.Show ();
			_jackdConfigWindow.Path = jackdConfig.Path;
			_jackdConfigWindow.DriverOptions = jackdConfig.GeneralOptions;
			_jackdConfigWindow.Driver = jackdConfig.Driver;
			_jackdConfigWindow.DriverOptions = jackdConfig.DriverOptions;
		}

		~JackdConfigController ()
		{
			Dispose (false);
		}

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		protected virtual void Dispose (bool isDisposing)
		{
			_jackdConfigWindow.Dispose ();
		}

		void HandleSaveJackd (object sender, EventArgs e)
		{
			IJackdConfigWindow configWindow = sender as IJackdConfigWindow;
			if (configWindow != null && UpdateJackd != null) {
				_jackdConfig = new JackdConfiguration (
					configWindow.Path,
					configWindow.GeneralOptions,
					configWindow.Driver,
					configWindow.DriverOptions);
				if (UpdateJackd != null) {
					UpdateJackd (this, new UpdateJackdEventArgs { JackdConfiguration = _jackdConfig });
				}
			}
		}

		void HandleClosing (object sender, EventArgs e)
		{
			if (AllWidgetsAreClosed != null) {
				AllWidgetsAreClosed (this, e);
			}
		}

		public event EventHandler AllWidgetsAreClosed;
		public event EventHandler<UpdateJackdEventArgs> UpdateJackd;
	}
}