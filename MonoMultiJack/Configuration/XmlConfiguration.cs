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
		public JackdConfiguration jackdConfig;
		
		/// <summary>
		/// Collection of Application Configurations
		/// </summary>
		public List<AppConfiguration> appConfigs;
		
		/// <summary>
		/// Path to config file
		/// </summary>
		protected string _configFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), ".monomultijack.xml");
		
		/// <summary>
		/// constructor
		/// </summary>
		public XmlConfiguration ()
		{
			this.appConfigs = new List<AppConfiguration>();
			if (!this.ReadXml())
			{
				throw new XmlException("Error reading XML configuration file.");
			}
		}
		
		/// <summary>
		/// Creates <seealso cref="this.jackdConfig"/> jackdConfiguration from XML
		/// </summary>
		/// <param name="jackdNode">
		/// A <see cref="XmlNode"/> with the jackd configuration values
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/> indicating success of parsing XML
		/// </returns>
		protected bool LoadJackdXml(XmlNode jackdNode)
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
				this.jackdConfig = new JackdConfiguration(path, driver, audiorate);
				return true;
			}
			catch
			{
				this.jackdConfig = new JackdConfiguration(String.Empty, String.Empty, String.Empty);
				return false;
			}
		}
		
		/// <summary>
		/// Creates <seealso cref="this.appConfigs"/> List of AppConfigurations from XML
		/// </summary>
		/// <param name="applicationNode">
		/// A <see cref="XmlNode"/> with the AppConfigurations values
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/> indicating success of parsing XML
		/// </returns>		
		protected bool LoadApplicationsXml(XmlNode applicationsNode)
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
						this.appConfigs.Add(newApp);
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
		protected bool ReadXml()
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
											if (!this.LoadJackdXml(nodeSecond))
											{
												throw new XmlException();
											}
											break;
										case "applications":
											if (!this.LoadApplicationsXml(nodeSecond))
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
		protected bool WriteXml()
		{
			try
			{
				using (XmlTextWriter writer = new XmlTextWriter(this._configFile, System.Text.Encoding.UTF8))
				{
					writer.Formatting = Formatting.Indented;
					writer.IndentChar = '\t';
					writer.Indentation = 1;
					writer.WriteStartDocument();
					writer.WriteStartElement("monomultijack");
					writer.WriteStartElement("jackd");
					writer.WriteElementString("path", this.jackdConfig.path);
					writer.WriteElementString("driver", this.jackdConfig.driver);
					writer.WriteElementString("audiorate", this.jackdConfig.audiorate);
					writer.WriteEndElement();
					writer.WriteStartElement("applications");
					foreach (AppConfiguration appConfig in this.appConfigs)
					{
						writer.WriteStartElement("application");
						writer.WriteElementString("name", appConfig.name);
						writer.WriteElementString("command", appConfig.command);
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
			this.jackdConfig = newJackdConfig;
			return this.WriteXml();
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
			this.appConfigs = newAppConfigs;
			return this.WriteXml();
		}
	}
}
