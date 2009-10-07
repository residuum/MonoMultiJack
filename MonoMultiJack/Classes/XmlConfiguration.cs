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
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace MonoMultiJack
{
	/// <summary>
	/// read and write configuration from / to XML
	/// </summary>
	public class XmlConfiguration
	{
		public JackdConfiguration jackdConfig {get; protected set;}
		public List<AppConfiguration> appConfigs {get; protected set;}
		protected string configFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), ".monomultijack.xml");

		/// <summary>
		/// constructor, reads from XML config file
		/// </summary>
		public XmlConfiguration()
		{
			this.appConfigs = new List<AppConfiguration> ();
			if (!this.readConfigFile())
			{
				this.jackdConfig = null;
				this.appConfigs = new List<AppConfiguration> ();
			}
		}
		
		private bool readConfigFile ()
		{
			try
			{
				if (System.IO.File.Exists(configFile))
				{
					XElement xmlFileContent = XElement.Load(configFile);
					foreach (XElement xmlElement in xmlFileContent.Elements())
					{
						switch (xmlElement.Name.ToString())
						{
						case "jackd":
							this.jackdConfig = new JackdConfiguration 
								(xmlElement.Element("path").Value,
								 xmlElement.Element("driver").Value,
								 xmlElement.Element("audiorate").Value);
							break;
						case "applications":
							foreach (XElement app in xmlElement.Elements())
							{
								if (app.Name == "application")
								{
									AppConfiguration appConfig = new AppConfiguration
										(app.Element("name").Value,
										 app.Element("command").Value);
									this.appConfigs.Add(appConfig);
								}
							}
							break;
						}
					}
					return true;
				}
				else
				{
					return false;
				}
			}
			catch 
			{
				return false;
			}
		}
		
		/// <summary>
		/// writes new configuration file, returns true on success
		/// </summary>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		private bool writeConfiguration ()
		{
			XElement xmlFileContent = new XElement ("monomultijack");
			XElement xmlJackd = new XElement ("jackd");
			XElement xmlPath = new XElement ("path");
			xmlPath.Value = this.jackdConfig.path;
			XElement xmlAudiorate = new XElement ("audiorate");
			xmlAudiorate.Value = this.jackdConfig.audiorate;
			XElement xmlDriver = new XElement ("driver");
			xmlDriver.Value = this.jackdConfig.driver;
			xmlJackd.Add (xmlPath);
			xmlJackd.Add (xmlAudiorate);
			xmlJackd.Add (xmlDriver);
			xmlFileContent.Add (xmlJackd);
			XElement xmlApplications = new XElement ("applications");
			foreach (AppConfiguration appConfig in this.appConfigs)
			{
				XElement xmlApp = new XElement ("application");
				XElement xmlName = new XElement ("name");
				xmlName.Value = appConfig.name;
				XElement xmlCommand = new XElement ("command");
				xmlCommand.Value = appConfig.command;
				xmlApp.Add (xmlName);
				xmlApp.Add (xmlCommand);
				xmlApplications.Add (xmlApp);
			}
			xmlFileContent.Add (xmlApplications);
			try
			{
				XDocument xmlDoc = new XDocument
				(
					new XDeclaration("1.0", "utf-8", "yes"),
				    xmlFileContent
				);
				XmlWriterSettings settings = new XmlWriterSettings();
				settings.Indent = true;
				settings.NewLineChars = "\n";
				settings.IndentChars = "\t";
				using (XmlWriter writer = XmlTextWriter.Create(this.configFile, settings))
				{
					xmlDoc.Save(writer);
				}
			}
			catch
			{
				return false;
			}
			return true;
		}
		
		/// <summary>
		/// updates configuration
		/// </summary>
		/// <param name="newJackdConfig">
		/// A <see cref="JackdConfiguration"/> new jackd configuration
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		public bool updateConfiguration (JackdConfiguration newJackdConfig)
		{
			bool status = false;
			if (this.jackdConfig != newJackdConfig)
			{
				this.jackdConfig = newJackdConfig;
				status = this.writeConfiguration();
			}
			else
			{
				status = true;
			}
			return status;
		}
		/// <summary>
		/// updates Configuration
		/// </summary>
		/// <param name="newAppConfigs">
		/// A <see cref="List"/> of <see cref="AppConfiguration"/>s to update
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		public bool updateConfiguration (List<AppConfiguration> newAppConfigs)
		{
			bool status;
			this.appConfigs = newAppConfigs;
			status = this.writeConfiguration();
			return status;
		}
	}
}
