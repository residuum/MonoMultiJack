// 
// PersistantConfiguration.cs
//  
// Author:
//       Thomas Mayer <thomas@residuum.org>
// 
// Copyright (c) 2009-2012 Thomas Mayer
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
using System.Text;
using System.Xml;
using System.Linq;

namespace MonoMultiJack.BusinessLogic.Configuration
{
	/// <summary>
	/// Class for managing configuration.
	/// </summary>
	public static class PersistantConfiguration
	{
		static readonly string _applicationFolder = Path.Combine(
		Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
		"MonoMultiJack"
			);

		static string JackdConfigFile {
			get {
				return Path.Combine(_applicationFolder, "jack.xml");
			}
		}
		
		static string ApplicationsConfigFile {
			get {
				return Path.Combine(_applicationFolder, "applications.xml");
			}
		}

		static string WindowSizeFile {
			get {
				return Path.Combine(_applicationFolder, "window.xml");
			}
		}

		/// <summary>
		/// Loads the jackd configuration.
		/// </summary>
		/// <returns>
		/// The jackd configuration.
		/// </returns>
		public static JackdConfiguration LoadJackdConfiguration()
		{
			if (File.Exists(JackdConfigFile)) {
				return ReadJackdFile(JackdConfigFile);
			}
			throw new FileNotFoundException();
		}

		static JackdConfiguration ReadJackdFile(string jackdConfigFile)
		{
			XmlDocument jackdConfigXml = new XmlDocument();
			jackdConfigXml.Load(jackdConfigFile);
			XmlNode firstNode = jackdConfigXml.DocumentElement;
			if (firstNode.Name == "jackd") {
				string path = firstNode.SelectSingleNode("path").InnerText;
				string generalOptions = firstNode.SelectSingleNode("general-options").InnerText;
				string driver = firstNode.SelectSingleNode("driver").InnerText;
				string driverOptions = firstNode.SelectSingleNode("driver-options").InnerText;
				return new JackdConfiguration(
					path,
					generalOptions,
					driver,
					driverOptions
				);
			}
			throw new XmlException("XML file is corrupt.");

		}

		/// <summary>
		/// Saves the jackd config.
		/// </summary>
		/// <param name='newJackdConfig'>
		/// New jackd config.
		/// </param>
		public static void SaveJackdConfig(JackdConfiguration newJackdConfig)
		{
			if (!Directory.Exists(_applicationFolder)) {
				Directory.CreateDirectory(_applicationFolder);
			}
			if (!File.Exists(JackdConfigFile)) {
				using (var fs = File.Create(JackdConfigFile)) {
					fs.Close();
				}
			}
			using (XmlTextWriter writer = new XmlTextWriter(JackdConfigFile, System.Text.Encoding.UTF8)) {
				writer.Formatting = Formatting.Indented;
				writer.IndentChar = '\t';
				writer.WriteStartDocument();
				writer.WriteStartElement("jackd");
				writer.WriteElementString("path", newJackdConfig.Path);
				writer.WriteElementString("general-options", newJackdConfig.GeneralOptions);
				writer.WriteElementString("driver", newJackdConfig.Driver);
				writer.WriteElementString("driver-options", newJackdConfig.DriverOptions);
				writer.WriteEndElement();
				writer.Flush();
			}
		}

		public static List<AppConfiguration> LoadAppConfigurations()
		{
			if (File.Exists(ApplicationsConfigFile)) {
				return ReadAppConfigXml(ApplicationsConfigFile);
			}
			throw new FileNotFoundException();
		}

		static List<AppConfiguration> ReadAppConfigXml(string appConfigFile)
		{
			XmlDocument windowSizeXml = new XmlDocument();
			windowSizeXml.Load(appConfigFile);
			XmlNode firstNode = windowSizeXml.DocumentElement;
			if (firstNode.Name == "applications") {
				List<AppConfiguration> appConfigs = new List<AppConfiguration>();
				XmlNodeList appNodes = firstNode.SelectNodes("application");
				foreach (XmlNode appNode in appNodes) {
					string name = appNode.SelectSingleNode("name").InnerText;
					string command = appNode.SelectSingleNode("command").InnerText;
					string arguments = appNode.SelectSingleNode("arguments").InnerText;
					appConfigs.Add(new AppConfiguration(name, command, arguments));
				}
				return appConfigs;
			}
			throw new XmlException("XML file is corrupt.");
		}

		public static void SaveAppConfiguations(List<AppConfiguration> newAppConfigurations)
		{
			if (!Directory.Exists(_applicationFolder)) {
				Directory.CreateDirectory(_applicationFolder);
			}
			if (!File.Exists(ApplicationsConfigFile)) {
				using (var fs = File.Create(ApplicationsConfigFile)) {
					fs.Close();
				}
			}
			using (XmlTextWriter writer = new XmlTextWriter(ApplicationsConfigFile, System.Text.Encoding.UTF8)) {
				writer.Formatting = Formatting.Indented;
				writer.IndentChar = '\t';
				writer.WriteStartDocument();
				writer.WriteStartElement("applications");
				foreach (AppConfiguration appConfig in newAppConfigurations) {
					writer.WriteStartElement("application");
					writer.WriteElementString("name", appConfig.Name);
					writer.WriteElementString("command", appConfig.Command);
					writer.WriteElementString("arguments", appConfig.Arguments);
					writer.WriteEndElement();
				}
				writer.WriteEndElement();
				writer.Flush();
			}
		}

		/// <summary>
		/// Loads the size of the window.
		/// </summary>
		/// <returns>
		/// The window size.
		/// </returns>
		public static WindowConfiguration LoadWindowSize()
		{
			if (File.Exists(WindowSizeFile)) {
				return ReadWindowSizeXml(WindowSizeFile);
			}
			return new WindowConfiguration();
		}

		static WindowConfiguration ReadWindowSizeXml(string windowSizeFile)
		{
			XmlDocument windowSizeXml = new XmlDocument();
			windowSizeXml.Load(windowSizeFile);
			XmlNode firstNode = windowSizeXml.DocumentElement;
			if (firstNode.Name == "window") {
				int xPos, yPos, xSize, ySize;
				string xPosS = firstNode.SelectSingleNode("x-position").InnerText;
				string yPosS = firstNode.SelectSingleNode("y-position").InnerText;
				string xSizeS = firstNode.SelectSingleNode("x-size").InnerText;
				string ySizeS = firstNode.SelectSingleNode("y-size").InnerText;
				if (int.TryParse(xPosS, out xPos)
					&& int.TryParse(yPosS, out yPos)
					&& int.TryParse(xSizeS, out xSize)
					&& int.TryParse(ySizeS, out ySize)) {
					return new WindowConfiguration(xPos, yPos, xSize, ySize);
				}

			}
			return new WindowConfiguration();
		}

		/// <summary>
		/// Saves the size of the window.
		/// </summary>
		/// <param name='newWindowConfig'>
		/// New window config.
		/// </param>
		public static void SaveWindowSize(WindowConfiguration newWindowConfig)
		{
			if (!Directory.Exists(_applicationFolder)) {
				Directory.CreateDirectory(_applicationFolder);
			}
			if (!File.Exists(WindowSizeFile)) {
				using (var fs = File.Create(WindowSizeFile)) {
					fs.Close();
				}
			}
			using (XmlTextWriter writer = new XmlTextWriter(WindowSizeFile, System.Text.Encoding.UTF8)) {
				writer.Formatting = Formatting.Indented;
				writer.IndentChar = '\t';
				writer.WriteStartDocument();
				writer.WriteStartElement("window");
				writer.WriteElementString(
					"x-position",
					newWindowConfig.XPosition.ToString()
				);
				writer.WriteElementString(
					"y-position",
					newWindowConfig.YPosition.ToString()
				);
				writer.WriteElementString("x-size", newWindowConfig.XSize.ToString());
				writer.WriteElementString("y-size", newWindowConfig.YSize.ToString());
				writer.WriteEndElement();
				writer.Flush();
			}
		}
	}
}