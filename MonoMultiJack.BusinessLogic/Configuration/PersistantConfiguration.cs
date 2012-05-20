// 
// XmlConfiguration.cs
//  
// Author:
//       Thomas Mayer <thomas@residuum.org>
// 
// Copyright (c) 2009 Thomas Mayer
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
	public class PersistantConfiguration
	{
		/// <summary>
		/// Jackd Configuration
		/// </summary>
		public JackdConfiguration JackdConfig {get; set;}
		
		/// <summary>
		/// Collection of Application Configurations
		/// </summary>
		public List<AppConfiguration> AppConfigs {get ; set;}
		
		/// <summary>
		/// Path to config file
		/// </summary>
		private readonly string _applicationFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MonoMultiJack");
		private readonly string _configFile;
		
		/// <summary>
		/// constructor
		/// </summary>
		public PersistantConfiguration ()
		{
			_configFile = Path.Combine (_applicationFolder, "configuration.xml");
			AppConfigs = new List<AppConfiguration>();
			if (!ReadXml())
			{
				throw new XmlException("Error reading XML configuration file.");
			}
		}
		
		/// <summary>
		/// Constructor with setting of new configurations
		/// </summary>
		/// <param name="newJackdConfig">
		/// The new <see cref="JackdConfiguration"/>
		/// </param>
		/// <param name="newAppConfigs">
		/// The new <see cref="List<AppConfiguration>"/>
		/// </param>
		public PersistantConfiguration(JackdConfiguration newJackdConfig, List<AppConfiguration> newAppConfigs)
		{
			_configFile = Path.Combine (_applicationFolder, "configuration.xml");
			AppConfigs = newAppConfigs;
			JackdConfig = newJackdConfig;
		}
		
		/// <summary>
		/// Creates <seealso cref="jackdConfig"/> jackdConfiguration from XML
		/// </summary>
		/// <param name="jackdNode">
		/// A <see cref="XmlNode"/> with the jackd configuration values
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/> indicating success of parsing XML
		/// </returns>
		private bool LoadJackdXml (XmlNode jackdNode)
		{
			try
			{
				string path = jackdNode.SelectNodes("path").Item(0).InnerText;
				string generalOptions = jackdNode.SelectNodes("general-options").Item(0).InnerText;
				string driver = jackdNode.SelectNodes("driver").Item(0).InnerText;
				string driverOptions = jackdNode.SelectNodes("driver-options").Item(0).InnerText;
				JackdConfig = new JackdConfiguration(path,generalOptions,driver,driverOptions);
				return true;
			}
			catch
			{
				JackdConfig = new JackdConfiguration(string.Empty, string.Empty, string.Empty, string.Empty);
				return false;
			}
		}
		
		/// <summary>
		/// Creates <seealso cref="appConfigs"/> List of AppConfigurations from XML
		/// </summary>
		/// <param name="applicationNode">
		/// A <see cref="XmlNode"/> with the AppConfigurations values
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/> indicating success of parsing XML
		/// </returns>		
		private bool LoadApplicationsXml (XmlNodeList applicationNodes)
		{
			try
			{
				foreach (XmlNode applicationNode in applicationNodes)
				{
					string name = applicationNode.SelectNodes ("name").Item (0).InnerText;
					string command = applicationNode.SelectNodes ("command").Item (0).InnerText;
					AppConfiguration newApp = new AppConfiguration (name, command);
					AppConfigs.Add (newApp);
				}
				return true;
			}
			catch (Exception e)
			{
				#if DEBUG
				Console.WriteLine (e.Message);
				#endif
				return false;
			}
		}
		
		/// <summary>
		/// Reads XML configuration
		/// </summary>
		/// <returns>
		/// A <see cref="System.Boolean"/> indicating success of parsing configuration file.
		/// </returns>
		private bool ReadXml ()
		{
			if (File.Exists (_configFile))
			{
				XmlDocument xmlConfigFile = new XmlDocument ();
				try
				{
					xmlConfigFile.Load (_configFile);
					XmlNode firstNode = xmlConfigFile.DocumentElement;
					if (firstNode.Name == "monomultijack")
					{
						LoadJackdXml (firstNode.SelectNodes ("jackd").Item(0));
						LoadApplicationsXml(firstNode.SelectNodes("applications").Item(0).SelectNodes("application"));
					}
					return true;
				}
				catch (Exception e)
				{
					#if DEBUG
					Console.WriteLine (e.Message);
					#endif
					return false;
				}
			}
			else
			{
				return false;
			}
		}
		
		/// <summary>
		/// Writes XML configuration file
		/// </summary>
		/// <returns>
		/// A <see cref="System.Boolean"/> indicating Success of writing XML file
		/// </returns>
		private bool WriteXml ()
		{
			try
			{
				if (!Directory.Exists (_applicationFolder))
				{
					Directory.CreateDirectory (_applicationFolder);
				}
				if (!File.Exists (_configFile))
				{
					using (var fs = File.Create(_configFile))
					{
						fs.Close ();
					}
				}
				using (XmlTextWriter writer = new XmlTextWriter(_configFile, System.Text.Encoding.UTF8))
				{
					writer.Formatting = Formatting.Indented;
					writer.IndentChar = '\t';
					writer.Indentation = 1;
					writer.WriteStartDocument ();
					writer.WriteStartElement ("monomultijack");
					writer.WriteStartElement ("jackd");
					writer.WriteElementString ("path", JackdConfig.Path);
					writer.WriteElementString ("general-options", JackdConfig.GeneralOptions);
					writer.WriteElementString ("driver", JackdConfig.Driver);
					writer.WriteElementString ("driver-options", JackdConfig.DriverOptions);
					writer.WriteEndElement ();
					writer.WriteStartElement ("applications");
					foreach (AppConfiguration appConfig in AppConfigs)
					{
						writer.WriteStartElement ("application");
						writer.WriteElementString ("name", appConfig.Name);
						writer.WriteElementString ("command", appConfig.Command);
						writer.WriteEndElement ();
					}
					writer.WriteEndElement ();
					writer.WriteEndElement ();
					writer.Flush ();
				}
				return true;
			}
			catch (Exception e)
			{
				#if DEBUG
				Console.WriteLine (e.Message);
				#endif
				return false;
			}
		}
		
		/// <summary>
		/// Updates Configuration
		/// </summary>
		/// <param name="newJackdConfig">
		/// The new <see cref="JackdConfiguration"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/> indicating success of update
		/// </returns>
		public bool UpdateConfiguration (JackdConfiguration newJackdConfig)
		{
			JackdConfig = newJackdConfig;
			return WriteXml();
		}
		
		/// <summary>
		/// Updates configuration
		/// </summary>
		/// <param name="newAppConfigs">
		/// The new <see cref="List<AppConfiguration>"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/> indicating success of update
		/// </returns>
		public bool UpdateConfiguration (List<AppConfiguration> newAppConfigs)
		{
			AppConfigs = newAppConfigs;
			return WriteXml();
		}
	}
}
