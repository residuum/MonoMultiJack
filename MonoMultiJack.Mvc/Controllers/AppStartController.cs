//
// AppStartController.cs
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
using MonoMultiJack.Widgets;
using MonoMultiJack.OS;
using MonoMultiJack.Configuration;

namespace MonoMultiJack.Controllers
{
	public class AppStartController : IController
	{
		readonly IProgram _application;
		readonly IAppStartWidget _appWidget;

		public AppStartController (AppConfiguration appConfiguration)
		{
			_application = DependencyResolver.GetImplementation<IProgram> ("IProgram", new object[] { appConfiguration });
			_appWidget = new AppStartWidget ();
			_appWidget.SetApp (appConfiguration.Name, appConfiguration.Command);
			_appWidget.StartApplication += AppWidget_StartApplication;
			_appWidget.StopApplication += AppWidget_StopApplication;
			_application.HasExited += Application_HasExited;
			_application.HasStarted += Application_HasStarted;
		}

		public IAppStartWidget Widget {
			get {
				return _appWidget;
			}
		}

		~AppStartController ()
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
			if (_application != null) {
				_application.Stop ();
				_application.Dispose ();
			}
			_appWidget.Dispose ();
		}

		void AppWidget_StartApplication (object sender, EventArgs e)
		{
			_application.Start ();
		}

		void AppWidget_StopApplication (object sender, EventArgs e)
		{
			_application.Stop ();
		}

		void Application_HasExited (object sender, EventArgs e)
		{
			_appWidget.IsRunning = false;
			if (ApplicationStatusHasChanged != null) {
				ApplicationStatusHasChanged (this, new EventArgs ());
			}
		}

		void Application_HasStarted (object sender, EventArgs e)
		{
			_appWidget.IsRunning = true;
			if (ApplicationStatusHasChanged != null) {
				ApplicationStatusHasChanged (this, new EventArgs ());
			}

		}

		public void StopApplication ()
		{
			_application.Stop ();
		}

		public bool IsRunning {
			get {
				return _application.IsRunning;
			}
		}

		public event EventHandler ApplicationStatusHasChanged;
		public event EventHandler AllWidgetsAreClosed;
	}
}