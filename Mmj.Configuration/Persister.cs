// 
// Persister.cs
//  
// Author:
//       Thomas Mayer <thomas@residuum.org>
// 
// Copyright (c) 2009-2015 Thomas Mayer
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
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Mmj.Configuration.Configuration;
using Mmj.Configuration.Snapshot;

namespace Mmj.Configuration
{
	/// <summary>
	/// Class for managing configuration.
	/// </summary>
	public static class Persister
	{
		static string ApplicationFolder = Path.Combine (
			                                  Environment.GetFolderPath (Environment.SpecialFolder.ApplicationData),
			                                  "MonoMultiJack"
		                                  );

		static string JackdConfigFile {
			get {
				return Path.Combine (ApplicationFolder, "jack.xml");
			}
		}

		static string ApplicationsConfigFile {
			get {
				return Path.Combine (ApplicationFolder, "applications.xml");
			}
		}

		static string WindowSizeFile {
			get {
				return Path.Combine (ApplicationFolder, "window.xml");
			}
		}

		public static void SetConfigDirectory (string configDirectory)
		{
			ApplicationFolder = configDirectory;
		}

		static string _snapshotFolder = null;

		public static string SnapshotFolder {
			get {
				if (_snapshotFolder != null && Directory.Exists (_snapshotFolder)) {
					return _snapshotFolder;
				}
				return Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);
			}
			private set {
				_snapshotFolder = value;
			}
		}

		/// <summary>
		/// Loads the jackd configuration.
		/// </summary>
		/// <returns>
		/// The jackd configuration.
		/// </returns>
		public static JackdConfiguration LoadJackdConfiguration ()
		{
			if (File.Exists (JackdConfigFile)) {
				return ReadConfig<JackdConfiguration> (JackdConfigFile);
			}
			throw new FileNotFoundException ();
		}

		/// <summary>
		/// Saves the jackd config.
		/// </summary>
		/// <param name='newJackdConfig'>
		/// New jackd config.
		/// </param>
		public static void SaveJackdConfig (JackdConfiguration newJackdConfig)
		{
			SaveConfig (newJackdConfig, JackdConfigFile);
		}

		public static List<AppConfiguration> LoadAppConfigurations ()
		{
			if (File.Exists (ApplicationsConfigFile)) {
				return ReadConfig<AppConfigurationCollection> (ApplicationsConfigFile).Configurations;
			}
			throw new FileNotFoundException ();
		}

		public static void SaveAppConfiguations (IEnumerable<AppConfiguration> newAppConfigurations)
		{
			AppConfigurationCollection collection = new AppConfigurationCollection (newAppConfigurations);
			SaveConfig (collection, ApplicationsConfigFile);
		}

		/// <summary>
		/// Loads the size of the window.
		/// </summary>
		/// <returns>
		/// The window size.
		/// </returns>
		public static WindowConfiguration LoadWindowSize ()
		{
			if (File.Exists (WindowSizeFile)) {
				return ReadConfig<WindowConfiguration> (WindowSizeFile);
			}
			return new WindowConfiguration ();
		}

		/// <summary>
		/// Saves the size of the window.
		/// </summary>
		/// <param name='newWindowConfig'>
		/// New window config.
		/// </param>
		public static void SaveWindowSize (WindowConfiguration newWindowConfig)
		{
			SaveConfig (newWindowConfig, WindowSizeFile);
		}

		public static void SaveSnapshot (Moment snapshot, string fileName)
		{
			_snapshotFolder = Path.GetDirectoryName (fileName);
			SaveConfig (snapshot, fileName);
		}

		public static Moment LoadSnapshot (string fileName)
		{
			_snapshotFolder = Path.GetDirectoryName (fileName);
			return ReadConfig<Moment> (fileName);
		}

		static T ReadConfig<T> (string fileName)
		{
			XmlSerializer serializer = new XmlSerializer (typeof(T));
			using (FileStream file = File.OpenRead (fileName)) {
				return (T)serializer.Deserialize (file);
			}

		}

		static void SaveConfig<T> (T entity, string fileName)
		{
			if (!Directory.Exists (ApplicationFolder)) {
				Directory.CreateDirectory (ApplicationFolder);
			}
			if (!File.Exists (fileName)) {
				using (FileStream fs = File.Create (fileName)) {
					fs.Close ();
				}
			}
			using (XmlTextWriter writer = new XmlTextWriter (fileName, System.Text.Encoding.UTF8)) {
				writer.IndentChar = '\t';
				writer.Formatting = Formatting.Indented;
				XmlSerializer serializer = new XmlSerializer (typeof(T));
				serializer.Serialize (writer, entity);
				writer.Flush ();
			}
		}
	}
}