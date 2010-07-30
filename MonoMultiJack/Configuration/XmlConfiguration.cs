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
using System.Xml;

namespace MonoMultiJack.Configuration
{
	/// <summary>
	/// Class for reading and writing XML configuration file.
	/// </summary>
	public class XmlConfiguration
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
		private string _configFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), ".monomultijack.xml");
		
		/// <summary>
		/// constructor
		/// </summary>
		public XmlConfiguration ()
		{
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
		public XmlConfiguration(JackdConfiguration newJackdConfig, List<AppConfiguration> newAppConfigs)
		{
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
		private bool LoadJackdXml(XmlNode jackdNode)
		{
			try
			{
				string path = String.Empty;
				string driver = String.Empty;
				string audiorate = String.Empty;
				foreach (XmlNode nodeThird in jackdNode.ChildNodes)
				{
					switch (nodeThird.Name)						
					{
						case "path":
							path = nodeThird.InnerText;
							break;
						case "driver":
							driver = nodeThird.InnerText;
							break;
						case "audiorate":
							audiorate = nodeThird.InnerText;
							break;
						default:
							break;
					}
				}
				JackdConfig = new JackdConfiguration(path, driver, audiorate);
				return true;
			}
			catch
			{
				JackdConfig = new JackdConfiguration(String.Empty, String.Empty, String.Empty);
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
		private bool LoadApplicationsXml(XmlNode applicationsNode)
		{
			try
			{
				foreach (XmlNode applicationNode in applicationsNode.ChildNodes)
				{
					if (applicationNode.Name == "application")
					{
						string name = String.Empty;
						string command = String.Empty;
						foreach (XmlNode subnode in applicationNode.ChildNodes)						
						{				
							switch (subnode.Name)						
							{
								case "name":
									name = subnode.InnerText;
									break;
								case "command":
									command = subnode.InnerText;
									break;
								default:
									break;
							}
						}
						AppConfiguration newApp = new AppConfiguration(name, command);
						AppConfigs.Add(newApp);
					}
				}
				return true;
			}
			catch
			{
				return false;
			}
		}
		
		/// <summary>
		/// Reads XML configuration
		/// </summary>
		/// <returns>
		/// A <see cref="System.Boolean"/> indicating success of parsing configuration file.
		/// </returns>
		private bool ReadXml()
		{
			if (File.Exists(_configFile))
			{
				XmlDocument xmlConfigFile = new XmlDocument();
				try
				{
					xmlConfigFile.Load(_configFile);
					foreach (XmlNode nodeFirst in xmlConfigFile.ChildNodes)
					{
						switch (nodeFirst.Name)
						{
							case "monomultijack":
								foreach (XmlNode nodeSecond in nodeFirst.ChildNodes)
								{
									switch (nodeSecond.Name)
									{
										case "jackd":
											if (!LoadJackdXml(nodeSecond))
											{
												throw new XmlException();
											}
											break;
										case "applications":
											if (!LoadApplicationsXml(nodeSecond))
											{
												throw new XmlException();
											}
											break;
									}								
								}
								break;
							default:
								break;
						}
							
					}
					return true;
				}
				catch
				{
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
		private bool WriteXml()
		{
			try
			{
				using (XmlTextWriter writer = new XmlTextWriter(_configFile, System.Text.Encoding.UTF8))
				{
					writer.Formatting = Formatting.Indented;
					writer.IndentChar = '\t';
					writer.Indentation = 1;
					writer.WriteStartDocument();
					writer.WriteStartElement("monomultijack");
					writer.WriteStartElement("jackd");
					writer.WriteElementString("path", JackdConfig.Path);
					writer.WriteElementString("driver", JackdConfig.Driver);
					writer.WriteElementString("audiorate", JackdConfig.Audiorate);
					writer.WriteEndElement();
					writer.WriteStartElement("applications");
					foreach (AppConfiguration appConfig in AppConfigs)
					{
						writer.WriteStartElement("application");
						writer.WriteElementString("name", appConfig.Name);
						writer.WriteElementString("command", appConfig.Command);
						writer.WriteEndElement();
					}
					writer.WriteEndElement();
					writer.WriteEndElement();
					writer.Flush();
				}
				return true;
			}
			catch
			{
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
		
		public static string GetScriptHeader()
		{
			return "#!/bin/sh\n";
		}
		
		public static string GetScriptFooter()
		{
			return " >> /dev/null 2>&1&\necho $!";
		}
	}
}
